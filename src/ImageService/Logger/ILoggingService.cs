using System;
using System.Diagnostics;
using Infrastructure.Logging;

namespace ImageService.Logger
{
    public interface ILoggingService
    {
        event EventHandler<MessageRecievedEventArgs> MessageRecieved;
        void Log(string message, EventLogEntryType type);
    }
}