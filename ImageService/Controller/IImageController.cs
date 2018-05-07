using ImageService.Commands;
using Infrastructure.Enums;
using Infrastructure.Logging;

namespace ImageService.Controller
{
    public interface IImageController
    {
        string ExecuteCommand(CommandEnum commandId, string[] args, out MessageTypeEnum result);
        void AddCommand(CommandEnum commandEnum, ICommand command);
    }
}