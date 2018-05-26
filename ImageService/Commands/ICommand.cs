using System.Diagnostics;

namespace ImageService.Commands
{
    public interface ICommand
    {
        // Executes the corresponding method.
        string Execute(string[] args, out EventLogEntryType result);
    }
}