using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Communication;
using Infrastructure;
using Newtonsoft.Json;

namespace Web.Models
{
    public class Home
    {
        public bool Active;

        public Home()
        {
            Active = false;
            StudentsInfoRoot = LoadStudentsInfoFromFile(@"C:\Users\edeny\Documents\ex01\details.txt");
            StudentsInfoRoot.StudentsInfo.Sort((x, y) => string.CompareOrdinal(x.FirstName, y.FirstName));
            Active = IsServiceActive("ImageService");

            try
            {
                using (ServiceController sc = new ServiceController("ImageService"))
                {
                    Active = (sc.Status == ServiceControllerStatus.Running);
                }
            }
            catch (ArgumentException)
            {
                Active = false;
            }
            catch (Win32Exception)
            {
                Active = false;
            }
        }

        [DataType(DataType.Text)]
        [Display(Name = "Students")]
        public StudentsInfoRootObject StudentsInfoRoot { get; set; }

        public StudentsInfoRootObject LoadStudentsInfoFromFile(string path)
        {
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

        public static bool IsServiceActive(string serviceName)
        {
            try
            {
                using (ServiceController sc = new ServiceController(serviceName))
                {
                    return sc.Status == ServiceControllerStatus.Running;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}