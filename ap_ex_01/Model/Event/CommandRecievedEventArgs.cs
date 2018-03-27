using ImageService.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Model
{
    public class CommandRecievedEventArgs : EventArgs
    {
        public CommandEnum CommandID { get; set; }
        public string[] Args { get; set; }
        public string RequestDirPath { get; set; } 

        public CommandRecievedEventArgs(CommandEnum id, string[] args, string path)
        {
            CommandID = id;
            Args = args;
            RequestDirPath = path;
        }
    }
}
