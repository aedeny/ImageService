using System.Diagnostics;

namespace ImageService.Commands
{
    public interface ICommand
    {
        /// <summary>
        ///     Executes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        string Execute(string[] args, out EventLogEntryType result);
    }
}