using Autodesk.Revit.DB;
using SharedModelUnloader.Models;
using SharedModelUnloader.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModelUnloader.Infrastructure.Helpers
{
    internal class ModelCleaner
    {
        #region Свойства
        public LoadingWindowsViewModel LoadingSettings { get; }
        #endregion


        #region Конструктор
        public ModelCleaner(LoadingWindowsViewModel loadingSettings)
        {
            this.LoadingSettings = loadingSettings;
        }
        #endregion


        #region Функции
        /// <summary>
        /// Очистка модели от неиспоьльзуемых элементов
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool ClearModelWithoutRules(Document currentDocument)
        {
            bool successFlag = false;

            string purgeGuid = "e8c63650-70b7-435a-9010-ec97660c1bda";
            List<PerformanceAdviserRuleId> purgeRuleIds = new List<PerformanceAdviserRuleId>();

            var adviser = PerformanceAdviser.GetPerformanceAdviser();
            var allRuleIds = adviser.GetAllRuleIds();

            foreach (var ruleId in allRuleIds)
            {
                if (ruleId.Guid.ToString() == purgeGuid)
                    purgeRuleIds.Add(ruleId);
            }

            var failureMessages = adviser.ExecuteRules(currentDocument, purgeRuleIds);

            if (failureMessages.Count > 0)
            {
                var deleteElementIds = failureMessages[0].GetFailingElements();

                try
                {
                    using(var t = new Transaction(currentDocument, "Clearing document"))
                    {
                        t.Start();

                        var countOfDelete  = currentDocument.Delete(deleteElementIds);

                        t.Commit();

                        if (countOfDelete.Count > 0)
                            successFlag = true;
                    }
                }

                catch
                {
                    // Добавить логгирование
                }

            }

            return successFlag;
        }


        /// <summary>
        /// Функция по открытию модели
        /// </summary>
        /// <param name="model">Модель для выгрузки</param>
        /// <returns>Документ Revit</returns>
        public Document OpenRevitModel(OutputModel model)
        {
            Document document = null;

            var modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(model.PathToFileRS);
            var openOption = new OpenOptions();
            openOption.OpenForeignOption = OpenForeignOption.Open;
            openOption.DetachFromCentralOption = DetachFromCentralOption.DetachAndPreserveWorksets;
            openOption.SetOpenWorksetsConfiguration(
                new WorksetConfiguration(WorksetConfigurationOption.CloseAllWorksets)
            );

            try
            {
                document = LoadingSettings.UIApplication.Application.
                    OpenDocumentFile(modelPath, openOption);
            }
            catch
            {
                // Добавить логгирование
            }

            return document;
        }


        /// <summary>
        /// Сохранение и закрытие документа
        /// </summary>
        /// <param name="currentDocument">Текущий открытый документ</param>
        /// <returns>Флаг закрытия</returns>
        public bool CloseAndSaveModel(OutputModel model, Document currentDocument)
        {
            bool flag = false;

            var worksetOpt = new WorksharingSaveAsOptions();
            worksetOpt.SaveAsCentral = true;
            worksetOpt.OpenWorksetsDefault = SimpleWorksetConfiguration.AskUserToSpecify;

            var saveOpt = new SaveAsOptions();
            saveOpt.OverwriteExistingFile = true;
            saveOpt.Compact = true;
            saveOpt.SetWorksharingOptions(worksetOpt);

            // Освобождение РН
            try 
            { 
                if (currentDocument.IsWorkshared)
                {
                    WorksharingUtils.RelinquishOwnership(
                        currentDocument,
                        new RelinquishOptions(true),
                        new TransactWithCentralOptions()
                    );
                }
            }
            catch
            {
                // Добавить логгирование
            }

            // Сохранение модели
            try
            {
                string saveFolder = GenerateSavingPath(model);
                if (saveFolder != null)
                {
                    if (!File.Exists(saveFolder))
                    {
                        try
                        {
                            Directory.CreateDirectory(saveFolder);
                        }
                        catch
                        {
                            // Добавить логгирование
                            currentDocument.Close(false);
                            return flag;
                        }
                    }
                    string fullSavePath = GenerateSavingPath(model, fullPath:true);
                    if (fullSavePath != null)
                    {
                        try
                        {
                            var modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(fullSavePath);
                            currentDocument.SaveAs(modelPath, saveOpt);

                            if (File.Exists(fullSavePath))
                                // Запись пути к модели
                                model.PathToFileInShared = fullSavePath;
                                flag = true;
                        }
                        catch
                        {
                            // Удаляем созданную ранее папку
                            Directory.Delete(saveFolder);
                        }
                    }
                }
            }
            catch
            {
                // Добавить логгирование
            }

            currentDocument.Close(false);
            return flag;
        }


        /// <summary>
        /// Создание пути для сохранения 
        /// </summary>
        /// <param name="loadingSettings">Настройки проекта</param>
        /// <param name="savingModel">Сохраняемая модель</param>
        /// <returns>Путь к файле или папке</returns>
        private string GenerateSavingPath(OutputModel savingModel, bool fullPath = false)
        {
            string savingPath = null;
            string secondPartPath = null;

            try
            {
                string sepator = LoadingSettings.ProjectSettings.FieldSeparator.ToString();
                string chapter = LoadingSettings.ProjectSettings.Chapter;
                string modelName = savingModel.Name;
                string shortModelName = modelName.Replace(LoadingSettings.ProjectCode + sepator, "").Replace(".rvt", "");
                string modelVersion = (savingModel.Version).ToString();

                string firstPartPath = LoadingSettings.ProjectSettings.PathToSaveModels;
                
                if (fullPath)
                {
                    secondPartPath = $"{chapter}\\{shortModelName}_V{modelVersion}\\{modelName}.rvt";
                }
                else
                {
                    secondPartPath = $"{chapter}\\{shortModelName}_V{modelVersion}";
                }
                
                if (!string.IsNullOrEmpty(firstPartPath) && !string.IsNullOrEmpty(secondPartPath))
                    savingPath = Path.Combine(firstPartPath, secondPartPath);
            }
            catch
            {
                // Добавить логгирование
            }
            return savingPath;
        }
        
        
        /// <summary>
        /// Сохранения данных о корректировке модели в тектовый файл
        /// </summary>
        /// <param name="savingModel">Исходящая модель</param>
        /// <returns>Флаг успешного завершения</returns>
        public void SaveReport(OutputModel savingModel)
        {
            string folderPath = GenerateSavingPath(savingModel);

            if(!string.IsNullOrEmpty(folderPath))
            {
                string publishDate = InfoSeeker.GetCurrentDateTime();
                
                string textMessage = $"Дата - {publishDate}\nАвтор - {savingModel.Author}\nКраткое описание - {savingModel.Description}";

                string fullPath = folderPath + "\\Изменения.txt";

                File.WriteAllText(fullPath, textMessage);
            }
        }
        #endregion
    }
}
