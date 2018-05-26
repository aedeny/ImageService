using System;
using System.Diagnostics;

namespace Infrastructure.Logging
{
    public class MessageRecievedEventArgs : EventArgs
    {
        public EventLogEntryType EventLogEntryType { get; set; }
        public string Message { get; set; }
    }
}