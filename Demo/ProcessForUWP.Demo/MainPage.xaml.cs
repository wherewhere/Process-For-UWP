using Newtonsoft.Json;
using ProcessForUWP.UWP.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace ProcessForUWP.Demo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            GetProcess();
        }

        private async void GetProcess()
        {
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = @"C:\Users\qq251\Downloads\Github\APK-Installer-UWP\APKInstaller\APKInstaller\platform-tools\adb.exe",
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            UWP.Process process = await Task.Run(() => { return new UWP.Process(); });
            process.Start(info);
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.OutputDataReceived += OnOutputDataReceived;
        }

        private void OnOutputDataReceived(UWP.Process sender, UWP.DataReceivedEventArgs e)
        {
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Text.Text += $"{e.Data}\n");
        }
    }
}
