using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageService.Logger;
using ImageService.Server;
using Infrastructure.Enums;
using Infrastructure.Event;
using Infrastructure.Logging;

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
        public DirectoyHandler(IImageController imageController, ILoggingService loggingService, string path)
        {
            _imageController = imageController;
            _loggingService = loggingService;
            _dirWatcher = new FileSystemWatcher(path)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true
            };
            _path = path;
            _commandsDictionary = new Dictionary<CommandEnum, Action<string[]>>();
        }

        public event EventHandler<DirectoryHandlerClosedEventArgs> DirectoryHandlerClosedEventHandler;

        // Invokes the corresponding method
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            if (!_commandsDictionary.TryGetValue(e.CommandId, out Action<string[]> currentCommand)) return;

            currentCommand.BeginInvoke(e.Args, null, null);
        }

        public void StartHandleDirectory(string dirPath)
        {
            _dirWatcher.Created += OnNewFileCreated;
            _loggingService.Log("Started handling directory " + _path, MessageTypeEnum.Info);
        }

        private void OnNewFileCreated(object sender, FileSystemEventArgs e)
        {
            _loggingService.Log("OnNewFileCreated: " + e.FullPath, MessageTypeEnum.Info);
            string filePath = new FileInfo(e.FullPath).FullName;

            if (!_extenstions.Contains(Path.GetExtension(filePath).ToLower())) return;

            string[] args = {filePath};

            // Notifies the controller about the newly created file
            string msg =
                _imageController.ExecuteCommand(CommandEnum.NewFileCommand, args, out MessageTypeEnum result);

            _loggingService.Log(msg, result);
        }

        public void OnDirectoryHandlerClosed(object sender, DirectoryHandlerClosedEventArgs args)
        {
            if (args != null && !args.DirectoryPath.Equals(_path)) return;
            _dirWatcher.Created -= OnNewFileCreated;

            ImageServer imageServer = (ImageServer) sender;
            imageServer.DirectoryHandlerClosed -= OnDirectoryHandlerClosed;
            imageServer.CommandRecieved -= OnCommandRecieved;

            DirectoryHandlerClosedEventHandler?.Invoke(this,
                new DirectoryHandlerClosedEventArgs(_path, "in OnDirectoryHandlerClosed"));
            _loggingService.Log("Stopped handling directory " + _path, MessageTypeEnum.Info);
            if (args != null) args.Closed = true;
        }

        #region Members

        private readonly IImageController _imageController;
        private readonly ILoggingService _loggingService;
        private readonly FileSystemWatcher _dirWatcher;
        private readonly string _path;
        private readonly Dictionary<CommandEnum, Action<string[]>> _commandsDictionary;
        private readonly string[] _extenstions = {".jpg", ".jpeg", ".png", ".bmp", ".gif"};

        #endregion
    }
}