using System;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using Infrastructure.Enums;
using ImageService.Logger;
using ImageService.Logger.Model;
using ImageService.Model.Event;
using System.Linq;
using System.IO;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members

        public event EventHandler<FileSystemEventArgs> OnClose;
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

        public void Close(FileSystemEventArgs args)
        {
            OnClose?.Invoke(this, args);
        }

        public void Parser(string command)
        {
            CommandEnum cid = (CommandEnum)Convert.ToInt16(command.Split(';')[0]);
            string[] args = command.Split(';', ',').Skip(1).ToArray();

            _controller.ExecuteCommand(cid, args, out MessageTypeEnum result);
        }
    }
}