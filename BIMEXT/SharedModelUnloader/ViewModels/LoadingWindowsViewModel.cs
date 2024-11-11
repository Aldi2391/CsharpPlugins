using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SharedModelUnloader.Infrastructure.Commands;
using SharedModelUnloader.Infrastructure.Helpers;
using SharedModelUnloader.Models;
using SharedModelUnloader.ViewModels.Base;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SharedModelUnloader.ViewModels
{
    internal class LoadingWindowsViewModel : ViewModel
    {

        #region Свойства 
        private readonly string _PathToFolderSettings = @"G:\BIMEXT\WIP\RUSSIAN\02_REVIT\04_Плагины\99_ТЗ\Зона Shared\Настройки";
        private string _ModelName;
        private string _UserName;
        private string _ProjectCode;
        private string _ProjectSettingsStatus;
        private string _RevitServerModelsStatus;
        private string _DataBaseStatus;
        private string _FinalStatus;
        private bool _FinalStatusBoolean;
        private string _ContinuationStatus;

        public string ModelName
        {
            get => _ModelName;
            set => Set(ref _ModelName, value);
        }

        public string UserName
        {
            get => _UserName; 
            set => Set(ref _UserName, value);
        }

        public string ProjectCode
        {
            get => _ProjectCode;
            set => Set(ref _ProjectCode, value);
        }

        public string ProjectSettingsStatus
        {
            get => _ProjectSettingsStatus;
            set => Set(ref _ProjectSettingsStatus, value);
        }

        public string RevitServerModelsStatus
        {
            get => _RevitServerModelsStatus;
            set => Set(ref _RevitServerModelsStatus, value);
        }

        public string DataBaseStatus
        {
            get => _DataBaseStatus;
            set => Set(ref _DataBaseStatus, value);
        }

        public string FinalStatus
        {
            get => _FinalStatus;
            set => Set(ref _FinalStatus, value);
        }

        public bool FinalStatusBoolean
        {
            get => _FinalStatusBoolean;
            set => Set(ref _FinalStatusBoolean, value);
        }

        public string ContinuationStatus
        {
            get => _ContinuationStatus;
            set => Set(ref _ContinuationStatus, value);
        }

        public UIApplication UIApplication { get; }

        public Autodesk.Revit.ApplicationServices.Application Application { get; }

        public Document Document { get; }

        public ProjectSettings ProjectSettings { get; }

        #endregion


        #region Команды

        public ICommand CloseLoadingWindowCommand {  get; }

        private bool CanCloseLoadingWindowCommandExecute(object parameter) => true;

        private void OnCloseLoadingWindowCommandExecuted(object parameter)
        {
            Window currentWindow = (Window)parameter;
            currentWindow.Close();
            if (FinalStatusBoolean)
            {
                var MainWindow = new MainWindow()
                {
                    DataContext = new MainWindowViewModel(this)
                };
                MainWindow.ShowDialog();
            }
        }

        #endregion

        public LoadingWindowsViewModel(UIApplication uiApp)
        {
            // Поля для работы с приложением Revit
            this.UIApplication = uiApp;
            this.Application = uiApp.Application;
            this.Document = uiApp.ActiveUIDocument.Document;

            // Поля для начала работы с плагином
            this.ModelName = this.Document.Title;
            this.UserName = InfoSeeker.GetUserName();
            this.ProjectCode = InfoSeeker.GetProjectCode(Document);
            this.ProjectSettings = new ProjectSettings(ModelName, ProjectCode, _PathToFolderSettings);

            // Поля для информирования пользователя
            this.ProjectSettingsStatus = GetProjectSettingsStatus();
            this.RevitServerModelsStatus = GetRevitServerModelsStatus();
            this.DataBaseStatus = GetDBStatus();
            this.FinalStatus = GetFinalStatus();
            this.FinalStatusBoolean = GetFinalStatusBoolean();
            this.ContinuationStatus = GetContinuationStatus();

            // Комманды
            CloseLoadingWindowCommand = new LambdaCommand(OnCloseLoadingWindowCommandExecuted, CanCloseLoadingWindowCommandExecute);
        }


        #region Служебные функции
        /// <summary>
        /// Получение статуса полноты настроек
        /// </summary>
        /// <returns></returns>
        private string GetProjectSettingsStatus()
        {
            bool jsonFlag = ProjectSettings.JsonArr is null ? false : true;
            bool revitServerPath = ProjectSettings.RevitServer is null ? false : true;
            bool pathToSaveModels = ProjectSettings.PathToSaveModels is null ? false : true;
            bool pathToDB = ProjectSettings.PathToDB is null ? false : true;
            bool pathToRevitServerFolder = ProjectSettings.RevitServerFolder is null ? false : true;
            bool pathToRevitServerModelsInfo = ProjectSettings.PathToRevitServerModelsInfo is null ? false : true;

            List<bool> allFlags = new List<bool>
            {
                jsonFlag,
                revitServerPath,
                pathToSaveModels, 
                pathToDB, 
                pathToRevitServerFolder,
                pathToRevitServerModelsInfo
            };

            if (allFlags.All(x => x))
            {
                return "Данные получены";
            }
            else
            {
                return "Ошибка";
            }
        }

        private string GetRevitServerModelsStatus()
        {
            if (File.Exists(ProjectSettings.PathToRevitServerModelsInfo))
                return "Данные получены";
            return "Ошибка";
        }

        private string GetDBStatus()
        {
            if (File.Exists(ProjectSettings.PathToDB))
                return "Доступ получен";
            return "Ошибка";
        }

        private string GetFinalStatus()
        {
            string result = string.Empty;
            
            if (ProjectCode == "Ошибка")
                result += "- Шифр проекта не получен. Проверьте имя модели!;\n";
            if (ProjectSettingsStatus == "Ошибка")
                result += "- Настройки проекта не получены или получены не полностью. Проверьте подключение к сетевому диску или обратитесь" +
                    "к bim-координатору проекта;\n";
            if (RevitServerModelsStatus == "Ошибка")
                result += "- Список моделей с RS не получены. Проверьте подключение к сетевому диску или обратитесь" +
                    "к bim-координатору проекта;\n";
            if (DataBaseStatus == "Ошибка")
                result += "- Доступ к базе данных не получен. Проверьте подключение к сетевому диску или обратитесь" +
                    "к bim-координатору проекта;\n";

            if (string.IsNullOrEmpty(result))
                result = "Все необходимые для работы данные получены.\nНажмите кнопку ПРОДОЛЖИТЬ";
            return result;
        }

        private bool GetFinalStatusBoolean()
        {
            if (FinalStatus == "Все необходимые для работы данные получены.\nНажмите кнопку ПРОДОЛЖИТЬ")
                return true;
            return false;
        }

        private string GetContinuationStatus()
        {
            if (FinalStatusBoolean)
                return "Продолжить";
            return "Закрыть";
        }
        #endregion
    }
}
