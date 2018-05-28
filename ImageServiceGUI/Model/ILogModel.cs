using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Threading;
using Infrastructure.Logging;

namespace ImageServiceGUI.Model
{
    internal interface ILogModel
    {
        Dispatcher UiDispatcher { get; set; }
        ObservableCollection<Tuple<SolidColorBrush, EventLogEntryType, string>> LogList { get; set; }
        void OnLogMessageRecieved(object sender, MessageRecievedEventArgs e);
        event PropertyChangedEventHandler PropertyChanged;
    }
}