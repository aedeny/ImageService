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
        private const string WaitingForConnection = "Waiting for connection...";
        private readonly Dispatcher _uiDispatcher;
        private string _logName;
        private string _outputDirectory;
        private string _selectedDirectoryHandler;
        private string _sourceName;
        private int _thumbnailSize;

        public SettingsViewModel()
        {
            Debug.WriteLine("SettingsViewModel c'tor");

            // Initializes parameters
            _logName = WaitingForConnection;
            _sourceName = WaitingForConnection;
            _outputDirectory = WaitingForConnection;
            BackgroundColor = new SolidColorBrush(Colors.SlateGray);
            _uiDispatcher = Application.Current.Dispatcher;

            DirectoryHandlers = new ObservableCollection<string>
            {
                @"C:\Users\ventu\Desktop\Image\Handler",
                "All",
                "GameBoys!!!"
            };

            OurTcpClientSingleton.Instance.ConnectedToService += OnConnectedToService;
            OurTcpClientSingleton.Instance.DirectoryHandlerRemoved += OnDirectoryHandlerSuccessfulyRemoved;
            SubmitRemove = new DelegateCommand<object>(OnRemove, CanRemove);
            PropertyChanged += RemoveSelectedHandlerCommand;

            OurTcpClientSingleton.Instance.ConfigurationReceived += OnConfigurationReceived;
        }

        public ObservableCollection<string> DirectoryHandlers { get; }

        public string OutputDirectory
        {
            get => m_outputDirectory;
            set
            {
                m_outputDirectory = value;
                NotifyPropertyChanged("OutputDirectory");
            }
        }

        public int ThumbnailSize
        {
            get => m_thumbnailSize;
            set
            {
                m_thumbnailSize = value;
                NotifyPropertyChanged("ThumbnailSize");
            }
        }

        public string LogName
        {
            get => m_logName;
            set
            {
                m_logName = value;
                NotifyPropertyChanged("LogName");
            }
        }

        public SolidColorBrush BackgroundColor { get; set; }

        public string SourceName
        {
            get => m_sourceName;
            set
            {
                m_sourceName = value;
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

        public void OnConnectedToService(object sender, EventArgs eventArgs)
        {
            Debug.WriteLine("In SettingsViewModel->OnConnectedToService");
            _uiDispatcher.BeginInvoke(new Action(() =>
            {
                BackgroundColor = new SolidColorBrush(Colors.DarkCyan);
                NotifyPropertyChanged("BackgroundColor");
            }));
        }

        private void SetSettings(SettingsInfo settingsInfo)
        {
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