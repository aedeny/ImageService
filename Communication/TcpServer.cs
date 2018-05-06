using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public class TcpServer : ITcpServer
    {
        private readonly int _port;
        private TcpListener _listener;
        private ICollection<ITcpClient> _clietns;
        private readonly ITcpClientHandler _ch;

        public TcpServer(int port, ITcpClientHandler ch)
        {
            _port = port;
            _ch = ch;
            _clietns = new List<ITcpClient>();
            Start();
        }


        public void Start()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _port);
            _listener = new TcpListener(ep);

            _listener.Start();
            Console.WriteLine("Waiting for connections...");

            Task task = new Task(() =>
            {
                while (true)
                {
                    try
                    {
                        TcpClient client = _listener.AcceptTcpClient();
                        Console.WriteLine("Got new connection");
                        _ch.HandleClient(client);
                    }
                    catch (SocketException)
                    {
                        break;
                    }
                }

                Console.WriteLine("Server stopped");
            });
            task.Start();
        }

        public void Stop()
        {
            _listener.Stop();
        }
    }
}