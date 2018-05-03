using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ImageServiceGUI.Annotations;
using Microsoft.Practices.Prism.Commands;

namespace ImageServiceGUI.ViewModels
{
    class SettingsViewModel : INotifyPropertyChanged
    {
        public SettingsViewModel()
        {
            Debug.WriteLine("SettingsViewModel c'tor");
            LogName = "[Log name here]";
            SourceName = "[Source Name Here]";
            OutputDirectory = "[Output Directory Here]";
            ThumbnailSize = 120;
            DirectoryHandlers = new ObservableCollection<string>()
            {
                "One",
                "Two",
                "Three"
            };

            SubmitRemove = new DelegateCommand<object>(OnRemove, CanRemove);
            PropertyChanged += RemoveSelectedHandlerCommand;
        }

        public ObservableCollection<string> DirectoryHandlers { get; }
        public string LogName { get; set; }
        public string SourceName { get; set; }
        public string OutputDirectory { get; set; }
        public int ThumbnailSize { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand SubmitRemove { get; private set; }
        private string _selectedDirectoryHandler;

        public string SelectedDirectoryHandler
        {
            get => _selectedDirectoryHandler;
            set
            {
                _selectedDirectoryHandler = value;
                Debug.WriteLine(_selectedDirectoryHandler);
                NotifyPropertyChanged("SelectedDirectoryHandler");
            }
        }

        public void NotifyPropertyChanged(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        /// <summary>
        /// Raises when a handler is removed from the Service.
        /// </summary>
        public void OnHandlerRemoved()
        {
            // Remove handler from view
            throw new NotImplementedException();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RemoveSelectedHandlerCommand(object sender, PropertyChangedEventArgs args)
        {
            Debug.WriteLine("Removing...");
            DelegateCommand<object> command = SubmitRemove as DelegateCommand<object>;
            command?.RaiseCanExecuteChanged();
        }

        private void OnRemove(object obj)
        {
            Debug.WriteLine("In OnButtonClicked");
            DirectoryHandlers.Remove(SelectedDirectoryHandler);
            SelectedDirectoryHandler = null;
        }

        private bool CanRemove(object obj)
        {
            Debug.WriteLine("In CanRemove");
            return !string.IsNullOrEmpty(SelectedDirectoryHandler);
        }
    }
}