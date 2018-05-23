using System;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Logger;
using Infrastructure.Event;
using Infrastructure.Logging;

namespace ImageService.Server
{
    public class ImageServer
    {
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
            DirectoryHandlerClosed += dh.OnDirectoryHandlerClosed;
        }

        public void Close()
        {
            DirectoryHandlerClosed?.Invoke(this, null);
        }

        public string CloseHandler(DirectoryHandlerClosedEventArgs args, out MessageTypeEnum result)
        {
            DirectoryHandlerClosed?.Invoke(this, args);
            result = MessageTypeEnum.Info;

            return args.DirectoryPath;
        }

        #region Members

        public event EventHandler<DirectoryHandlerClosedEventArgs> DirectoryHandlerClosed;
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;
        private readonly IImageController _controller;
        private readonly ILoggingService _loggingService;

        #endregion

        //public void Parser(string command)
        //{
        //    CommandEnum cid = (CommandEnum) Convert.ToInt16(command.Split(';')[0]);
        //    string[] args = command.Split(';').Skip(1).ToArray();

        //    _controller.ExecuteCommand(cid, args, out MessageTypeEnum _);
        //}
    }
}