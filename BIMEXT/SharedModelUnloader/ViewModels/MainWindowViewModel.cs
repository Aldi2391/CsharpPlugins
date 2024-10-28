using SharedModelUnloader.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

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