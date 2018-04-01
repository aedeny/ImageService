using ImageService.Enums;
using ImageService.Logger.Model;

namespace ImageService.Controller
{
    public interface IImageController
    {
        string ExecuteCommand(CommandEnum commandId, string[] args, out MessageTypeEnum result);
    }
}