using ImageService.Server;
using Infrastructure.Event;
using Infrastructure.Logging;

namespace ImageService.Commands
{
    internal class CloseDirectoryHandlerCommand : ICommand
    {
        private readonly ImageServer _imageServer;

        public CloseDirectoryHandlerCommand(ImageServer imageServer)
        {
            _imageServer = imageServer;
        }

        public string Execute(string[] args, out MessageTypeEnum result)
        {
            DirectoryHandlerClosedEventArgs directoryHandlerClosedEventArgs =
                new DirectoryHandlerClosedEventArgs(args[0], "Close");
            string s = _imageServer.CloseHandler(directoryHandlerClosedEventArgs, out result);
            return directoryHandlerClosedEventArgs.Closed ? s : null;
        }
    }
}