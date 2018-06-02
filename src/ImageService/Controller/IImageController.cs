using System.Diagnostics;
using ImageService.Commands;
using Infrastructure.Enums;

namespace ImageService.Controller
{
    public interface IImageController
    {
        /// <summary>
        ///     Executes the command.
        /// </summary>
        /// <param name="commandId">The command identifier.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        string ExecuteCommand(CommandEnum commandId, string[] args, out EventLogEntryType result);

        /// <summary>
        ///     Adds the command to commands dictionary.
        /// </summary>
        /// <param name="commandEnum">The command enum.</param>
        /// <param name="command">The command.</param>
        void AddCommand(CommandEnum commandEnum, ICommand command);
    }
}