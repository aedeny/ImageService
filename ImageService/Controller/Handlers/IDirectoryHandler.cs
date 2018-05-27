using System;
using Infrastructure.Event;

namespace ImageService.Controller.Handlers
{
    public interface IDirectoryHandler
    {
        event EventHandler<DirectoryHandlerClosedEventArgs> DirectoryHandlerClosed;
        void HandleDirectory(string dirPath);
        void OnCommandRecieved(object sender, CommandRecievedEventArgs e);
    }
}