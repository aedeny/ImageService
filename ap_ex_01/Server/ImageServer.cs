using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController mController;
        private ILoggingService mLoggingService;
        #endregion

        #region Properties
        // The event that notifies about a new command being recieved
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;
        #endregion

        public ImageServer(IImageController controller, ILoggingService loggingService)
        {
            mController = controller;
            mLoggingService = loggingService;
        }

        public void CreateHandler(string path)
        {
            DirectoyHandler dh = new DirectoyHandler(mController, mLoggingService, path);
            dh.StartHandleDirectory(path);
            CommandRecieved += dh.OnCommandRecieved;
        }

        public void OnCloseServer()
        {
            CommandRecieved?.Invoke(this, new CommandRecievedEventArgs(CommandEnum.CloseCommand, null, null));
        }

    }
}
