using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Event
{
    // WHICH ARGS ARE NEEDED?
    public class ConfigurationReceivedEventArgs : EventArgs
    {
        public string[] Args { get; set; }

        public ConfigurationReceivedEventArgs(string[] args)
        {
            Args = args;
        }
    }
}