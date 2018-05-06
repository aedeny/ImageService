using Infrastructure.Event;
using ImageService.Server;
using Infrastructure.Logging;


namespace ImageService.Commands
{
    class CloseDirectoryHandlerCommand : ICommand
    {
        private readonly ImageServer _imageServer;

        public CloseDirectoryHandlerCommand(ImageServer imageServer)
        {
            _imageServer = imageServer;
        }

        public string Execute(string[] args, out MessageTypeEnum result)
        {
            DirectoryHandlerClosedEventArgs directoryHandlerClosedEventArgs = new DirectoryHandlerClosedEventArgs(args[0], "Close");
            string s = _imageServer.CloseHandler(directoryHandlerClosedEventArgs, out result);
            return directoryHandlerClosedEventArgs.Closed ? s : null;
        }
    }
}
