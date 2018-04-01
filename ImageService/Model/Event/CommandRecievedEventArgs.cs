using System;
using ImageService.Enums;

namespace ImageService.Model.Event
{
    public class CommandRecievedEventArgs : EventArgs
    {
        public CommandEnum CommandId { get; set; }
        public string[] Args { get; set; }
        public string RequestDirPath { get; set; } 

        public CommandRecievedEventArgs(CommandEnum id, string[] args, string path)
        {
            CommandId = id;
            Args = args;
            RequestDirPath = path;
        }
    }
}
