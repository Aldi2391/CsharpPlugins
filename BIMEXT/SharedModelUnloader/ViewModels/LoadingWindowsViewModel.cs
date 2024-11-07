using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SharedModelUnloader.Infrastructure.Helpers;
using SharedModelUnloader.Models;
using SharedModelUnloader.ViewModels.Base;

namespace SharedModelUnloader.ViewModels
{
    internal class LoadingWindowsViewModel : ViewModel
    {
        #region Свойства 
        private string _ModelName;
        private string _UserName;
        private string _ProjectCode;
        private ProjectSettings _ProjectSettings;
        private string _Status;

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

        public ProjectSettings ProjectSettings
        {
            get => _ProjectSettings;
            set => Set(ref _ProjectSettings, value);
        }

        public string Status
        {
            get => _Status;
            set => Set(ref _Status, value);
        }

        public UIApplication UIApplication { get; }

        public Application Application { get; }

        public Document Document { get; }
        
        #endregion

        public LoadingWindowsViewModel(UIApplication uiApp)
        {
            // Поля для работы с приложением Revit
            this.UIApplication = uiApp;
            this.Application = uiApp.Application;
            this.Document = uiApp.ActiveUIDocument.Document;

            // Поля для работы с плагином
            this.ModelName = this.Document.Title;
            this.UserName = ProjectInfoSeeker.GetUserName();
            this.ProjectCode = ProjectInfoSeeker.GetProjectCode(Document);
            this.ProjectSettings = new ProjectSettings(ModelName, ProjectCode);
        }

    }
}
