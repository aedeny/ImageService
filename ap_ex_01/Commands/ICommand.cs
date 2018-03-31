using ImageService.Logger.Model;

namespace ImageService.Commands
{
    public interface ICommand
    {
        string Execute(string[] args, out MessageTypeEnum result);
    }
}
