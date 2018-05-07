using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Logging;
using Infrastructure.Event;

namespace ImageService.Commands
{
    internal class SettingsInfoRetrievalCommand : ICommand
    {
        public event EventHandler<SettingsInfoRetrievalEventArgs> SettingsInfoRetrievalCommandRecieved;

        public string Execute(string[] args, out MessageTypeEnum result)
        {
            SettingsInfoRetrievalEventArgs eventArgs = new SettingsInfoRetrievalEventArgs();
            SettingsInfoRetrievalCommandRecieved?.Invoke(this, eventArgs);
            result = MessageTypeEnum.Info;
            return "Settings Info";
        }
    }
}