using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Infrastructure.Event;
using Microsoft.Practices.Prism.Commands;

namespace ImageServiceGUI.ViewModels
{
    class SettingsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> DirectoryHandlers { get; }

        public string LogName { get; set; }

        public string SourceName { get; set; }

        public string OutputDirectory { get; set; }

        public int ThumbnailSize { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand SubmitRemove { get; }

        private string _selectedDirectoryHandler;

        public SettingsViewModel()
        {
            Debug.WriteLine("SettingsViewModel c'tor");
            LogName = "[Log name here]";
            SourceName = "[Source Name Here]";
            OutputDirectory = "[Output Directory Here]";
            ThumbnailSize = 120;
            DirectoryHandlers = new ObservableCollection<string>()
            {
                @"C:\Users\edeny\Documents\ex01\handled_dir1",
                "All",
                "GameBoys!!!"
            };
            OurTcpClientSingleton.Instance.DirectoryHandlerRemoved += OnDirectoryHandlerSuccessfulyRemoved;
            SubmitRemove = new DelegateCommand<object>(OnRemove, CanRemove);
            PropertyChanged += RemoveSelectedHandlerCommand;
        }

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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        /// <summary>
        /// Raises when a handler is successfuly removed from the Service.
        /// </summary>
        public void OnDirectoryHandlerSuccessfulyRemoved(object sender, DirectoryHandlerClosedEventArgs eventArgs)
        {
            Debug.WriteLine(eventArgs.DirectoryPath);
            DirectoryHandlers.Remove(eventArgs.DirectoryPath); // TODO Fix bug here
            SelectedDirectoryHandler = null;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RemoveSelectedHandlerCommand(object sender, PropertyChangedEventArgs args)
        {
            DelegateCommand<object> command = SubmitRemove as DelegateCommand<object>;
            command?.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Asks Service to remove selected directory handler.
        /// Note: This method doesn't remove the dir handler from the ListBox.
        /// </summary>
        /// <param name="obj"></param>
        private void OnRemove(object obj)
        {
            Debug.WriteLine("In OnButtonClicked");
            OurTcpClientSingleton.Instance.RemoveHandler(SelectedDirectoryHandler);
        }

        private bool CanRemove(object obj)
        {
            Debug.WriteLine("In CanRemove");
            return !string.IsNullOrEmpty(SelectedDirectoryHandler);
        }
    }
}