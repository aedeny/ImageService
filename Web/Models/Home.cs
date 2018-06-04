using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using Infrastructure;
using Newtonsoft.Json;

namespace Web.Models
{
    public class Home
    {
        public Home()
        {
            StudentsInfoRoot = LoadStudentsInfoFromFile(@"C:\Users\edeny\Documents\ex01\details.txt");
            StudentsInfoRoot.StudentsInfo.Sort((x, y) => string.CompareOrdinal(x.FirstName, y.FirstName));
            Debug.WriteLine("Finished Home Constructor");
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
    }
}