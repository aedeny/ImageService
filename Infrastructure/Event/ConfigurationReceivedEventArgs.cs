using System;

namespace Infrastructure.Event
{
    public class ConfigurationReceivedEventArgs : EventArgs
    {
        public ConfigurationReceivedEventArgs(string args)
        {
            Args = args;
        }

        public string Args { get; set; }
    }
}