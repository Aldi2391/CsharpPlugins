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
        public LoadingWindowsViewModel LoadingSettings { get; }

        public ModelCleaner(LoadingWindowsViewModel loadingSettings)
        {
            this.LoadingSettings = loadingSettings;
        }


        /// <summary>
        /// Очистка модели от неиспоьльзуемых элементов
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string ClearModelWithoutRules(OutputModel model)
        {
            return string.Empty;
        }


        /// <summary>
        /// Функция по открытию модели
        /// </summary>
        /// <param name="model">Модель для выгрузки</param>
        /// <returns>Документ Revit</returns>
        private Document OpenRevitModel(OutputModel model)
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
        private bool CloseAndSaveModel(OutputModel model, Document currentDocument)
        {
            bool flag = false;

            string documentName = currentDocument.Title;

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
                            File.Create(saveFolder);
                        }
                        catch
                        {
                            // Добавить логгирование
                        }
                    }
                    else
                    {
                        string fullSavePath = GenerateSavingPath(model, fullPath:true);
                        if (fullSavePath != null)
                        {
                            try
                            {
                                currentDocument.SaveAs(fullSavePath, saveOpt);
                                if(File.Exists(fullSavePath))
                                    flag = true;
                            }
                            catch
                            {
                                // Добавить логгирование
                            }
                        }
                    }
                }
            }
            catch
            {
                // Добавить логгирование 
            }

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
                string projectName = LoadingSettings.ProjectSettings.ProjectName;
                string chapter = LoadingSettings.ProjectSettings.Chapter;
                string modelName = LoadingSettings.ProjectSettings.ModelName;
                string shortModelName = modelName.Replace(LoadingSettings.ProjectCode, "").Replace(".rvt", "");
                string modelVersion = (savingModel.Version + 1).ToString();

                string firstPartPath = LoadingSettings.ProjectSettings.PathToSaveModels;
                
                if (fullPath)
                {
                    secondPartPath = $"{projectName}\\{chapter}\\{shortModelName}_{modelVersion}\\{modelName}";
                }
                else
                {
                    secondPartPath = $"{projectName}\\{chapter}\\{shortModelName}_{modelVersion}";
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
    }
}
