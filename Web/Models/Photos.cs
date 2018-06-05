using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Communication;
using Infrastructure;
using Infrastructure.Event;

namespace Web.Models
{
    public class Photos
    {
        [DataType(DataType.Text)]
        [Display(Name = "Thumbnails")]
        public ObservableCollection<string> Thumbnails { get; set; }

        private bool _recievedOutputDirectory;
        public string OutputDirectory;


        public Photos()
        {
            Thumbnails = new ObservableCollection<string>();
            _recievedOutputDirectory = false;
            GuiTcpClientSingleton.Instance.Close();

            if (!Utils.IsServiceActive("ImageService"))
            {
                return;
            }

            GuiTcpClientSingleton.Instance.ConfigurationReceived += OnConfigurationsReceived;

            // TODO Can we do better? Probably. Do we want to do better? No. Will we do better? Maybe.
            Task.Run(() =>
            {
                for (int i = 0; i < 20; i++)
                    if (!_recievedOutputDirectory)
                        Thread.Sleep(250);
                    else
                        break;
            }).Wait();

            DirectoryInfo directoryName;

            try
            {
                directoryName = new DirectoryInfo(OutputDirectory);
            }
            catch (Exception)
            {
                return;
            }

            foreach (FileInfo fi in directoryName.GetFiles("*.jpg", SearchOption.AllDirectories))
            {
                Thumbnails.Add(fi.FullName);
            }
        }

        private void OnConfigurationsReceived(object sender, ConfigurationReceivedEventArgs e)
        {
            SettingsInfo settingsInfo = SettingsInfo.FromJson(e.Args);
            OutputDirectory = settingsInfo.OutputDirectory;
            _recievedOutputDirectory = true;
        }
    }
}