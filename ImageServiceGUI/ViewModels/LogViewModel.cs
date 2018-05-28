using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using ImageServiceGUI.Model;

namespace ImageServiceGUI.ViewModels
{
    internal class LogViewModel : ViewModel
    {
        private readonly ILogModel _logModel;
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogViewModel" /> class.
        /// </summary>
        public LogViewModel()
        {
            _logModel = new LogModel {UiDispatcher = Application.Current.Dispatcher};
            GuiTcpClientSingleton.Instance.ConnectedToService += _logModel.OnClientConnectedToService;
            GuiTcpClientSingleton.Instance.LogMessageRecieved += _logModel.OnLogMessageRecieved;

            LogList = _logModel.LogList;
            BackgroundColor = _logModel.BackgroundColor;
        }

        public ObservableCollection<Tuple<SolidColorBrush, EventLogEntryType, string>> LogList { get; set; }
        public SolidColorBrush BackgroundColor { get; set; }
    }
}