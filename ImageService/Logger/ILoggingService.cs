using System;
using System.Diagnostics;
using Infrastructure.Logging;

namespace ImageService.Logger
{
    public interface ILoggingService
    {
        event EventHandler<MessageRecievedEventArgs> MsgRecievedEvent;
        void Log(string message, EventLogEntryType type);
    }
}