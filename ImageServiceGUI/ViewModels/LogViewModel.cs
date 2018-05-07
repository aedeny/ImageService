using System;
using System.Collections.Generic;
using System.Windows.Media;
using Infrastructure.Logging;

namespace ImageServiceGUI.ViewModels
{
    internal class LogViewModel
    {
        public LogViewModel()
        {
            OurTcpClientSingleton.Instance.LogMsgRecieved += OnLogRecieved;
            List<Tuple<MessageTypeEnum, string>> logsList = OurTcpClientSingleton.Instance.GetLogList();
            LogList = new List<Tuple<SolidColorBrush, MessageTypeEnum, string>>();
            foreach (Tuple<MessageTypeEnum, string> log in logsList)
                LogList.Add(
                    new Tuple<SolidColorBrush, MessageTypeEnum, string>(MessageTypeColor(log.Item1), log.Item1,
                        log.Item2));
        }

        public List<Tuple<SolidColorBrush, MessageTypeEnum, string>> LogList { get; set; }

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
    }
}