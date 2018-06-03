using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using ImageServiceGUI.Model;
using Communication;

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
            GuiTcpClientSingleton.Instance.ConnectedToService += OnClientConnectedToService;
            GuiTcpClientSingleton.Instance.LogMessageRecieved += _logModel.OnLogMessageRecieved;

            LogList = _logModel.LogList;
            BackgroundColor = new SolidColorBrush(Colors.SlateGray);

            _logModel.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                NotifyPropertyChanged(args.PropertyName);
            };
        }

        public ObservableCollection<Tuple<SolidColorBrush, EventLogEntryType, string>> LogList { get; set; }
        public SolidColorBrush BackgroundColor { get; set; }

        public void OnClientConnectedToService(object sender, EventArgs e)
        {
            Debug.WriteLine("In LogModel->OnClientConnectedToService");
            _logModel.UiDispatcher.BeginInvoke(new Action(() =>
            {
                BackgroundColor = new SolidColorBrush(Colors.DarkCyan);
                NotifyPropertyChanged("BackgroundColor");
            }));
        }
    }
}