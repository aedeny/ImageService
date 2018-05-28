using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Threading;
using Infrastructure.Logging;

namespace ImageServiceGUI.Model
{
    internal class LogModel : ILogModel
    {
        private Dispatcher _uiDispatcher;

        public Dispatcher UiDispatcher
        {
            get => _uiDispatcher;
            set => _uiDispatcher = value;

        }

        public LogModel()
        {
            LogList = new ObservableCollection<Tuple<SolidColorBrush, EventLogEntryType, string>>();
            BackgroundColor = new SolidColorBrush(Colors.SlateGray);
        }

        public ObservableCollection<Tuple<SolidColorBrush, EventLogEntryType, string>> LogList { get; set; }
        public SolidColorBrush BackgroundColor { get; set; }


        public void OnLogMessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            _uiDispatcher.BeginInvoke(new Action(() =>
            {
                LogList.Insert(0, new Tuple<SolidColorBrush, EventLogEntryType, string>(
                    GetMessageTypeColor(e.EventLogEntryType),
                    e.EventLogEntryType, e.Message));

                NotifyPropertyChanged("LogList");
            }));
        }

        public void OnClientConnectedToService(object sender, EventArgs e)
        {
            Debug.WriteLine("In LogModel->OnClientConnectedToService");
            _uiDispatcher.BeginInvoke(new Action(() =>
            {
                BackgroundColor = new SolidColorBrush(Colors.DarkCyan);
                NotifyPropertyChanged("BackgroundColor");
            }));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private SolidColorBrush GetMessageTypeColor(EventLogEntryType messageType)
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
    }
}