using SharedModelUnloader.ViewModels.Base;
using System;


namespace SharedModelUnloader.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        public string UserName { get; }
        

        public MainWindowViewModel()
        {
            UserName = Environment.UserName ?? throw new Exception("Ошибка при получении имени пользователя");
        }
    }
}