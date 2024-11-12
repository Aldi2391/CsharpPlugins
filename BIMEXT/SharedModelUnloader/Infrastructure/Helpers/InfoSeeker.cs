using Autodesk.Revit.DB;
using SharedModelUnloader.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace SharedModelUnloader.Infrastructure.Helpers
{
    internal class InfoSeeker
    {
        /// <summary>
        /// Получение имени пользователя
        /// </summary>
        /// <returns></returns>
        public static string GetUserName()
        {
            try
            {
                return Environment.UserName;
            }
            catch
            {
                return "Ошибка";
            }
        }


        /// <summary>
        /// Получение кода\шифра проекта
        /// </summary>
        /// <param name="document">Открытый документ Revit</param>
        /// <returns></returns>
        public static string GetProjectCode(Document document)
        {
            string projectTitle = document.Title;
            if (!projectTitle.Contains("_"))
                return "Ошибка";
            return projectTitle.Split('_')[0];
        }


        /// <summary>
        /// Чтение путей к моделям из txt файла
        /// </summary>
        /// <param name="settings">Индивидуальные настройки проекта</param>
        /// <returns></returns>
        public static List<string> ReadFileToList(ProjectSettings settings)
        {
            // Выходные данные
            var outData = new List<string>();

            // Получение данных с настроек
            string filePath = settings.PathToRevitServerModelsInfo;
            string projName = settings.ProjectName?.ToLower();
            List<string> ignoreFolders = settings.IgnoreFolderRules?.Select(f => f.ToLower()).ToList();
            
            // Проверка наличия значения у свойства и доступа к файлу
            if (filePath == null || !File.Exists(filePath))
                return outData;
            
            // Проверка наличия имени проекта
            if (projName is null)
                return outData;

            // Чтение данных с файла
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                // Проверка строк
                string trimmedLine = line.Trim();
                
                if (!string.IsNullOrEmpty(trimmedLine))
                {
                    bool flagToAdd = true;
                    string lowerLine = trimmedLine.ToLower();

                    if (lowerLine.Contains(projName))
                    {
                        if (ignoreFolders != null && ignoreFolders.Count > 0)
                        {
                            foreach (string folder in ignoreFolders)
                            {
                                if (lowerLine.Contains(folder))
                                {
                                    flagToAdd = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        flagToAdd = false;
                    }

                    if (flagToAdd)
                        outData.Add(line);
                }
            }
            return outData;
        }
    }
}
