using System;
using System.Diagnostics;
using Infrastructure.Logging;

namespace ImageService.Logger
{
    public class LoggingService : ILoggingService
    {
        public event EventHandler<MessageRecievedEventArgs> MsgRecievedEvent;

        public void Log(string message, EventLogEntryType type)
        {
            MessageRecievedEventArgs msgRecievedArgs = new MessageRecievedEventArgs
            {
                Message = message,
                EventLogEntryType = type
            };

            MsgRecievedEvent?.Invoke(this, msgRecievedArgs);
        }
    }
}