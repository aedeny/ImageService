using System.ServiceProcess;

namespace ImageService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            ServiceBase[] servicesToRun = {
                new ImageService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
