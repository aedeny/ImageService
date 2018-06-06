using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Communication;
using Infrastructure;
using Infrastructure.Enums;
using Infrastructure.Event;

namespace ImageServiceGUI.Model
{
    internal class SettingsModel : ISettingsModel
    {
        private const string WaitingForConnection = "Waiting for connection";
        private readonly Dispatcher _uiDispatcher;
        private SolidColorBrush _backgroundColor;

        private string _logName = WaitingForConnection;
        private string _outputDirectory = WaitingForConnection;
        private string _sourceName = WaitingForConnection;
        private int _thumbnailSize;

        public SettingsModel(Dispatcher uiDispatcher)
        {
            _uiDispatcher = uiDispatcher;
            DirectoryHandlers = new ObservableCollection<string>();
            _backgroundColor = new SolidColorBrush(Colors.SlateGray);
            BindingOperations.EnableCollectionSynchronization(DirectoryHandlers, DirectoryHandlers);
            GuiTcpClientSingleton.Instance.ConnectedToService += OnClientConnectedToService;
            GuiTcpClientSingleton.Instance.ConfigurationReceived += OnSettingsInfoReceived;
        }

        public ObservableCollection<string> DirectoryHandlers { get; set; }


        public string OutputDirectory
        {
            get => _outputDirectory;
            set
            {
                _outputDirectory = value;
                NotifyPropertyChanged("OutputDirectory");
            }
        }

        public SolidColorBrush BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                NotifyPropertyChanged("BackgroundColor");
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


        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Called when the remove button is clicked.
        ///     Asks Service to remove selected directory handler.
        ///     Note: This method doesn't remove the dir handler from the ListBox.
        /// </summary>
        /// <param name="selectedDirectoryHandler"></param>
        public void OnRemove(string selectedDirectoryHandler)
        {
            Debug.WriteLine("In OnRemove");
            string command = (int) CommandEnum.CloseDirectoryHandlerCommand + "|" + selectedDirectoryHandler;
            GuiTcpClientSingleton.Instance.Writer.Write(command);
        }

        /// <summary>
        ///     Called when a client is connected to service.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnClientConnectedToService(object sender, EventArgs e)
        {
            Debug.WriteLine("In SettingsViewModel->OnClientConnectedToService");
            _uiDispatcher.BeginInvoke(new Action(() => { BackgroundColor = new SolidColorBrush(Colors.DarkCyan); }));
        }

        public void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        ///     Sets the settings information.
        /// </summary>
        /// <param name="settingsInfo">The settings information.</param>
        private void SetSettingsInfo(SettingsInfo settingsInfo)
        {
            LogName = settingsInfo.LogName;
            SourceName = settingsInfo.SourceName;
            OutputDirectory = settingsInfo.OutputDirectory;
            ThumbnailSize = settingsInfo.ThumbnailSize;

            List<string> handlers = settingsInfo.HandledDirectories;

            foreach (string handler in handlers)
                DirectoryHandlers.Add(handler);
        }

        /// <summary>
        ///     Called when the settings information is received.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="ConfigurationReceivedEventArgs" /> instance containing the event data.</param>
        public void OnSettingsInfoReceived(object sender, ConfigurationReceivedEventArgs eventArgs)
        {
            Debug.WriteLine("In OnSettingsInfoReceived");
            SetSettingsInfo(SettingsInfo.FromJson(eventArgs.Args));
        }
    }
}