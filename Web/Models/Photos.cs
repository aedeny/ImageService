using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Communication;
using Infrastructure;
using Infrastructure.Event;

namespace Web.Models
{
    public class Photos
    {
        private bool _recievedOutputDirectory;
        public string OutputDirectory;

        public Photos()
        {
            Active = false;
            Thumbnails = new ObservableCollection<PhotoInfo>();
            _recievedOutputDirectory = false;
            GuiTcpClientSingleton.Instance.Close();

            if (!Utils.IsServiceActive("ImageService"))
            {
                return;
            }

            Active = true;
            GuiTcpClientSingleton.Instance.ConfigurationReceived += OnConfigurationsReceived;

            // TODO Can we do better? Probably. Do we want to do better? No. Will we do better? Maybe.
            Task.Run(() =>
            {
                for (int i = 0; i < 20; i++)
                    if (!_recievedOutputDirectory)
                    {
                        Thread.Sleep(250);
                    }
                    else
                    {
                        break;
                    }
            }).Wait();

            if (!Directory.Exists(OutputDirectory))
            {
                return;
            }

            string pattern = OutputDirectory + "\\thumbnails\\.*.(jpg|jpeg|gif|png|bmp)";
            pattern = pattern.Replace("\\", "\\\\");
            Regex reg = new Regex(pattern);

            List<string> temp = Directory.EnumerateFiles(OutputDirectory, "*.*", SearchOption.AllDirectories)
                .Where(s => reg.IsMatch(s)).ToList();

            foreach (string s in temp) Thumbnails.Add(new PhotoInfo(s));
        }

        public bool Active { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Thumbnails")]
        public ObservableCollection<PhotoInfo> Thumbnails { get; set; }

        private void OnConfigurationsReceived(object sender, ConfigurationReceivedEventArgs e)
        {
            SettingsInfo settingsInfo = SettingsInfo.FromJson(e.Args);
            OutputDirectory = settingsInfo.OutputDirectory;
            _recievedOutputDirectory = true;
        }
    }
}