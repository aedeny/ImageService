using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Communication;
using Infrastructure.Logging;

namespace Web.Models
{
    public class Logs
    {
        [DataType(DataType.Text)]
        [Display(Name = "Logs List")]
        public ObservableCollection<Tuple<string, string, string>> LogList { get; set; }
        private bool _logHistoryRecieved;

        public Logs()
        {
            _logHistoryRecieved = false;

            if (GuiTcpClientSingleton.Instance.Connected) GuiTcpClientSingleton.Instance.Close();

            LogList = new ObservableCollection<Tuple<string, string, string>>();
            GuiTcpClientSingleton.Instance.LogMessageRecieved += OnLogMessageRecieved;
            GuiTcpClientSingleton.Instance.LogHistoryMessageRecieved += OnLogHistoryRecieved;

            Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                    if (!_logHistoryRecieved)
                        Thread.Sleep(100);
                    else
                        break;
            }).Wait();
        }

        public void OnLogMessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            string color = GetMessageTypeColor(e.EventLogEntryType);
            LogList.Insert(0, new Tuple<string, string, string>(
                GetMessageTypeColor(e.EventLogEntryType), e.EventLogEntryType.ToString(), e.Message.Replace(".", "")));
        }

        public void OnLogHistoryRecieved(object sender, MessageRecievedEventArgs e)
        {
            _logHistoryRecieved = true;
            GuiTcpClientSingleton.Instance.LogMessageRecieved -= OnLogMessageRecieved;
            GuiTcpClientSingleton.Instance.LogHistoryMessageRecieved -= OnLogHistoryRecieved;
        }

        private static string GetMessageTypeColor(EventLogEntryType messageType)
        {
            switch (messageType)
            {
                case EventLogEntryType.Error:
                    return "red";
                case EventLogEntryType.Information:
                    return "green";
                case EventLogEntryType.Warning:
                    return "yellow";
                case EventLogEntryType.SuccessAudit:
                    return "green";
                case EventLogEntryType.FailureAudit:
                    return "red";
                default:
                    return "yellow";
            }
        }
    }
}