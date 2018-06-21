using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ImageService.Logger;
using ImageService.Server;
using Infrastructure.Enums;
using Infrastructure.Event;

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
        private readonly Dictionary<CommandEnum, Action<string[]>> _commandsDictionary;
        private readonly FileSystemWatcher _dirWatcher;
        private readonly string[] _extenstions = {".jpg", ".jpeg", ".png", ".bmp", ".gif"};


        private readonly IImageController _imageController;
        private readonly ILoggingService _loggingService;
        private readonly string _path;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DirectoyHandler" /> class.
        /// </summary>
        /// <param name="imageController">The image controller.</param>
        /// <param name="loggingService">The logging service.</param>
        /// <param name="path">The path.</param>
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

        public event EventHandler<DirectoryHandlerClosedEventArgs> DirectoryHandlerClosed;

        /// <summary>
        ///     Called when a command is recieved.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="CommandRecievedEventArgs" /> instance containing the event data.</param>
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            if (!_commandsDictionary.TryGetValue(e.CommandId, out Action<string[]> currentCommand))
            {
                return;
            }

            currentCommand.BeginInvoke(e.Args, null, null);
        }


        /// <summary>
        ///     Handles the directory.
        /// </summary>
        /// <param name="dirPath">The dir path.</param>
        public void HandleDirectory(string dirPath)
        {
            _dirWatcher.Created += OnNewFileCreated;
            _loggingService.Log("Started handling directory " + _path, EventLogEntryType.Information);
        }

        /// <summary>
        ///     Called when a new file is created.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs" /> instance containing the event data.</param>
        private void OnNewFileCreated(object sender, FileSystemEventArgs e)
        {
            _loggingService.Log("OnNewFileCreated: " + e.FullPath, EventLogEntryType.Information);
            string filePath = new FileInfo(e.FullPath).FullName;

            if (!_extenstions.Contains(Path.GetExtension(filePath).ToLower()))
            {
                return;
            }

            string[] args = {filePath};

            // Notifies the controller about the newly created file
            string msg =
                _imageController.ExecuteCommand(CommandEnum.NewFileCommand, args, out EventLogEntryType result);

            _loggingService.Log(msg, result);
        }

        /// <summary>
        ///     Called when a directory handler is closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="DirectoryHandlerClosedEventArgs" /> instance containing the event data.</param>
        public void OnDirectoryHandlerClosed(object sender, DirectoryHandlerClosedEventArgs args)
        {
            if (args != null && !args.DirectoryPath.Equals(_path))
            {
                return;
            }

            _dirWatcher.Created -= OnNewFileCreated;

            DirectoryHandlerClosed?.Invoke(this,
                new DirectoryHandlerClosedEventArgs(_path, "in OnDirectoryHandlerClosed"));

            _loggingService.Log("Stopped handling directory " + _path, EventLogEntryType.Information);
            if (args != null)
            {
                args.Closed = true;
            }

            ImageServer imageServer = (ImageServer) sender;
            imageServer.DirectoryHandlerClosed -= OnDirectoryHandlerClosed;
            imageServer.CommandRecieved -= OnCommandRecieved;
        }
    }
}