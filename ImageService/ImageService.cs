using System;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Timers;
using ImageService.Controller;
using ImageService.Logger;
using ImageService.Logger.Model;
using ImageService.Model;
using ImageService.Server;

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
        private int _eventId = 1;
        private ImageServer _imageServer;
        private IImageServiceModel _model;
        private IImageController _controller;
        private ILoggingService _loggingService;

        // Gets info from App.config
        private readonly string _sourceName = ConfigurationManager.AppSettings["SourceName"];
        private readonly string _logName = ConfigurationManager.AppSettings["LogName"];

        public ImageService()
        {
            InitializeComponent();
            eventLog = new EventLog();
            if (!EventLog.SourceExists(_sourceName))
            {
                EventLog.CreateEventSource(
                    _sourceName, _logName);
            }

            eventLog.Source = _sourceName;
            eventLog.Log = _logName;
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

            // Gets info from App.config
            string outputDir = ConfigurationManager.AppSettings["OutputDir"];
            string handledDir = ConfigurationManager.AppSettings["HandledDir"];
            if (!int.TryParse(ConfigurationManager.AppSettings["ThumbnailSize"], out int thumbnailSize))
            {
                // Sets default thumbnail size
                thumbnailSize = 100;
            }

            _model = new ImageServiceModel(outputDir, thumbnailSize);
            _controller = new ImageController(_model);
            _imageServer = new ImageServer(_controller, _loggingService);
            _imageServer.CreateHandler(handledDir);
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
    }
}