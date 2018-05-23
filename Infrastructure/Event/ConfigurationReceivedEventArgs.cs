using System;

namespace Infrastructure.Event
{
    // WHICH ARGS ARE NEEDED?
    public class ConfigurationReceivedEventArgs : EventArgs
    {
        public ConfigurationReceivedEventArgs(string args)
        {
            Args = args;
        }

        public string Args { get; set; }
    }
}