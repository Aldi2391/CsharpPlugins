using SharedModelUnloader.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModelUnloader.Models
{
    internal class OutputModel : ViewModel
    {
        #region Свойства
        private bool _IsSelected;
        private string _Name;
        private int _Version;
        private string _Chapter;
        private string _Description;
        private string _Author;
        private string _AuthorEmail;
        private string _Date;

        public bool IsSelected
        {
            get => _IsSelected;
            set => Set(ref _IsSelected, value);
        }

        public string Name
        {
            get => _Name;
            set => Set(ref _Name, value);
        }

        public int Version
        {
            get => _Version;
            set => Set(ref _Version, value);
        }

        public string Chapter
        {
            get => _Chapter;
            set => Set(ref _Chapter, value);
        }

        public string Description
        {
            get => _Description;
            set => Set(ref _Description, value);
        }
        
        public string Author
        {
            get => _Author;
            set => Set(ref _Author, value);
        }

        public string AuthorEmail
        {
            get => _AuthorEmail;
            set => Set(ref _AuthorEmail, value);
        }

        public string Date
        {
            get => _Date;
            set => Set(ref _Date, value);
        }

        public string ProjectCode { get; }
        
        public string Priority { get; }
        
        public string HomeNumber { get; }
        
        public string Phase { get; }

        public string PathToFolder {  get; }
        #endregion

        #region Конструктор
        public OutputModel(string name, int version, string description, string date, ProjectSettings settings)
        {
            // Получение данных из аргументов
            this.IsSelected = false;
            this.Name = name;
            this.Version = version;
            this.Description = description;
            this.Date = date;
            
            // Получение данных из настроек
            this.ProjectCode = settings.ProjectCode;
            this.Priority = settings.Priority;
            this.HomeNumber = settings.HomeNumber;
            this.Chapter = settings.Chapter;
            this.Phase = settings.Phase;
            this.PathToFolder = settings.PathToSaveModels;
        }
        #endregion
    }
}
