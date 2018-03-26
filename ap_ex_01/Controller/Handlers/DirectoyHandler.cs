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
        private string m_path;
        #endregion

        public event EventHandler<DirectoryCloseEventArgs> DirectoryClosedEvent;

        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void StartHandleDirectory(string dirPath)
        {
            throw new NotImplementedException();
        }

    }
}
