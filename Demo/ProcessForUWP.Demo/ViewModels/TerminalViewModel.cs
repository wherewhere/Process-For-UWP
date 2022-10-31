using ProcessForUWP.Demo.Helpers;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace ProcessForUWP.Demo.ViewModels
{
    public class TerminalViewModel : INotifyPropertyChanged
    {
        private readonly string _path;
        private UWP.ProcessEx _process;

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

        public TerminalViewModel(string Path)
        {
            _path = Path;
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
                _process = await Task.Run(() => { return new UWP.ProcessEx { StartInfo = info }; });
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

        private void OnOutputDataReceived(UWP.ProcessEx sender, UWP.DataReceivedEventArgsEx e)
        {
            _ = UIHelper.ShellDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => OutputData += $"{e.Data}\n");
        }
    }
}
