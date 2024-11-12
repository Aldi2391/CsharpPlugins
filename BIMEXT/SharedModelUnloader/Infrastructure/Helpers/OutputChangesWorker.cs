using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using SharedModelUnloader.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SharedModelUnloader.Infrastructure.Helpers
{
    
    internal class OutputChangesWorker
    {
        /// <summary>
        /// Обновление данных по версии из БД
        /// </summary>
        /// <param name="outputModels">Модели для выгрузки</param>
        public static void UpdateOutputModels(List<OutputModel> outputModels, string pathToDB)
        {
            if (outputModels != null && outputModels.Count > 0)
            {
                DataBaseHelper dbHelper = new DataBaseHelper(pathToDB);
                
                foreach (var model in outputModels)
                {
                    string modelName = model.Name;
                    try
                    {
                        ModelRecord latestRecord = dbHelper.GetLatestModelRecord(modelName);

                        if (latestRecord != null)
                        {
                            if (latestRecord.ModelVersion > model.Version)
                            {
                                model.Version = latestRecord.ModelVersion;
                                model.Author = latestRecord.UserName;
                                model.AuthorEmail = latestRecord.UserEmail;
                                model.Description = latestRecord.ModelDescription;
                                model.Date = latestRecord.PublishDate;
                            }
                        }
                        else
                        {
                            model.Version = 0;
                            model.Author = "-";
                            model.AuthorEmail = "-";
                            model.Description = "Не публиковалась";
                            model.Date = "-";
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }
        

        /// <summary>
        /// Создание элементов OutputModel
        /// </summary>
        /// <param name="settings">Настройки проекта</param>
        /// <returns></returns>
        public static List<OutputModel> GetOutputModelsFromFile(ProjectSettings settings, string userName)
        {
            // Создаем выходной список
            List<OutputModel> outData = new List<OutputModel>();
            
            // Получение списка с моделями
            List<string> modelPaths = InfoSeeker.ReadFileToList(settings);

            // Проверка наличия путей
            if (modelPaths.Count == 0)
                return outData;

            // Создание элементов
            foreach (var modelPath in modelPaths)
            {
                // Получение имени
                string currentModelName = "Имя модели не определено";
                string tmpModelPath = modelPath.Replace(settings.RevitServerFolder + "\\", "");
                string[] folders = tmpModelPath.Split('\\');
                try
                {
                    string lastPart = folders[folders.Length - 1];
                    if (lastPart.Contains(".rvt"))
                        currentModelName = lastPart;
                }
                catch
                { }

                bool flagToAdd = IsSectionModel(currentModelName, settings);

                if (flagToAdd)
                {
                    int version = 0;
                    string description = null;
                    string date = null;

                    var outputModel = new OutputModel(currentModelName, version, description, date, userName, settings);
                    
                    outData.Add(outputModel);
                }
            }

            return outData;
        }


        /// <summary>
        /// Определение флага для фильтрации сторонних моделей
        /// </summary>
        /// <param name="verifiableName">Проверяемое имя</param>
        /// <param name="settings">Параметры проекта</param>
        /// <returns></returns>
        private static bool IsSectionModel(string verifiableName, ProjectSettings settings)
        {
            char fieldSeparator = settings.FieldSeparator;
            
            List<int> emptyIndexes = GetEmptyIndexes(settings);

            string[] templateList = settings.ModelName.Split(fieldSeparator);

            foreach(var ind in emptyIndexes)
            {
                templateList[ind] = "[^_]+";
            }

            string pattern = $"^{string.Join(fieldSeparator.ToString(), templateList)}$";

            return Regex.IsMatch(verifiableName, pattern, RegexOptions.IgnoreCase);
        }


        /// <summary>
        /// Получение индексов с пропусками полей из шаблона наименования
        /// </summary>
        /// <param name="settings">Настройки проекта</param>
        /// <returns></returns>
        private static List<int> GetEmptyIndexes(ProjectSettings settings)
        {
            List<int> emptyIndexes = new List<int>();

            string scheme = settings.Scheme;
            char fieldSeparator = settings.FieldSeparator;

            string[] fields = scheme.Split(fieldSeparator);

            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i] == "#")
                    emptyIndexes.Add(i);
            }

            return emptyIndexes;
        }
    }
}
