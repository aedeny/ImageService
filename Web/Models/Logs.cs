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

            NoWarnings = true;
            NoInformation = true;
            NoErrors = true;

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

        [DataType(DataType.Text)]
        [Display(Name = "No Warnings")]
        public bool NoWarnings { get; private set; }

        [DataType(DataType.Text)]
        [Display(Name = "No Information")]
        public bool NoInformation { get; private set; }

        [DataType(DataType.Text)]
        [Display(Name = "No Errors")]
        public bool NoErrors { get; private set; }

        public void OnLogMessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            // TODO it better...
            if (e.EventLogEntryType.ToString().Equals("Information"))
            {
                NoInformation = false;
            } else if (e.EventLogEntryType.ToString().Equals("Error"))
            {
                NoErrors = false;
            }
            else
            {
                e.EventLogEntryType = EventLogEntryType.Warning;
                NoWarnings = false;
            }

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