using ImageService.Model;
using System;

namespace ImageService.Controller.Handlers
{
    public interface IDirectoryHandler
    {
        // The Event That Notifies that the Directory is being closed
        event EventHandler<DirectoryCloseEventArgs> DirectoryClosedEvent;

        // The Function Recieves the directory to Handle
        void StartHandleDirectory(string dirPath);

        // The Event that will be activated upon new Command
        void OnCommandRecieved(object sender, CommandRecievedEventArgs e);
    }
}
