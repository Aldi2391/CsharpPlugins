using SharedModelUnloader.Infrastructure.Commands;
using SharedModelUnloader.ViewModels.Base;
using System.Windows.Input;

namespace SharedModelUnloader.ViewModels
{
    internal class OutputChangesViewModel : ViewModel
    {
        #region Свойства
        public LoadingWindowsViewModel LoadingSettings { get; }

        #endregion


        #region Комманды
        public ICommand BackCommand { get; }
        private bool CanBackCommandExecute(object parameter) => true;
        private void OnBackCommandExecuted(object parameter)
        {

        }


        public ICommand SelectAllCommand { get; }
        private bool CanSelectAllCommandExecute(object parameter)
        {
            return true;
        }
        private void OnSelectAllCommandExecuted(object parameter)
        {

        }


        public ICommand ResetCommand { get; }
        private bool CanResetCommandExecute(object parameter)
        {
            return true;
        }
        private void OnResetCommandExecuted(object parameter)
        {

        }


        public ICommand PublishCommand { get; }
        private bool CanPublishCommandExecute(object parameter)
        {
            return true;
        }
        private void OnPublishCommandExecuted(object parameter)
        {

        }
        #endregion


        #region Конструктор
        public OutputChangesViewModel(LoadingWindowsViewModel loadingSettings)
        {
            // Инициализация свойств
            this.LoadingSettings = loadingSettings;

            // Инициализация комманд
            this.BackCommand = new LambdaCommand(OnBackCommandExecuted, CanBackCommandExecute);
            this.SelectAllCommand = new LambdaCommand(OnSelectAllCommandExecuted, CanSelectAllCommandExecute);
            this.ResetCommand = new LambdaCommand(OnResetCommandExecuted, CanResetCommandExecute);
            this.PublishCommand = new LambdaCommand(OnPublishCommandExecuted, CanPublishCommandExecute);
        }
        #endregion
    }
}
