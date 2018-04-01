using System;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Enums;
using ImageService.Logger;
using ImageService.Model.Event;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private readonly IImageController _controller;
        private readonly ILoggingService _loggingService;
        #endregion

        #region Properties
        // The event that notifies about a new command being recieved
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;
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
        }

        public void Close()
        {
            CommandRecieved?.Invoke(this, new CommandRecievedEventArgs(CommandEnum.CloseCommand, null, null));
        }

    }
}
