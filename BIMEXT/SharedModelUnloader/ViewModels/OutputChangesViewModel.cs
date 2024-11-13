using SharedModelUnloader.Infrastructure.Commands;
using SharedModelUnloader.Models;
using SharedModelUnloader.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SharedModelUnloader.Infrastructure.Helpers;
using System.Collections.Generic;
using SQLitePCL;
using System.Windows;
using System.Linq;

namespace SharedModelUnloader.ViewModels
{
    internal class OutputChangesViewModel : ViewModel
    {
        #region Свойства
        private ObservableCollection<OutputModel> _OutputModels;
        private string _ResultMessage;

        public LoadingWindowsViewModel LoadingSettings { get; }
        
        public ObservableCollection<OutputModel> OutputModels 
        {
            get => _OutputModels;
            set => Set(ref _OutputModels, value);
        }

        public string ResultMessage
        {
            get => _ResultMessage;
            set => Set(ref _ResultMessage, value);
        }

        #endregion


        #region Комманды
        public ICommand BackCommand { get; }
        private bool CanBackCommandExecute(object parameter) => true;
        private void OnBackCommandExecuted(object parameter)
        {
            Window currentWindow = (Window)parameter;
            currentWindow.Close();
            var MainWindow = new MainWindow()
            {
                    DataContext = new MainWindowViewModel(LoadingSettings)
            };
            MainWindow.ShowDialog();
        }


        public ICommand SelectAllCommand { get; }
        private bool CanSelectAllCommandExecute(object parameter)
        {
            if(OutputModels.Count > 0)
                return true;
            return false;
        }
        private void OnSelectAllCommandExecuted(object parameter)
        {
            foreach(var model in OutputModels)
                model.IsSelected = true;
        }


        public ICommand ResetCommand { get; }
        private bool CanResetCommandExecute(object parameter)
        {
            if (OutputModels.Count > 0)
                return true;
            return false;
        }
        private void OnResetCommandExecuted(object parameter)
        {
            foreach (var model in OutputModels)
                model.IsSelected = false;
        }


        public ICommand PublishCommand { get; }
        private bool CanPublishCommandExecute(object parameter)
        {
            return OutputModels.Any(model => model.IsSelected);
        }
        private void OnPublishCommandExecuted(object parameter)
        {

        }
        #endregion


        #region Конструктор
        public OutputChangesViewModel(LoadingWindowsViewModel loadingSettings)
        {
            Batteries_V2.Init();
            // Инициализация свойств
            this.LoadingSettings = loadingSettings;
            this.OutputModels = GetOutputModels();

            // Инициализация комманд
            this.BackCommand = new LambdaCommand(OnBackCommandExecuted, CanBackCommandExecute);
            this.SelectAllCommand = new LambdaCommand(OnSelectAllCommandExecuted, CanSelectAllCommandExecute);
            this.ResetCommand = new LambdaCommand(OnResetCommandExecuted, CanResetCommandExecute);
            this.PublishCommand = new LambdaCommand(OnPublishCommandExecuted, CanPublishCommandExecute);
        }
        #endregion


        #region Вспомогательные функции
        private ObservableCollection<OutputModel> GetOutputModels()
        {
            ObservableCollection<OutputModel> outData = new ObservableCollection<OutputModel>();

            List<OutputModel> modelsFromRS = OutputChangesWorker.GetOutputModelsFromFile(
                LoadingSettings.ProjectSettings, 
                LoadingSettings.UserName
            );

            OutputChangesWorker.UpdateOutputModels(modelsFromRS, LoadingSettings.ProjectSettings.PathToDB);

            foreach (var model in modelsFromRS)
            {
                outData.Add(model);
            }
            return outData;
        }
        #endregion
    }
}