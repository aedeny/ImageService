using System;
using System.Diagnostics;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Logger;
using Infrastructure.Event;

namespace ImageService.Server
{
    public class ImageServer
    {
        private readonly IImageController _controller;
        private readonly ILoggingService _loggingService;

        public ImageServer(IImageController controller, ILoggingService loggingService)
        {
            _controller = controller;
            _loggingService = loggingService;
        }

        public void CreateHandler(string path)
        {
            DirectoyHandler dh = new DirectoyHandler(_controller, _loggingService, path);
            dh.HandleDirectory(path);
            CommandRecieved += dh.OnCommandRecieved;
            DirectoryHandlerClosed += dh.OnDirectoryHandlerClosed;
        }

        public void Close()
        {
            DirectoryHandlerClosed?.Invoke(this, null);
        }

        public string CloseHandler(DirectoryHandlerClosedEventArgs args, out EventLogEntryType result)
        {
            DirectoryHandlerClosed?.Invoke(this, args);
            result = EventLogEntryType.Information;

            return args.DirectoryPath;
        }


        public event EventHandler<DirectoryHandlerClosedEventArgs> DirectoryHandlerClosed;
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;
    }
}