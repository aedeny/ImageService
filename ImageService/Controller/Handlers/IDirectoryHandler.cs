using System;
using Infrastructure.Event;

namespace ImageService.Controller.Handlers
{
    public interface IDirectoryHandler
    {
        // The event that notifies that the directory is being closed.
        event EventHandler<DirectoryHandlerClosedEventArgs> DirectoryHandlerClosedEventHandler;

        // The function recieves the directory to handle.
        void StartHandleDirectory(string dirPath);

        // The event that will be activated upon new command.
        void OnCommandRecieved(object sender, CommandRecievedEventArgs e);
    }
}