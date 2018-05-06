using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Infrastructure.Enums;
using Infrastructure.Logging;

namespace ImageServiceGUI
{
    class OurTcpClientSingleton
    {
        private static OurTcpClientSingleton _instance;
        public event EventHandler LogMsgRecieved;
        public event EventHandler DirHandlerRemoved;
        private IPEndPoint _ep;
        private TcpClient _client;
        private BinaryWriter _writer;
        private BinaryReader _reader;
        //private NetworkStream _stream;

        private OurTcpClientSingleton()
        {
            Start();
        }

        public static OurTcpClientSingleton Instance => _instance ?? (_instance = new OurTcpClientSingleton());

        // TODO Put in Task?
        public void Start()
        {
            _ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            _client = new TcpClient();
            _client.Connect(_ep);

            Console.WriteLine(@"You are connected");
            NetworkStream stream = _client.GetStream();
            _reader = new BinaryReader(stream);
            _writer = new BinaryWriter(stream);

            _writer.Write("Yes0");
            _writer.Write("Yes1");
            _writer.Write("Yes2");
            _writer.Write("Yes3");
            string readCommand = _reader.ReadString();
            Console.WriteLine(@"Result = {0}", readCommand);
        }

        /// <summary>
        /// Parse the msg recieved from the server.
        /// </summary>
        /// <param name="msg">The message to parse.</param>
        public void ParseMessage(string msg)
        {
            string[] parameters = msg.Split(';');
            CommandEnum command = (CommandEnum) int.Parse(parameters[0]);
            switch (command)
            {
                case CommandEnum.LogCommand:
                    Log(parameters[1], (MessageTypeEnum) int.Parse(parameters[2]));
                    break;
                case CommandEnum.CloseCommand:
                    DirHandlerRemoved?.Invoke(this, EventArgs.Empty);
                    break;
            }

            throw new NotImplementedException();
        }

        /**
         * Asks the server to send the config details which will later be recieved and sent to the SettingsVM by an event.
         */
        public void GetConfigDetails()
        {
            _writer.Write(CommandEnum.GetConfigCommand.ToString());
        }

        /// <summary>
        /// Sends the LogViewModel a log msg via event.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        /// <param name="messageType">Message type.</param>
        public void Log(string msg, MessageTypeEnum messageType)
        {
            MessageRecievedEventArgs messageRecievedEventArgs = new MessageRecievedEventArgs()
            {
                Message = msg,
                Status = messageType
            };
            LogMsgRecieved?.Invoke(this, messageRecievedEventArgs);
        }

        /// <summary>
        /// Sends a messageType to the TCP Server to remove the specified handler and wait for a confirmation.
        /// </summary>
        /// <param name="handledDirectory"></param>
        /// <returns></returns>
        public void RemoveHandler(string handledDirectory)
        {
            string command = CommandEnum.CloseCommand.ToString() + ";" + handledDirectory;
            _writer.Write(command);
        }

        /// <summary>
        /// Closes the TCP Client.
        /// </summary>
        public void Close()
        {
            _client.Close();
        }

        public List<Tuple<MessageTypeEnum, string>> GetLogList()
        {
            // Dummy log list
            return new List<Tuple<MessageTypeEnum, string>>()
            {
                new Tuple<MessageTypeEnum, string>(MessageTypeEnum.Warning,
                    "A long time ago in a galaxy far, far away...."),
                new Tuple<MessageTypeEnum, string>(MessageTypeEnum.Info, "It is a period of civil war."),
                new Tuple<MessageTypeEnum, string>(MessageTypeEnum.Failure,
                    "Rebel spaceships, striking from a hidden base, have won their first victory against the evil Galactic Empire."),
                new Tuple<MessageTypeEnum, string>(MessageTypeEnum.Warning,
                    "During the battle, Rebel spies managed to steal secret plans to the Empire’s ultimate weapon, the DEATH STAR, an armored space station with enough power to destroy an entire planet."),
                new Tuple<MessageTypeEnum, string>(MessageTypeEnum.Info,
                    "Pursued by the Empire’s sinister agents, Princess Leia races home aboard her starship, custodian of the stolen plans that can save her people and restore freedom to the galaxy…."),
            };
        }
    }
}