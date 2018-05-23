using System;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Timers;
using ImageService.Commands;
using ImageService.Communication;
using ImageService.Controller;
using ImageService.Logger;
using ImageService.Model;
using ImageService.Server;
using Infrastructure.Enums;
using Infrastructure.Logging;
using Infrastructure;
using System.Net.Sockets;
using System.IO;

namespace ImageService
{
    public enum ServiceState
    {
        ServiceStopped = 0x00000001,
        ServiceStartPending = 0x00000002,
        ServiceStopPending = 0x00000003,
        ServiceRunning = 0x00000004,
        ServiceContinuePending = 0x00000005,
        ServicePausePending = 0x00000006,
        ServicePaused = 0x00000007
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    }

    public partial class ImageService : ServiceBase
    {
        private readonly string _logName = ConfigurationManager.AppSettings["LogName"];

        // Gets info from App.config
        private readonly string _sourceName = ConfigurationManager.AppSettings["SourceName"];

        //private string kaka = ConfigurationManager.
        private IImageController _controller;
        private int _eventId = 1;
        private ImageServer _imageServer;
        private ILoggingService _loggingService;
        private IImageServiceModel _model;
        private TcpServer _tcpServer;
        private SettingsInfo _settingsInfo;
        private NetworkStream _stream;
        private BinaryWriter _writer;

        public ImageService()
        {
            InitializeComponent();
            eventLog = new EventLog();
            if (!EventLog.SourceExists(_sourceName))
                EventLog.CreateEventSource(
                    _sourceName, _logName);

            eventLog.Source = _sourceName;
            eventLog.Log = _logName;
            eventLog.EnableRaisingEvents = true;
        }

        // KFIRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRR
        public void OnEntryWritten(object sender, EntryWrittenEventArgs e)
        {
            _writer.Write(CommandEnum.NewLogCommand + "|" + e.Entry.Message + "|" + e.Entry.EntryType);
        }

        protected override void OnStart(string[] args)
        {
            eventLog.WriteEntry("In OnStart");

            // Sets up a timer to trigger every minute.  
            Timer timer = new Timer
            {
                // 60 seconds
                Interval = 60000
            };
            timer.Elapsed += OnTimer;
            timer.Start();

            // Updates the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus
            {
                dwCurrentState = ServiceState.ServiceStartPending,
                dwWaitHint = 100000
            };
            SetServiceStatus(ServiceHandle, ref serviceStatus);

            // Updates the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.ServiceRunning;
            SetServiceStatus(ServiceHandle, ref serviceStatus);
            _loggingService = new LoggingService();
            _loggingService.MsgRecievedEvent += OnMsgEvent;


            _controller = new ImageController();
            _imageServer = new ImageServer(_controller, _loggingService);

            SetSettingsInfo();

            _model = new ImageServiceModel(_settingsInfo.OutputDirectory, _settingsInfo.ThumbnailSize);

            // CREATING THE TCP SERVER KFIR
            _tcpServer = new TcpServer(8000, _loggingService, new TcpClientHandlerFactory(_controller));
            _tcpServer.Connected += OnConnected;

            _controller.AddCommand(CommandEnum.NewFileCommand, new NewFileCommand(_model));
            _controller.AddCommand(CommandEnum.CloseDirectoryHandlerCommand,
                new CloseDirectoryHandlerCommand(_imageServer));
            _controller.AddCommand(CommandEnum.ConfigCommand, new SettingsInfoRetrievalCommand());
        }

        protected override void OnStop()
        {
            eventLog.WriteEntry("In OnStop");
            _imageServer.Close();
        }

        private void OnMsgEvent(object sender, MessageRecievedEventArgs args)
        {
            eventLog.WriteEntry(args.Message);
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            eventLog.WriteEntry("Monitoring the System", EventLogEntryType.Information, _eventId++);
        }

        private void SetSettingsInfo()
        {
            _settingsInfo = new SettingsInfo
            {
                OutputDirectory = ConfigurationManager.AppSettings["OutputDir"],
                SourceName = ConfigurationManager.AppSettings["SourceName"],
                LogName = ConfigurationManager.AppSettings["LogName"],
                // HandledDir = ConfigurationManager.AppSettings["HandledDir"],
                ThumbnailSize = !int.TryParse(ConfigurationManager.AppSettings["ThumbnailSize"], out int thumbnailSize)
                    ? 100
                    : thumbnailSize
            };

            string handledDirInfo = ConfigurationManager.AppSettings["HandledDir"];
            string[] handeledDirectories = handledDirInfo.Split(';');

            foreach (string handeledDir in handeledDirectories)
            {
                _settingsInfo.HandledDir.Add(handeledDir);
                _imageServer.CreateHandler(handeledDir);
            }
        } 

        public void OnConnected(object sender, ConnectedEventArgs args)
        {
            _stream = args.Stream;
            _writer = new BinaryWriter(_stream);

            string settings = _settingsInfo.ToJson();
            _writer.Write(CommandEnum.ConfigCommand + "|" + settings);
            _writer.Flush();

            eventLog.EntryWritten += OnEntryWritten;
        }
    }
}