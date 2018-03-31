using ImageService.Enums;
using ImageService.Logger.Model;

namespace ImageService.Controller
{
    public interface IImageController
    {
        string ExecuteCommand(CommandEnum commandID, string[] args, out MessageTypeEnum result); 
    }
}
