namespace ImageService.Communication
{
    public class NewClientConnectedEventArgs
    {
        public ITcpClientHandler ClientHandler { get; set; }
    }
}