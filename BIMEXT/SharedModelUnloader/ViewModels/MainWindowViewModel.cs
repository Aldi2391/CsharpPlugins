using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SharedModelUnloader.Infrastructure.Commands;
using SharedModelUnloader.Infrastructure.Helpers;
using SharedModelUnloader.ViewModels.Base;
using SharedModelUnloader.Views.Windows;
using System;
using System.Windows;
using System.Windows.Input;


namespace SharedModelUnloader.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Свойства
        
        public LoadingWindowsViewModel LoadingSettings { get; }

        public ICommand OutputChangesCommand { get; }

        public ICommand InputChangesCommand { get; }

        public ICommand ChronologyCommand { get; }

        public ICommand TransitionToWorkingModelsCommand { get; }


        #endregion

        #region Комманды

        private bool CanOutputChangesCommandExecute(object parameter) => true;

        private void OnOutputChangesCommandExecuted(object parameter)
        {
            // Закрытие стартового окна
            Window currentWindow = (Window)parameter;
            currentWindow.Close();

            OutputChangesWindow outputChangesWindow = new OutputChangesWindow()
            {
                DataContext = new OutputChangesViewModel(LoadingSettings)
            };
            outputChangesWindow.ShowDialog();

        }

        private bool CanInputChangesCommandExecute(object parameter) => true;

        private void OnInputChangesCommandExecuted(object parameter)
        {

        }

        private bool CanChronologyCommandExecute(object parameter) => true;

        private void OnChronologyCommandExecuted(object parameter)
        {

        }

        private bool CanTransitionToWorkingModelsCommandExecute(object parameter) => true;

        private void OnTransitionToWorkingModelsCommandExecuted(object parameter)
        {

        }

        #endregion

        public MainWindowViewModel(LoadingWindowsViewModel settings)
        {
            // Инициализация свойств
            this.LoadingSettings = settings;

            // Инициализация комманд
            OutputChangesCommand = new LambdaCommand(OnOutputChangesCommandExecuted, CanOutputChangesCommandExecute);
            InputChangesCommand = new LambdaCommand(OnInputChangesCommandExecuted, CanInputChangesCommandExecute);
            ChronologyCommand = new LambdaCommand(OnChronologyCommandExecuted, CanChronologyCommandExecute);
            TransitionToWorkingModelsCommand = new LambdaCommand(OnTransitionToWorkingModelsCommandExecuted, CanTransitionToWorkingModelsCommandExecute);
        }
    }
}