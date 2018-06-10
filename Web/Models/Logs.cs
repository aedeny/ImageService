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
            LogList = new ObservableCollection<Tuple<string, string>>();
            if (!Utils.IsServiceActive("ImageService"))
            {
                return;
            }

            GuiTcpClientSingleton.Instance.Close();

            NumOfWarnings = 0;
            NumOfInformation = 0;
            NumOfErrors = 0;

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
        public int NumOfWarnings { get; private set; }

        [DataType(DataType.Text)]
        [Display(Name = "No Information")]
        public int NumOfInformation { get; private set; }

        [DataType(DataType.Text)]
        [Display(Name = "No Errors")]
        public int NumOfErrors { get; private set; }

        public void OnLogMessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            if (e.EventLogEntryType.ToString().Equals("Information"))
            {
                NumOfInformation++;
            }
            else if (e.EventLogEntryType.ToString().Equals("Error"))
            {
                NumOfErrors++;
            }
            else
            {
                e.EventLogEntryType = EventLogEntryType.Warning;
                NumOfWarnings++;
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