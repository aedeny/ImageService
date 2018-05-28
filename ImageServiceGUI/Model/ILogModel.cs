using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Threading;
using Infrastructure.Logging;

namespace ImageServiceGUI.Model
{
    internal interface ILogModel
    {
        void OnLogMessageRecieved(object sender, MessageRecievedEventArgs e);
        void OnClientConnectedToService(object sender, EventArgs e);
        Dispatcher UiDispatcher { get; set; }
        ObservableCollection<Tuple<SolidColorBrush, EventLogEntryType, string>> LogList { get; set; }
        SolidColorBrush BackgroundColor { get; set; }
    }
}