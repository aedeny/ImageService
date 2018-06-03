using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Communication;
using Infrastructure;
using Infrastructure.Event;

namespace Web.Models
{
    public class Settings
    {
        private string _logName;
        private string _outputDirectory;
        private string _sourceName;

        private bool _gotSettings;

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Log Name")]
        public string LogName
        {
            get => _logName ?? "[Log Name]";
            set => _logName = value;
        }

        [DataType(DataType.Text)]
        [Display(Name = "Output Directory")]
        public string OutputDirectory
        {
            get => _outputDirectory ?? "[Output Directory]";
            set => _outputDirectory = value;
        }

        [DataType(DataType.Text)]
        [Display(Name = "Source Name")]
        public string SourceName
        {
            get => _sourceName ?? "[Source Name]";
            set => _sourceName = value;
        }

        [DataType(DataType.Text)]
        [Display(Name = "Thumbnail Size")]
        public int ThumbnailSize { get; set; }

        public Settings()
        {
            _gotSettings = false;
            if (GuiTcpClientSingleton.Instance.Connected)
            {
                GuiTcpClientSingleton.Instance.Close();
            }

            ThumbnailSize = 0;
            GuiTcpClientSingleton.Instance.ConnectedToService += OnClientConnectedToService;
            GuiTcpClientSingleton.Instance.ConfigurationReceived += OnSettingsInfoReceived;

            // TODO Can we do better? Probably. Do we want to do better? No. Will we do better? Maybe.
            Task.Run(() =>
            {
                for (int i = 0; i < 20; i++)
                {
                    if (!_gotSettings)
                    {
                        Thread.Sleep(250);
                    }
                    else
                    {
                        break;
                    }
                }
            }).Wait();
        }

        private void OnSettingsInfoReceived(object sender, ConfigurationReceivedEventArgs e)
        {
            Debug.WriteLine("OnSettingsInfoReceived");
            SettingsInfo settingsInfo = SettingsInfo.FromJson(e.Args);
            LogName = settingsInfo.LogName;
            SourceName = settingsInfo.SourceName;
            OutputDirectory = settingsInfo.OutputDirectory;
            ThumbnailSize = settingsInfo.ThumbnailSize;
            _gotSettings = true;

            //List<string> handlers = settingsInfo.HandledDirectories;

            //foreach (string handler in handlers)
            //    DirectoryHandlers.Add(handler);
        }

        private void OnClientConnectedToService(object sender, EventArgs e)
        {
            Debug.WriteLine("OnClientConnectedToService");
        }
    }
}