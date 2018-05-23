using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
        private const string WaitingForConnection = "Waiting for connection...";
        public ObservableCollection<string> DirectoryHandlers { get; }
        private string _outputDirectory = WaitingForConnection;
        private string _logName = WaitingForConnection;
        private string _sourceName = WaitingForConnection;
        private int _thumbnailSize;

        public SettingsViewModel()
        {
            _uiDispatcher = Application.Current.Dispatcher;
            Debug.WriteLine("SettingsViewModel c'tor");

            DirectoryHandlers = new ObservableCollection<string>();
            _uiDispatcher = Application.Current.Dispatcher;
            BackgroundColor = new SolidColorBrush(Colors.SlateGray);
            OurTcpClientSingleton.Instance.ConnectedToService += OnConnectedToService;
            OurTcpClientSingleton.Instance.DirectoryHandlerRemoved += OnDirectoryHandlerSuccessfulyRemoved;
            SubmitRemove = new DelegateCommand<object>(OnRemove, CanRemove);
            PropertyChanged += RemoveSelectedHandlerCommand;

            OurTcpClientSingleton.Instance.ConfigurationReceived += OnConfigurationReceived;
        }

        public SolidColorBrush BackgroundColor { get; set; }

        private void OnConnectedToService(object sender, EventArgs e)
        {
            Debug.WriteLine("In SettingsViewModel->OnConnectedToService");
            _uiDispatcher.BeginInvoke(new Action(() =>
            {
                BackgroundColor = new SolidColorBrush(Colors.DarkCyan);
                NotifyPropertyChanged("BackgroundColor");
            }));
        }

        public string OutputDirectory
        {
            get => _outputDirectory;
            set
            {
                _outputDirectory = value;
                NotifyPropertyChanged("OutputDirectory");
            }
        }

        public int ThumbnailSize
        {
            get => _thumbnailSize;
            set
            {
                _thumbnailSize = value;
                NotifyPropertyChanged("ThumbnailSize");
            }
        }

        public string LogName
        {
            get => _logName;
            set
            {
                _logName = value;
                NotifyPropertyChanged("LogName");
            }
        }

        public string SourceName
        {
            get => _sourceName;
            set
            {
                _sourceName = value;
                NotifyPropertyChanged("SourceName");
            }
        }

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
            _uiDispatcher.BeginInvoke(new Action(() => { SetSettings(SettingsInfo.FromJson(eventArgs.Args)); }));
        }

        private void SetSettings(SettingsInfo settingsInfo)
        {
            LogName = settingsInfo.LogName;
            SourceName = settingsInfo.SourceName;
            OutputDirectory = settingsInfo.OutputDirectory;
            ThumbnailSize = settingsInfo.ThumbnailSize;

            // string[] handlers = settingsInfo.HandledDir.Split(';');
            string[] handlers = settingsInfo.HandledDir.ToObject<string[]>();

            foreach (string handler in handlers)
            {
                DirectoryHandlers.Add(handler);
            }
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
            string command = (int) CommandEnum.CloseDirectoryHandlerCommand + "|" + SelectedDirectoryHandler;
            OurTcpClientSingleton.Instance.Writer.Write(command);
        }

        private bool CanRemove(object obj)
        {
            Debug.WriteLine("In CanRemove");
            return !string.IsNullOrEmpty(SelectedDirectoryHandler);
        }
    }
}