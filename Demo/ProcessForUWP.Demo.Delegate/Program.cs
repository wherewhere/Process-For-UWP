using AAPTForNet;
using AAPTForNet.Models;
using AdbApkInstallerUWP.Core.Models;
using Newtonsoft.Json;
using ProcessForUWP.Desktop;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace AdbApkInstallerUWP.Delegate
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Communication.InitializeAppServiceConnection();

            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }
}
