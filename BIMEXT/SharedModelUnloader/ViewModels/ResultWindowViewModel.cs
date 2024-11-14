using SharedModelUnloader.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using SharedModelUnloader.Infrastructure.Commands;

namespace SharedModelUnloader.ViewModels
{
    internal class ResultWindowViewModel : ViewModel
    {
        #region Свойства
        private string _Message;

        public string Message
        {
            get => _Message;
            set => Set(ref _Message, value);
        }
        #endregion

        #region Команды
        public ICommand CloseCommand { get; }
        private bool CanCloseCommandExecute(object parameter) => true;
        private void OnCloseCommandExecuted(object parameter)
        {
            Window currentWindow = (Window)parameter;
            currentWindow.Close();
        }
        #endregion

        #region Конструктор
        public ResultWindowViewModel(string message)
        {
            Message = message;
            CloseCommand = new LambdaCommand(OnCloseCommandExecuted, CanCloseCommandExecute);
        }
        #endregion

    }
}
