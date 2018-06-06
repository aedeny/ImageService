using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public class Utils
    {
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

        // Credit: https://stackoverflow.com/a/13266776
        public static string AbsoluteToRelativePath(string pathToFile, string referencePath)
        {
            Uri fileUri = new Uri(pathToFile);
            Uri referenceUri = new Uri(referencePath);
            return referenceUri.MakeRelativeUri(fileUri).ToString();
        }
    }
}
