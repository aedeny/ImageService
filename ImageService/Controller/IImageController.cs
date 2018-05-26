using System.Diagnostics;
using ImageService.Commands;
using Infrastructure.Enums;

namespace ImageService.Controller
{
    public interface IImageController
    {
        string ExecuteCommand(CommandEnum commandId, string[] args, out EventLogEntryType result);
        void AddCommand(CommandEnum commandEnum, ICommand command);
    }
}