using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ImageServiceGUI.Model;
using Infrastructure.Event;
using Microsoft.Practices.Prism.Commands;
using Communication;

namespace ImageServiceGUI.ViewModels
{
    internal class SettingsViewModel : ViewModel
    {
        private readonly ISettingsModel _model;
        private readonly Dispatcher _uiDispatcher;
        private string _selectedDirectoryHandler;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SettingsViewModel" /> class.
        /// </summary>
        public SettingsViewModel()
        {
            Debug.WriteLine("SettingsViewModel c'tor");
            _uiDispatcher = Application.Current.Dispatcher;
            _model = new SettingsModel(_uiDispatcher);
            GuiTcpClientSingleton.Instance.DirectoryHandlerRemoved += OnDirectoryHandlerSuccessfulyRemoved;
            SubmitRemoveCommand = new DelegateCommand<object>(OnRemove, CanRemove);
            PropertyChanged += RemoveSelectedHandler;
            _model.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                NotifyPropertyChanged(args.PropertyName);
            };
        }

        public ObservableCollection<string> DirectoryHandlers
        {
            get => _model.DirectoryHandlers;
            set => _model.DirectoryHandlers = value;
        }

        public SolidColorBrush BackgroundColor => _model.BackgroundColor;

        public string OutputDirectory => _model.OutputDirectory;

        public int ThumbnailSize => _model.ThumbnailSize;

        public string LogName => _model.LogName;

        public string SourceName => _model.SourceName;

        public ICommand SubmitRemoveCommand { get; }

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
            DelegateCommand<object> command = SubmitRemoveCommand as DelegateCommand<object>;
            command?.RaiseCanExecuteChanged();
        }


        /// <summary>
        ///     Called when the remove button is clicked.
        ///     Asks Service to remove selected directory handler.
        ///     Note: This method doesn't remove the dir handler from the ListBox.
        /// </summary>
        /// <param name="obj">The object.</param>
        public void OnRemove(object obj)
        {
            _model.OnRemove(SelectedDirectoryHandler);
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