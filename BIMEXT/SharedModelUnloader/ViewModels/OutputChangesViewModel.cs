using SharedModelUnloader.Infrastructure.Commands;
using SharedModelUnloader.Models;
using SharedModelUnloader.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SharedModelUnloader.Infrastructure.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharedModelUnloader.ViewModels
{
    internal class OutputChangesViewModel : ViewModel
    {
        #region Свойства
        private ObservableCollection<OutputModel> _OutputModels;

        public LoadingWindowsViewModel LoadingSettings { get; }
        
        public ObservableCollection<OutputModel> OutputModels 
        {
            get => _OutputModels;
            set => Set(ref _OutputModels, value);
        }
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

            // Получение моделей
            InizializeModelsAsync();
        }
        #endregion

        #region Вспомогательные функции

        private async Task InizializeModelsAsync()
        {
            try
            {
                this.OutputModels = await GetOutputModels();
            }
            catch { }
        }

        private async Task<ObservableCollection<OutputModel>> GetOutputModels()
        {
            ObservableCollection<OutputModel> outData = new ObservableCollection<OutputModel>();

            List<OutputModel> modelsFromRS = OutputChangesWorker.GetOutputModelsFromFile(
                LoadingSettings.ProjectSettings, 
                LoadingSettings.UserName
            );

            await OutputChangesWorker.UpdateOutputModels(modelsFromRS, LoadingSettings.ProjectSettings.PathToDB);

            foreach (var model in modelsFromRS)
            {
                outData.Add(model);
            }
            return outData;
        }
        #endregion
    }
}
