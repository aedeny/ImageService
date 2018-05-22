using System.Net.Sockets;

namespace ImageService.Communication
{
    public class ConnectedEventArgs
    {
        public NetworkStream Stream { get; set; }
    }
}