using SharedModelUnloader.Infrastructure.Commands;
using SharedModelUnloader.Infrastructure.Helpers;
using SharedModelUnloader.Models;
using SharedModelUnloader.ViewModels.Base;
using SharedModelUnloader.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SharedModelUnloader.ViewModels
{
    internal class PublishWindowViewModel : ViewModel
    {
        #region Свойства
        private string _Author;
        private string _Description;

        public string Author
        {
            get => _Author;
            set => Set(ref _Author, value);
        }

        public string Description
        {
            get => _Description;
            set => Set(ref _Description, value);
        }

        public ObservableCollection<OutputModel> PublishedModels { get; }

        public LoadingWindowsViewModel LoadingSettings { get; }

        public string ResultMessage { get; set; }
        #endregion

        #region Команды
        public ICommand PublishCommand { get; }

        private bool CanPublishCommandExecute(object parameter)
        {
            if(string.IsNullOrEmpty(Author) || string.IsNullOrEmpty(Description))
                return false;
            return true;
        }

        private void OnPublishCommandExecuted(object parameter)
        {
            // Выгрузка моделей
            foreach(var model in PublishedModels)
            {
                // Обновление данных перед записью
                model.Author = Author;
                model.AuthorEmail = LoadingSettings.UserName + "@samolet.ru";
                model.Description = Description;
                model.Version++;

                // Открываем модель в фоновом режиме
                var modelCleaner = new ModelCleaner(LoadingSettings);
                var document = modelCleaner.OpenRevitModel(model);
                
                if (document != null)
                {
                    // Очистка модели
                    bool clearingResult = modelCleaner.ClearModelWithoutRules(document);
                    if (clearingResult)
                    {
                        // Сохранение модели
                        bool savingResult = modelCleaner.CloseAndSaveModel(model, document);
                        if (savingResult)
                        {
                            // Сохранение данных в БД
                            DataBaseHelper dbHelper = new DataBaseHelper(LoadingSettings.ProjectSettings.PathToDB);
                            bool updateDBFlag = dbHelper.WriteChangesToDB(model);
                            if (updateDBFlag)
                            {
                                // Сохранение миниотчета
                                modelCleaner.SaveReport(model);
                            }
                            else
                            {
                                // Добавить логгирование
                            }
                        }
                        else
                        {
                            ResultMessage += $"Не удалось сохранить модель {model.Name}\n";
                        }
                    }
                    else
                    {
                        ResultMessage += $"Не удалось очистить модель {model.Name}\n";
                    }
                }
                else
                {
                    ResultMessage += $"Не удалось открыть модель {model.Name}\n";
                }
            }

            // Финальное сообщение
            if (string.IsNullOrEmpty(ResultMessage))
                ResultMessage = "Выгрузка прошла успешно!!!";


            // Закрытие моделей
            Window currentWindow = (Window)parameter;
            currentWindow.Close();

            // Открытие окна с результатами
            Window resultWindow = new ResultWindow()
                {
                    DataContext = new ResultWindowViewModel(ResultMessage)
                };
            resultWindow.ShowDialog();
        }
        #endregion

        #region Конструктор
        public PublishWindowViewModel(LoadingWindowsViewModel loadingSettings, ObservableCollection<OutputModel> publishedModels)
        {
            this.LoadingSettings = loadingSettings;
            this.PublishedModels = publishedModels;
            this.PublishCommand = new LambdaCommand(OnPublishCommandExecuted, CanPublishCommandExecute);
            this.ResultMessage = "";
        }
        #endregion
    }
}
