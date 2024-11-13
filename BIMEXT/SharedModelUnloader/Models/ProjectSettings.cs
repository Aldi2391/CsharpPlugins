using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace SharedModelUnloader.Models
{
    /// <summary>
    /// Класс для работы с преднастройками
    /// </summary>
    internal class ProjectSettings
    {
        #region Свойства
        private string _PathToFolderSetting { get; }

        public string ModelName { get; }

        public JArray JsonArr { get; }

        public string ProjectName { get; }

        public string ProjectCode { get; }

        public string Priority { get; }

        public string HomeNumber { get; }

        public string Chapter { get; }

        public string Phase { get; }

        public string RevitServer { get; }

        public string PathToSaveModels { get; }

        public string PathToDB { get; }

        public string RevitServerFolder { get; }

        public string PathToRevitServerModelsInfo { get; }

        public List<string> IgnoreFolderRules { get; }

        public string Scheme { get; }

        public char FieldSeparator { get; }
        #endregion


        #region Конструктор
        public ProjectSettings(string modelName, string projectCode, string pathToFolderSettings)
        {
            // Получение файла с настройками
            ModelName = modelName;
            ProjectCode = projectCode;
            _PathToFolderSetting = pathToFolderSettings;
            JsonArr = GetJsonArray();

            // Парсинг данных
            ProjectName = GetStringValueFromJson("projectInfo", "project_name");
            Priority = GetDataFromScheme(ModelName, "projectCode", "Очередь");
            HomeNumber = GetDataFromScheme(ModelName, "projectCode", "НомерДома");
            Chapter = GetDataFromScheme(ModelName, "projectCode", "ШифрРаздела");
            Phase = GetDataFromScheme(ModelName, "projectCode", "СтадияПроектирования");
            RevitServer = GetStringValueFromJson("projectInfo", "revit_server");
            RevitServerFolder = GetStringValueFromJson("projectInfo", "revit_server_folder");
            PathToSaveModels = GetStringValueFromJson("paths", "path_to_save_models");
            PathToDB = GetStringValueFromJson("paths", "path_to_db");
            PathToRevitServerModelsInfo = GetStringValueFromJson("paths", "path_to_revit_server_models_info");
            IgnoreFolderRules = GetArrayValueFromJson("ignoreFolderRules", "folder_names");
            Scheme = GetStringValueFromJson("projectCode", "scheme");
            FieldSeparator = (GetStringValueFromJson("projectCode", "field_separator")?.ToCharArray())[0];
        }
        #endregion


        #region Служебные функции для получения данных
        /// <summary>
        /// Получение объекта JArray 
        /// </summary>
        /// <returns>Объект JArray</returns>
        private JArray GetJsonArray()
        {
            if(ProjectCode == null)
                return null;
            
            var jsonPath = Path.Combine(_PathToFolderSetting, $"{ProjectCode}.json");

            if (!File.Exists(jsonPath))
                return null;

            var jsonString = File.ReadAllText(jsonPath);
            return JArray.Parse(jsonString);
        }


        /// <summary>
        /// Получение значения в виде строки
        /// </summary>
        /// <param name="nameObj">Имя объекта</param>
        /// <param name="nameField">Имя поля</param>
        /// <returns></returns>
        private string GetStringValueFromJson(string nameObj, string nameField)
        {
            string value = null;
            if (JsonArr == null)
                return value;

            foreach (var item in JsonArr)
            {
                if (item["name"].ToString() == nameObj)
                {
                    value = item[nameField].ToString();
                }
            }
            return value;
        }


        /// <summary>
        /// Получение значения в виде массива
        /// </summary>
        /// <param name="jarr">Массив с данными</param>
        /// <param name="nameObj">Имя объекта</param>
        /// <param name="nameField">Имя поля</param>
        /// <returns></returns>
        private List<string> GetArrayValueFromJson(string nameObj, string nameField)
        {
            List<string> values = null;
            if (JsonArr == null)
                return values;

            foreach (var item in JsonArr)
            {
                if (item["name"].ToString() == nameObj)
                {
                    values = new List<string>();
                    foreach (var element in item[nameField])
                        values.Add(element.ToString());
                }
            }
            return values;
        }


        /// <summary>
        /// Получение данных из имени модели и схемы
        /// </summary>
        /// <param name="modelName">Имя текущей модели</param>
        /// <param name="jFieldName">Имя json поля</param>
        /// <returns></returns>
        private string GetDataFromScheme(string modelName, string jObjName, string jFieldName)
        {
            string scheme = GetStringValueFromJson(jObjName, "scheme");
            string value = GetPartFromScheme(modelName, scheme, jFieldName);

            return value;
        }


        /// <summary>
        /// Получение данных из имени модели по схеме
        /// </summary>
        /// <param name="modelName">Имя модели</param>
        /// <param name="scheme">Схема наименования</param>
        /// <param name="part">Искомая часть</param>
        /// <returns></returns>
        private string GetPartFromScheme(string modelName, string scheme, string part)
        {
            string value = null;
            char fieldSeparator = '_';

            if (JsonArr == null)
                return value;
            
            // Получение сепаратора с файла с настройками
            fieldSeparator = (GetStringValueFromJson("projectCode", "field_separator")?.ToCharArray())[0];


            // Проверка на наличие символа сепаратора и флага для схемы
            if (!modelName.Contains(fieldSeparator) || !scheme.Contains(part))
                return value;

            int partIndex = -1;
            var partArray = scheme.Split('_');

            for (int i = 0; i < partArray.Length; i++)
            {
                if (partArray[i] == part)
                {
                    partIndex = i;
                    break;
                }
            }

            if (partIndex != -1)
                value = modelName.Split(fieldSeparator)[partIndex];

            return value;
        }
        #endregion
    }
}
