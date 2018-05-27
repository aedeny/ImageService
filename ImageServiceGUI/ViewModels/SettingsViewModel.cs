using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Infrastructure;
using Infrastructure.Enums;
using Infrastructure.Event;
using Microsoft.Practices.Prism.Commands;

namespace ImageServiceGUI.ViewModels
{
    internal class SettingsViewModel : INotifyPropertyChanged
    {
        private const string WaitingForConnection = "Waiting for connection...";
        private readonly Dispatcher _uiDispatcher;
        private string _logName = WaitingForConnection;
        private string _outputDirectory = WaitingForConnection;

        private string _selectedDirectoryHandler;
        private string _sourceName = WaitingForConnection;
        private int _thumbnailSize;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SettingsViewModel" /> class.
        /// </summary>
        public SettingsViewModel()
        {
            _uiDispatcher = Application.Current.Dispatcher;
            Debug.WriteLine("SettingsViewModel c'tor");

            DirectoryHandlers = new ObservableCollection<string>();
            BindingOperations.EnableCollectionSynchronization(DirectoryHandlers, DirectoryHandlers);
            _uiDispatcher = Application.Current.Dispatcher;
            BackgroundColor = new SolidColorBrush(Colors.SlateGray);
            GuiTcpClientSingleton.Instance.ConnectedToService += OnClientConnectedToService;
            GuiTcpClientSingleton.Instance.DirectoryHandlerRemoved += OnDirectoryHandlerSuccessfulyRemoved;
            SubmitRemove = new DelegateCommand<object>(OnRemove, CanRemove);
            PropertyChanged += RemoveSelectedHandler;

            GuiTcpClientSingleton.Instance.ConfigurationReceived += OnSettingsInfoReceived;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> DirectoryHandlers { get; }

        public SolidColorBrush BackgroundColor { get; set; }

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
                NotifyPropertyChanged("SelectedDirectoryHandler");
            }
        }

        /// <summary>
        ///     Called when a client is connected to service.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnClientConnectedToService(object sender, EventArgs e)
        {
            Debug.WriteLine("In SettingsViewModel->OnClientConnectedToService");
            _uiDispatcher.BeginInvoke(new Action(() =>
            {
                BackgroundColor = new SolidColorBrush(Colors.DarkCyan);
                NotifyPropertyChanged("BackgroundColor");
            }));
        }

        public void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        /// <summary>
        ///     Called when the settings information is received.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="ConfigurationReceivedEventArgs" /> instance containing the event data.</param>
        public void OnSettingsInfoReceived(object sender, ConfigurationReceivedEventArgs eventArgs)
        {
            Debug.WriteLine("In OnSettingsInfoReceived");
            _uiDispatcher.BeginInvoke(new Action(() => { SetSettingsInfo(SettingsInfo.FromJson(eventArgs.Args)); }));
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

            foreach (string handler in handlers) DirectoryHandlers.Add(handler);
        }

        /// <summary>
        ///     Called when a directory handler is successfuly removed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="DirectoryHandlerClosedEventArgs" /> instance containing the event data.</param>
        public void OnDirectoryHandlerSuccessfulyRemoved(object sender, DirectoryHandlerClosedEventArgs eventArgs)
        {
            Debug.WriteLine("In OnDirectoryHandlerSuccessfulyRemoved");
            Debug.WriteLine(eventArgs.DirectoryPath);
            _uiDispatcher.BeginInvoke(new Action(() =>
            {
                DirectoryHandlers.Remove(eventArgs.DirectoryPath);
                SelectedDirectoryHandler = null;
                NotifyPropertyChanged("DirectoryHandlers");
            }));
        }

        /// <summary>
        ///     Removes the selected handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="PropertyChangedEventArgs" /> instance containing the event data.</param>
        public void RemoveSelectedHandler(object sender, PropertyChangedEventArgs args)
        {
            DelegateCommand<object> command = SubmitRemove as DelegateCommand<object>;
            command?.RaiseCanExecuteChanged();
        }


        /// <summary>
        ///     Called when the remove button is clicked.
        ///     Asks Service to remove selected directory handler.
        ///     Note: This method doesn't remove the dir handler from the ListBox.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void OnRemove(object obj)
        {
            Debug.WriteLine("In OnRemove");
            string command = (int) CommandEnum.CloseDirectoryHandlerCommand + "|" + SelectedDirectoryHandler;
            GuiTcpClientSingleton.Instance.Writer.Write(command);
        }

        /// <summary>
        ///     Determines whether this instance can remove the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///     <c>true</c> if this instance can remove the specified object; otherwise, <c>false</c>.
        /// </returns>
        private bool CanRemove(object obj)
        {
            Debug.WriteLine("In CanRemove");
            return !string.IsNullOrEmpty(SelectedDirectoryHandler);
        }
    }
}