using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading;
using Communication;
using Infrastructure.Logging;

namespace Web.Models
{
    public class Logs
    {
        private readonly object _lock = new object();

        public Logs()
        {
            if (GuiTcpClientSingleton.Instance.Connected)
            {
                GuiTcpClientSingleton.Instance.Close();
            }

            LogList = new ObservableCollection<Tuple<string, string>>();

            // TODO OnLogMessageRecieved is redundant? Don't we only use full log history?
            GuiTcpClientSingleton.Instance.LogMessageRecieved += OnLogMessageRecieved;

            GuiTcpClientSingleton.Instance.LogHistoryMessageRecieved += OnLogHistoryRecieved;

            lock (_lock)
            {
                Monitor.Wait(_lock, 5000);
            }
        }

        [DataType(DataType.Text)]
        [Display(Name = "Logs List")]
        public ObservableCollection<Tuple<string, string>> LogList { get; set; }

        public void OnLogMessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            LogList.Insert(0, new Tuple<string, string>(e.EventLogEntryType.ToString(), e.Message.Replace(".", "")));
        }

        public void OnLogHistoryRecieved(object sender, MessageRecievedEventArgs e)
        {
            GuiTcpClientSingleton.Instance.LogMessageRecieved -= OnLogMessageRecieved;
            GuiTcpClientSingleton.Instance.LogHistoryMessageRecieved -= OnLogHistoryRecieved;

            lock (_lock)
            {
                Monitor.Pulse(_lock);
            }
        }
    }
}