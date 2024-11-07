using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModelUnloader.Infrastructure.Helpers
{
    internal class ProjectInfoSeeker
    {
        public static string GetUserName()
        {
            try
            {
                return Environment.UserName;
            }
            catch
            {
                return "-";
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
                return "-";
            return projectTitle.Split('_')[0];
        }
    }
}
