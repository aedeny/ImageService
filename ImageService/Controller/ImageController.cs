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

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImageController" /> class.
        /// </summary>
        public ImageController()
        {
            _commandsDictionary = new Dictionary<CommandEnum, ICommand>();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Executes the command.
        /// </summary>
        /// <param name="commandId">The command identifier.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
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


        /// <inheritdoc />
        /// <summary>
        ///     Adds the command to commands dictionary.
        /// </summary>
        /// <param name="commandEnum">The command enum.</param>
        /// <param name="command">The command.</param>
        public void AddCommand(CommandEnum commandEnum, ICommand command)
        {
            _commandsDictionary.Add(commandEnum, command);
        }
    }
}