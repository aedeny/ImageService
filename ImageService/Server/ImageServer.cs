using System;
using System.Linq;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Logger;
using Infrastructure.Enums;
using Infrastructure.Event;
using Infrastructure.Logging;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members

        public event EventHandler<DirectoryHandlerClosedEventArgs> CloseDirectoryHandler;
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
            CloseDirectoryHandler += dh.StopHandleDirectory;
        }

        public void Close()
        {
            CloseDirectoryHandler?.Invoke(this, null);
        }

        public string CloseHandler(DirectoryHandlerClosedEventArgs args, out MessageTypeEnum result)
        {
            CloseDirectoryHandler?.Invoke(this, args);
            result = MessageTypeEnum.Info;

            return args.DirectoryPath;
        }

        public void Parser(string command)
        {
            CommandEnum cid = (CommandEnum) Convert.ToInt16(command.Split(';')[0]);
            string[] args = command.Split(';').Skip(1).ToArray();

            _controller.ExecuteCommand(cid, args, out MessageTypeEnum _);
        }
    }
}