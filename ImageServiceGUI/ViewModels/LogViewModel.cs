using System;

namespace ImageServiceGUI.ViewModels
{
    class LogViewModel
    {
        public LogViewModel()
        {
            OurTcpClientSingleton.Instance.LogMsgRecieved += OnLogRecieved;
        }

        public void OnLogRecieved(object obj, EventArgs e)
        {
            // TODO Add log msg to view
        }
    }
}