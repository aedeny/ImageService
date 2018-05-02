using Infrastructure.Logging;

namespace ImageService.Commands
{
    public interface ICommand
    {
        // Executes the corresponding method.
        string Execute(string[] args, out MessageTypeEnum result);
    }
}