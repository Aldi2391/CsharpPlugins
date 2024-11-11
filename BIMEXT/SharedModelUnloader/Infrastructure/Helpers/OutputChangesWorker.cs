using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using SharedModelUnloader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModelUnloader.Infrastructure.Helpers
{
    internal class OutputChangesWorker
    {
        /// <summary>
        /// Обновление данных по версии из БД
        /// </summary>
        /// <param name="outputModels">Модели для выгрузки</param>
        public static async Task UpdateOutputModels(List<OutputModel> outputModels, string pathToDB)
        {
            if (outputModels != null && outputModels.Count > 0)
            {
                DataBaseHelper dbHelper = new DataBaseHelper(pathToDB);
                
                foreach (var model in outputModels)
                {
                    string modelName = model.Name;
                    try
                    {
                        ModelRecord latestRecord = await dbHelper.GetLatestModelRecordAsync(modelName);

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
        public static List<OutputModel> GetOutputModels(ProjectSettings settings)
        {
            List<OutputModel> outData = new List<OutputModel>();
            
            // Получение списка с моделями
            List<string> modelPaths = InfoSeeker.ReadFileToList(settings);

            // Проверка наличия путей
            if (modelPaths.Count == 0)
                return outData;

            // Получение всех данных из настроек
            List<string> projectFeatures = GetAllSubstringFromSettings(settings);

            // Создание элементов
            foreach (var modelPath in modelPaths)
            {
                if (ContainsAllSubstring(modelPath, projectFeatures))
                {
                    string modelName = modelPath.Split('\\')[modelPath.Length - 1];
                    int version = 0;
                    string description = null;
                    string date = null;

                    var outputModel = new OutputModel(modelName, version, description, date, settings);
                    outData.Add(outputModel);
                }
            }

            return outData;
        }


        /// <summary>
        /// Проверка наличия всех признаков нужной модели
        /// </summary>
        /// <param name="inputString">Путь к модели</param>
        /// <param name="substings">Признаки модели</param>
        /// <returns>Да\Нет</returns>
        private static bool ContainsAllSubstring(string inputString, List<string> substings)
        {
            if(substings.Count == 0 || inputString == null || substings == null)
                return false;
            
            string lowerInputString = inputString.ToLower();

            IEnumerable<string> lowerSubstrings = from substing in substings
                                           where substing != null
                                           select substing.ToLower();

            return lowerSubstrings.All(sub => inputString.Contains(sub));
        }


        /// <summary>
        /// Получение всех признаков модели
        /// </summary>
        /// <param name="settings">Настроки проекта</param>
        /// <returns>Список характеристик</returns>
        private static List<string> GetAllSubstringFromSettings(ProjectSettings settings)
        {
            List<string> outData = new List<string>();

            List<string> features = new List<string>
            {
                settings.ProjectCode,
                settings.Priority,
                settings.HomeNumber,
                settings.Chapter,
                settings.Phase
            };

            foreach (var feature in features)
            {
                if (feature == null)
                    continue;
                outData.Add(feature);
            }

            return outData;
        }
    }
}
