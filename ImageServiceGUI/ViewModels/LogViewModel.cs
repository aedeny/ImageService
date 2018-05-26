using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ImageServiceGUI.Annotations;
using Infrastructure.Logging;

namespace ImageServiceGUI.ViewModels
{
    internal class LogViewModel : INotifyPropertyChanged
    {
        private readonly Dispatcher _uiDispatcher;

        public LogViewModel()
        {
            _uiDispatcher = Application.Current.Dispatcher;
            BackgroundColor = new SolidColorBrush(Colors.SlateGray);
            GuiTcpClientSingleton.Instance.ConnectedToService += OnConnectedToService;
            GuiTcpClientSingleton.Instance.LogMessageRecieved += OnLogMessageRecieved;

            List<Tuple<EventLogEntryType, string>> logsList = GuiTcpClientSingleton.Instance.GetLogList();

            LogList = new List<Tuple<SolidColorBrush, EventLogEntryType, string>>();
            foreach (Tuple<EventLogEntryType, string> log in logsList)
                LogList.Add(
                    new Tuple<SolidColorBrush, EventLogEntryType, string>(MessageTypeColor(log.Item1), log.Item1,
                        log.Item2));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SolidColorBrush BackgroundColor { get; set; }
        public List<Tuple<SolidColorBrush, EventLogEntryType, string>> LogList { get; set; }

        /// <summary>
        ///     Adds a new log entry to the GUI's list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLogMessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            _uiDispatcher.BeginInvoke(new Action(() =>
            {
                LogList.Add(new Tuple<SolidColorBrush, EventLogEntryType, string>(MessageTypeColor(e.EventLogEntryType),
                    e.EventLogEntryType, e.Message));

                NotifyPropertyChanged("LogList");
            }));
        }


        private void OnConnectedToService(object sender, EventArgs e)
        {
            Debug.WriteLine("In LogViewModel->OnConnectedToService");
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


        public SolidColorBrush MessageTypeColor(EventLogEntryType messageType)
        {
            switch (messageType)
            {
                case EventLogEntryType.Error:
                    return new SolidColorBrush(Colors.Red);
                case EventLogEntryType.Information:
                    return new SolidColorBrush(Colors.Green);
                case EventLogEntryType.Warning:
                    return new SolidColorBrush(Colors.Yellow);
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