using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ImageService.Commands;
using Infrastructure.Enums;

namespace ImageService.Controller
{
    public class ImageController : IImageController
    {
        private readonly Dictionary<CommandEnum, ICommand> _commandsDictionary;

        public ImageController()
        {
            _commandsDictionary = new Dictionary<CommandEnum, ICommand>();
        }

        public string ExecuteCommand(CommandEnum commandId, string[] args, out EventLogEntryType result)
        {
            result = EventLogEntryType.Error;

            if (!_commandsDictionary.TryGetValue(commandId, out ICommand currentCommand)) return "No such command";

            Task<Tuple<string, EventLogEntryType>> task = new Task<Tuple<string, EventLogEntryType>>(() =>
            {
                string s = currentCommand.Execute(args, out EventLogEntryType temp);
                return Tuple.Create(s, temp);
            });

            task.Start();
            Tuple<string, EventLogEntryType> tuple = task.Result;
            result = tuple.Item2;

            return tuple.Item1;
        }

        public void AddCommand(CommandEnum commandEnum, ICommand command)
        {
            _commandsDictionary.Add(commandEnum, command);
        }
    }
}