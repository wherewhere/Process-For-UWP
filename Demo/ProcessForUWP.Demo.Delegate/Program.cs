using ProcessForUWP.Desktop.Helpers;
using System.Threading;

namespace AdbApkInstallerUWP.Delegate
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Communication.InitializeAppServiceConnection();

            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }
}
