using System;

namespace Infrastructure.Event
{
    public class DirectoryHandlerClosedEventArgs : EventArgs
    {
        public string DirectoryPath { get; set; }

        public string Message { get; set; }
        public bool Closed { get; set; }

        public DirectoryHandlerClosedEventArgs(string dirPath, string message)
        {
            DirectoryPath = dirPath;
            Message = message;
            Closed = false;
        }
    }
}