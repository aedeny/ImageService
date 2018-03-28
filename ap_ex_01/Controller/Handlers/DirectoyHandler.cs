using ImageService.Model;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Model;
using System.Text.RegularExpressions;

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
        #region Members
        private IImageController mImageController;
        private ILoggingService mLoggingService;
        private FileSystemWatcher mDirWatcher;
        private string mPath;
        private readonly string[] mExtenstions = { ".jpg", ".png", ".bmp", ".gif" };
        #endregion

        public event EventHandler<DirectoryCloseEventArgs> DirectoryClosedEvent;

        public DirectoyHandler(IImageController imageController, ILoggingService loggingService, string path)
        {
            mImageController = imageController;
            mLoggingService = loggingService;
            mDirWatcher = new FileSystemWatcher(path);
            mPath = path;
        }

        private void OnNewFileCreated(object sender, FileSystemEventArgs e)
        {
            foreach (string file in Directory.GetFiles(mPath))
            {
                if (mExtenstions.Contains(Path.GetExtension(file)))
                {
                    string[] args = { file };
                    OnCommandRecieved(this, new CommandRecievedEventArgs(CommandEnum.NewFileCommand, args, mPath));
                }
            }
        }

        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            string msg = mImageController.ExecuteCommand(e.CommandID, e.Args, out bool result);

            if (result)
            {
                mLoggingService.Log(msg, MessageTypeEnum.INFO);
            }
            else
            {
                mLoggingService.Log(msg, MessageTypeEnum.FAILURE);
            }
        }

        public void StartHandleDirectory(string dirPath)
        {
            mDirWatcher.Created += OnNewFileCreated;
        }
    }
}
