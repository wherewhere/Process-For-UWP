using Microsoft.Toolkit.Uwp;
using ProcessForUWP.UWP;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.System;

namespace ProcessForUWP.Demo.ViewModels
{
    public class TerminalViewModel : INotifyPropertyChanged
    {
        private readonly string _path;
        private ProcessEx _process;

        public DispatcherQueue DispatcherQueue { get; private set; }

        private string _outputData = string.Empty;
        public string OutputData
        {
            get => _outputData;
            set
            {
                _outputData = value;
                RaisePropertyChangedEvent();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangedEvent([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            if (name != null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
        }

        public TerminalViewModel(string path, DispatcherQueue dispatcher)
        {
            _path = path;
            DispatcherQueue = dispatcher;
        }

        public async Task Refresh()
        {
            if (string.IsNullOrEmpty(OutputData))
            {
                ProcessStartInfo info = new()
                {
                    FileName = _path,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                _process = await Task.Run(() => new ProcessEx { StartInfo = info });
                _process.Start();
                _process.BeginErrorReadLine();
                _process.BeginOutputReadLine();
                _process.OutputDataReceived += OnOutputDataReceived;
            }
            else
            {
                OutputData = string.Empty;
                _process.Kill();
                _process.Close();
                _process.Start();
            }
        }

        private void OnOutputDataReceived(ProcessEx sender, DataReceivedEventArgsEx e)
        {
            _ = DispatcherQueue.EnqueueAsync(() => OutputData += $"{e.Data}\n");
        }
    }
}
