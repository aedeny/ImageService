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
        public event PropertyChangedEventHandler PropertyChanged;

        public LogViewModel()
        {
            _uiDispatcher = Application.Current.Dispatcher;
            BackgroundColor = new SolidColorBrush(Colors.SlateGray);
            OurTcpClientSingleton.Instance.ConnectedToService += OnConnectedToService;
            OurTcpClientSingleton.Instance.LogMsgRecieved += OnLogRecieved;

            // THIS IS WHERE THE MAGIC HAPPENS!!!! LOGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG!
            List<Tuple<MessageTypeEnum, string>> logsList = OurTcpClientSingleton.Instance.GetLogList();

            LogList = new List<Tuple<SolidColorBrush, MessageTypeEnum, string>>();
            foreach (Tuple<MessageTypeEnum, string> log in logsList)
                LogList.Add(
                    new Tuple<SolidColorBrush, MessageTypeEnum, string>(MessageTypeColor(log.Item1), log.Item1,
                        log.Item2));
        }

        public SolidColorBrush BackgroundColor { get; set; }
        public List<Tuple<SolidColorBrush, MessageTypeEnum, string>> LogList { get; set; }


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

        public void OnLogRecieved(object obj, EventArgs e)
        {
            // TODO Add log msg to view
        }

        public SolidColorBrush MessageTypeColor(MessageTypeEnum messageType)
        {
            switch (messageType)
            {
                case MessageTypeEnum.Failure:
                    return new SolidColorBrush(Colors.Red);
                case MessageTypeEnum.Info:
                    return new SolidColorBrush(Colors.Green);
                case MessageTypeEnum.Warning:
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