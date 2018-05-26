using System.Net.Sockets;

namespace ImageService.Communication
{
    public class NewClientConnectedEventArgs
    {
        public NetworkStream Stream { get; set; }
    }
}