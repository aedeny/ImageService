using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Enums;
using Infrastructure.Event;
using Infrastructure.Logging;

namespace ImageServiceGUI
{
    internal class GuiTcpClientSingleton
    {
        private static GuiTcpClientSingleton _instance;
        private TcpClient _client;
        private IPEndPoint _ep;

        private GuiTcpClientSingleton()
        {
            Connected = false;
            ConnectToService();
        }

        public BinaryWriter Writer { get; set; }
        public BinaryReader Reader { get; set; }

        public static GuiTcpClientSingleton Instance => _instance ?? (_instance = new GuiTcpClientSingleton());
        public bool Connected { get; private set; }

        public event EventHandler<MessageRecievedEventArgs> LogMessageRecieved;
        public event EventHandler ConnectedToService;
        public event EventHandler<DirectoryHandlerClosedEventArgs> DirectoryHandlerRemoved;
        public event EventHandler<ConfigurationReceivedEventArgs> ConfigurationReceived;

        // TODO Put in Task?
        public void ConnectToService()
        {
            _ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            _client = new TcpClient();

            Task c = Task.Run(() =>
            {
                while (!Connected)
                {
                    Thread.Sleep(100);
                    try
                    {
                        _client.Connect(_ep);
                        Connected = true;
                    }
                    catch (SocketException) { }
                }

                ConnectedToService?.Invoke(this, null);
                Listen();
            });
        }

        private void Listen()
        {
            Debug.WriteLine(@"TcpClient Connected");
            NetworkStream stream = _client.GetStream();
            Reader = new BinaryReader(stream);
            Writer = new BinaryWriter(stream);

            new Task(() =>
            {
                Thread.Sleep(1000);
                try
                {
                    while (true)
                    {
                        string commandLine = Reader.ReadString();
                        Debug.WriteLine("Got: " + commandLine);
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
            string[] parameters = msg.Split('|');
            CommandEnum command = (CommandEnum) Enum.Parse(typeof(CommandEnum), parameters[0]);

            switch (command)
            {
                case CommandEnum.NewLogCommand:
                    Log(parameters[1], (EventLogEntryType) Enum.Parse(typeof(EventLogEntryType), parameters[2]));
                    break;
                case CommandEnum.CloseDirectoryHandlerCommand:
                    DirectoryHandlerClosedEventArgs
                        dhceArgs = new DirectoryHandlerClosedEventArgs(parameters[1], "hmm");

                    DirectoryHandlerRemoved?.Invoke(this, dhceArgs);
                    break;
                case CommandEnum.NewFileCommand:
                    break;
                case CommandEnum.ConfigCommand:
                    ConfigurationReceivedEventArgs creArgs = new ConfigurationReceivedEventArgs(parameters[1]);
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
        public void Log(string msg, EventLogEntryType messageType)
        {
            MessageRecievedEventArgs messageRecievedEventArgs = new MessageRecievedEventArgs
            {
                Message = msg,
                EventLogEntryType = messageType
            };

            LogMessageRecieved?.Invoke(this, messageRecievedEventArgs);
        }


        /// <summary>
        ///     Closes the TCP Client.
        /// </summary>
        public void Close()
        {
            _client.Close();
        }

        public List<Tuple<EventLogEntryType, string>> GetLogList()
        {
            // Dummy log list
            return new List<Tuple<EventLogEntryType, string>>
            {
                new Tuple<EventLogEntryType, string>(EventLogEntryType.Warning,
                    "A long time ago in a galaxy far, far away...."),
                new Tuple<EventLogEntryType, string>(EventLogEntryType.Information, "It is a period of civil war."),
                new Tuple<EventLogEntryType, string>(EventLogEntryType.Error,
                    "Rebel spaceships, striking from a hidden base, have won their first victory against the evil Galactic Empire."),
                new Tuple<EventLogEntryType, string>(EventLogEntryType.Warning,
                    "During the battle, Rebel spies managed to steal secret plans to the Empire’s ultimate weapon, the DEATH STAR, an armored space station with enough power to destroy an entire planet."),
                new Tuple<EventLogEntryType, string>(EventLogEntryType.Information,
                    "Pursued by the Empire’s sinister agents, Princess Leia races home aboard her starship, custodian of the stolen plans that can save her people and restore freedom to the galaxy….")
            };
        }
    }
}