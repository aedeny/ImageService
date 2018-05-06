using ImageService.Model.Event;
using ImageService.Server;
using Infrastructure.Logging;


namespace ImageService.Commands
{
    class CloseHandlerCommand : ICommand
    {
        private readonly ImageServer _imageServer;

        public CloseHandlerCommand(ImageServer imageServer)
        {
            _imageServer = imageServer;
        }

        public string Execute(string[] args, out MessageTypeEnum result)
        {
            DirectoryCloseEventArgs directoryCloseEventArgs = new DirectoryCloseEventArgs(args[0], "Close");
            return _imageServer.CloseHandler(directoryCloseEventArgs, out result);
        }
    }
}
