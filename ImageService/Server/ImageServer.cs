using System;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Enums;
using ImageService.Logger;
using ImageService.Logger.Model;
using ImageService.Model.Event;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        public event EventHandler OnClose;
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;
        private readonly IImageController _controller;
        private readonly ILoggingService _loggingService;

        #endregion


        public ImageServer(IImageController controller, ILoggingService loggingService)
        {
            _controller = controller;
            _loggingService = loggingService;
        }

        public void CreateHandler(string path)
        {
            DirectoyHandler dh = new DirectoyHandler(_controller, _loggingService, path);
            dh.StartHandleDirectory(path);
            CommandRecieved += dh.OnCommandRecieved;
            OnClose += dh.StopHandleDirectory;
        }

        public void Close()
        {
            OnClose?.Invoke(this, null);
        }
    }
}