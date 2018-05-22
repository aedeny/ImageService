using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Infrastructure.Enums;
using Infrastructure.Event;
using Microsoft.Practices.Prism.Commands;

namespace ImageServiceGUI.ViewModels
{
    internal class SettingsViewModel : INotifyPropertyChanged
    {
        private readonly Dispatcher _uiDispatcher;

        private string _selectedDirectoryHandler;

        public SettingsViewModel()
        {
            _uiDispatcher = Application.Current.Dispatcher;
            Debug.WriteLine("SettingsViewModel c'tor");

            LogName = "[Log name here]";
            SourceName = "[Source Name Here]";
            OutputDirectory = "[Output Directory Here]";
            ThumbnailSize = 120;
            DirectoryHandlers = new ObservableCollection<string>
            {
                @"C:\Users\ventu\Desktop\Image\Handler",
                "All",
                "GameBoys!!!"
            };

            OurTcpClientSingleton.Instance.DirectoryHandlerRemoved += OnDirectoryHandlerSuccessfulyRemoved;
            SubmitRemove = new DelegateCommand<object>(OnRemove, CanRemove);
            PropertyChanged += RemoveSelectedHandlerCommand;

            OurTcpClientSingleton.Instance.ConfigurationReceived += OnConfigurationReceived;
        }

        public ObservableCollection<string> DirectoryHandlers { get; }

        public string LogName { get; set; }

        public string SourceName { get; set; }

        public string OutputDirectory { get; set; }

        public int ThumbnailSize { get; set; }

        public ICommand SubmitRemove { get; }

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

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        ///     Raises when configuration was successfuly received.
        /// </summary>
        public void OnConfigurationReceived(object sender, ConfigurationReceivedEventArgs eventArgs)
        {
            Debug.WriteLine("In OnConfigurationReceived");
            _uiDispatcher.BeginInvoke(new Action(() =>
            {
                SetSettings(SettingsInfo.FromJson(eventArgs.Args));
            }));
        }

        private void SetSettings(SettingsInfo settingsInfo)
        {
            Debug.WriteLine("In SetSettings");
            Debug.WriteLine("In SetSettings");
            Debug.WriteLine("In SetSettings");
            LogName = settingsInfo.LogName;
            SourceName = settingsInfo.SourceName;
            OutputDirectory = settingsInfo.OutputDirectory;
            ThumbnailSize = settingsInfo.ThumbnailSize;
        }

        /// <summary>
        ///     Raises when a handler is successfuly removed from the Service.
        /// </summary>
        public void OnDirectoryHandlerSuccessfulyRemoved(object sender, DirectoryHandlerClosedEventArgs eventArgs)
        {
            Debug.WriteLine("In OnDirectoryHandlerSuccessfulyRemoved");
            Debug.WriteLine(eventArgs.DirectoryPath);
            _uiDispatcher.BeginInvoke(new Action(() =>
            {
                DirectoryHandlers.Remove(SelectedDirectoryHandler);
                SelectedDirectoryHandler = null;
            }));
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
        ///     Asks Service to remove selected directory handler.
        ///     Note: This method doesn't remove the dir handler from the ListBox.
        /// </summary>
        /// <param name="obj"></param>
        private void OnRemove(object obj)
        {
            Debug.WriteLine("In OnRemove");
            string command = (int) CommandEnum.CloseDirectoryHandlerCommand + ";" + SelectedDirectoryHandler;
            OurTcpClientSingleton.Instance.Writer.Write(command);
        }

        private bool CanRemove(object obj)
        {
            Debug.WriteLine("In CanRemove");
            return !string.IsNullOrEmpty(SelectedDirectoryHandler);
        }   
    }
}