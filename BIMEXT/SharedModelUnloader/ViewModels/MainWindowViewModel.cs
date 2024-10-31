using Autodesk.Revit.UI;
using SharedModelUnloader.ViewModels.Base;
using System;


namespace SharedModelUnloader.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        public string UserName { get; }
        public UIApplication uiApp { get; }

        public MainWindowViewModel(UIApplication uiApp)
        {
            UserName = Environment.UserName ?? throw new Exception("Ошибка при получении имени пользователя");
            this.uiApp = uiApp;
        }
    }
}