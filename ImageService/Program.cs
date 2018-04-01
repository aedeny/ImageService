using System.ServiceProcess;

namespace ap_ex_01
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ImageService.ImageService(args)
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
