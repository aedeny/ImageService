using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Communication;
using Infrastructure;
using Infrastructure.Event;
using Newtonsoft.Json;

namespace Web.Models
{
    public class Home
    {
        public bool Active;
        public int NumberOfPhotos;
        public string OutputDirectory;
        private bool _recievedOutputDirectory;

        public Home()
        {
            Active = false;
            _recievedOutputDirectory = false;
            GuiTcpClientSingleton.Instance.Close();
            StudentsInfoRoot = LoadStudentsInfoFromFile(@"C:\Users\edeny\Documents\ex01\details.txt");
            StudentsInfoRoot?.StudentsInfo.Sort((x, y) => string.CompareOrdinal(x.FirstName, y.FirstName));

            Active = Utils.IsServiceActive("ImageService");
            if (Active)
            {
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

                NumberOfPhotos = GetNumberOfPhotos(OutputDirectory);
            }
            else
            {
                NumberOfPhotos = -1;
            }
        }

        private void OnConfigurationsReceived(object sender, ConfigurationReceivedEventArgs e)
        {
            SettingsInfo settingsInfo = SettingsInfo.FromJson(e.Args);
            OutputDirectory = settingsInfo.OutputDirectory;
            _recievedOutputDirectory = true;
        }

        private static int GetNumberOfPhotos(string path)
        {
            if (!Directory.Exists(path))
            {
                return 0;
            }

            // Matches all files with these extensions which are not in thumbnails directory.
            string pattern = "^"+path + "\\(?!thumbnails).*.(jpg|jpeg|gif|png|bmp)";
            pattern = pattern.Replace("\\", "\\\\");
            Regex reg = new Regex(pattern);
            return Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(s => reg.IsMatch(s)).ToList().Count;
        }

        [DataType(DataType.Text)]
        [Display(Name = "Students")]
        public StudentsInfoRootObject StudentsInfoRoot { get; set; }

        public StudentsInfoRootObject LoadStudentsInfoFromFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<StudentsInfoRootObject>(json);
            }
        }

        public class StudentsInfoRootObject
        {
            public List<StudentInfo> StudentsInfo { get; set; }
        }
    }
}