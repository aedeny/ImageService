namespace ImageService.Communication
{
    public interface ITcpClientHandler
    {
        void HandleClient();
        void Write(string s);
    }
}