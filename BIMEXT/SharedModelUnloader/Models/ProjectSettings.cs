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
        // Пути к папке с настройками и списку координаторов
        private const string PATHTOFOLDERSETTINGS = @"G:\BIMEXT\WIP\RUSSIAN\02_REVIT\02_Настройки\04_Плагины\00_Зона Shared";

        private JArray jsonArr;

        public string ProjectCode { get; }

        public string Priority { get; }

        public string HomeNumber { get; }

        public string Chapter { get; }

        public string Phase { get; }

        public string RevitServer { get; }

        public string PathToSave { get; }

        public string PathToDB { get; }

        public string PathToRevitServerFolder { get; }


        public ProjectSettings(string modelName, string projectCode)
        {
            // Получение файла с настройками
            ProjectCode = projectCode;
            jsonArr = GetJsonArray();

            // Парсинг данных
            Priority = GetDataFromScheme(modelName, "projectCode", "Очередь");
            HomeNumber = GetDataFromScheme(modelName, "projectCode", "НомерДома");
            Chapter = GetDataFromScheme(modelName, "projectCode", "ШифрРаздела");
            Phase = GetDataFromScheme(modelName, "projectCode", "СтадияПроектирования");
            RevitServer = GetStringValueFromJson("paths", "revit_server");
            PathToSave = GetStringValueFromJson("paths", "path_to_save_models");
            PathToDB = GetStringValueFromJson("paths", "path_to_db");
            PathToRevitServerFolder = GetStringValueFromJson("paths", "path_to_revit_server");
        }


        #region Служебные функции для получения данных

        /// <summary>
        /// Получение объекта JArray 
        /// </summary>
        /// <returns>Объект JArray</returns>
        private JArray GetJsonArray()
        {
            if(ProjectCode == null)
                return null;
            
            var jsonPath = Path.Combine(PATHTOFOLDERSETTINGS, $"{ProjectCode}.json");

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
            if (jsonArr == null)
                return value;

            foreach (var item in jsonArr)
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
            if (jsonArr == null)
                return values;

            foreach (var item in jsonArr)
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

            if (jsonArr == null)
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
