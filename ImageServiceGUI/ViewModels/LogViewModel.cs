using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using ImageServiceGUI.Annotations;
using Infrastructure.Logging;

namespace ImageServiceGUI.ViewModels
{
    internal class LogViewModel : INotifyPropertyChanged
    {
        private readonly Dispatcher _uiDispatcher;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogViewModel" /> class.
        /// </summary>
        public LogViewModel()
        {
            _uiDispatcher = Application.Current.Dispatcher;
            BackgroundColor = new SolidColorBrush(Colors.SlateGray);
            GuiTcpClientSingleton.Instance.ConnectedToService += OnClientConnectedToService;
            GuiTcpClientSingleton.Instance.LogMessageRecieved += OnLogMessageRecieved;

            List<Tuple<EventLogEntryType, string>> logsList = GuiTcpClientSingleton.Instance.GetLogList();

            LogList = new ObservableCollection<Tuple<SolidColorBrush, EventLogEntryType, string>>();
            BindingOperations.EnableCollectionSynchronization(LogList, LogList);
            foreach (Tuple<EventLogEntryType, string> log in logsList)
                LogList.Add(
                    new Tuple<SolidColorBrush, EventLogEntryType, string>(GetMessageTypeColor(log.Item1), log.Item1,
                        log.Item2));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SolidColorBrush BackgroundColor { get; set; }
        public ObservableCollection<Tuple<SolidColorBrush, EventLogEntryType, string>> LogList { get; set; }


        /// <summary>
        ///     Called when a log message is recieved. Adds a new log entry to the GUI's list.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MessageRecievedEventArgs" /> instance containing the event data.</param>
        private void OnLogMessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            _uiDispatcher.BeginInvoke(new Action(() =>
            {
                LogList.Insert(0, new Tuple<SolidColorBrush, EventLogEntryType, string>(
                    GetMessageTypeColor(e.EventLogEntryType),
                    e.EventLogEntryType, e.Message));

                NotifyPropertyChanged("LogList");
            }));
        }


        /// <summary>
        ///     Called when client is connected to service.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnClientConnectedToService(object sender, EventArgs e)
        {
            Debug.WriteLine("In LogViewModel->OnClientConnectedToService");
            _uiDispatcher.BeginInvoke(new Action(() =>
            {
                BackgroundColor = new SolidColorBrush(Colors.DarkCyan);
                NotifyPropertyChanged("BackgroundColor");
            }));
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        ///     Gets the color of the message type.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <returns></returns>
        public SolidColorBrush GetMessageTypeColor(EventLogEntryType messageType)
        {
            switch (messageType)
            {
                case EventLogEntryType.Error:
                    return new SolidColorBrush(Colors.Red);
                case EventLogEntryType.Information:
                    return new SolidColorBrush(Colors.Green);
                case EventLogEntryType.Warning:
                    return new SolidColorBrush(Colors.Yellow);
                case EventLogEntryType.SuccessAudit:
                    return new SolidColorBrush(Colors.Green);
                case EventLogEntryType.FailureAudit:
                    return new SolidColorBrush(Colors.Red);
                default:
                    return new SolidColorBrush(Colors.White);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}