using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Infrastructure.Enums;
using Infrastructure.Event;
using Infrastructure.Logging;

namespace ImageServiceGUI
{
    internal class OurTcpClientSingleton
    {
        private static OurTcpClientSingleton _instance;
        private TcpClient _client;

        private IPEndPoint _ep;
        //private NetworkStream _stream;

        private OurTcpClientSingleton()
        {
            Start();
        }

        public BinaryWriter Writer { get; set; }
        public BinaryReader Reader { get; set; }

        public static OurTcpClientSingleton Instance => _instance ?? (_instance = new OurTcpClientSingleton());
        public event EventHandler LogMsgRecieved;
        public event EventHandler<DirectoryHandlerClosedEventArgs> DirectoryHandlerRemoved;
        public event EventHandler<ConfigurationReceivedEventArgs> ConfigurationReceived;

        // TODO Put in Task?
        public void Start()
        {
            _ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            _client = new TcpClient();
            _client.Connect(_ep);

            Debug.WriteLine(@"TcpClient Connected");
            NetworkStream stream = _client.GetStream();
            Reader = new BinaryReader(stream);
            Writer = new BinaryWriter(stream);

            new Task(() =>
            {
                try
                {
                    while (true)
                    {
                        string commandLine = Reader.ReadString();
                        ParseMessage(commandLine);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.StackTrace);
                }
            }).Start();
        }

        /// <summary>
        ///     Parse the msg recieved from the server.
        /// </summary>
        /// <param name="msg">The message to parse.</param>
        public void ParseMessage(string msg)
        {
            string[] parameters = msg.Split(';');
            CommandEnum command = (CommandEnum) int.Parse(parameters[0]);
            switch (command)
            {
                case CommandEnum.NewLogCommand:
                    Log(parameters[1], (MessageTypeEnum) int.Parse(parameters[2]));
                    break;
                case CommandEnum.CloseDirectoryHandlerCommand:
                    DirectoryHandlerClosedEventArgs dhceArgs = new DirectoryHandlerClosedEventArgs(parameters[1], "hmm");
                    DirectoryHandlerRemoved?.Invoke(this, dhceArgs);
                    break;
                case CommandEnum.NewFileCommand:
                    break;
                case CommandEnum.ConfigCommand:
                    ConfigurationReceivedEventArgs creArgs = new ConfigurationReceivedEventArgs(parameters.Skip(1).ToString());
                    ConfigurationReceived?.Invoke(this, creArgs);
                    break;
                case CommandEnum.LogHistoryCommand:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        /// <summary>
        ///     Sends the LogViewModel a log msg via event.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        /// <param name="messageType">Message type.</param>
        public void Log(string msg, MessageTypeEnum messageType)
        {
            MessageRecievedEventArgs messageRecievedEventArgs = new MessageRecievedEventArgs
            {
                Message = msg,
                Status = messageType
            };
            LogMsgRecieved?.Invoke(this, messageRecievedEventArgs);
        }


        /// <summary>
        ///     Closes the TCP Client.
        /// </summary>
        public void Close()
        {
            _client.Close();
        }

        public List<Tuple<MessageTypeEnum, string>> GetLogList()
        {
            // Dummy log list
            return new List<Tuple<MessageTypeEnum, string>>
            {
                new Tuple<MessageTypeEnum, string>(MessageTypeEnum.Warning,
                    "A long time ago in a galaxy far, far away...."),
                new Tuple<MessageTypeEnum, string>(MessageTypeEnum.Info, "It is a period of civil war."),
                new Tuple<MessageTypeEnum, string>(MessageTypeEnum.Failure,
                    "Rebel spaceships, striking from a hidden base, have won their first victory against the evil Galactic Empire."),
                new Tuple<MessageTypeEnum, string>(MessageTypeEnum.Warning,
                    "During the battle, Rebel spies managed to steal secret plans to the Empire’s ultimate weapon, the DEATH STAR, an armored space station with enough power to destroy an entire planet."),
                new Tuple<MessageTypeEnum, string>(MessageTypeEnum.Info,
                    "Pursued by the Empire’s sinister agents, Princess Leia races home aboard her starship, custodian of the stolen plans that can save her people and restore freedom to the galaxy….")
            };
        }
    }
}