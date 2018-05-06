using System;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using Infrastructure.Enums;
using ImageService.Logger;
using Infrastructure.Logging;
using ImageService.Model.Event;
using System.Linq;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members

        public event EventHandler<DirectoryCloseEventArgs> OnClose;
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

        public string CloseHandler(DirectoryCloseEventArgs args, out MessageTypeEnum result)
        {
            OnClose?.Invoke(this, args);
            result = MessageTypeEnum.Info;

            return args.DirectoryPath + " Closed";
        }

        public void Parser(string command)
        {
            CommandEnum cid = (CommandEnum)Convert.ToInt16(command.Split(';')[0]);
            string[] args = command.Split(';', ',').Skip(1).ToArray();

            _controller.ExecuteCommand(cid, args, out MessageTypeEnum result);
        }
    }
}