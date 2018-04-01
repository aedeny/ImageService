using System.ServiceProcess;

namespace ImageService
{
    internal static class Program
    {
        private static void Main()
        {
            ServiceBase[] servicesToRun =
            {
                new ImageService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}