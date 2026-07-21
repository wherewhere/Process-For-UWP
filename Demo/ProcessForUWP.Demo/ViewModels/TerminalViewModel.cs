using Microsoft.UI.Xaml.Controls;
using ProcessForUWP.Core;
using ProcessForUWP.Demo.Helpers;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace ProcessForUWP.Demo.ViewModels
{
    public class TerminalViewModel(string path, TabViewItem tab) : INotifyPropertyChanged
    {
        private IProcess _process;

        public CoreDispatcher Dispatcher => tab.Dispatcher;
        public RichTextBlock Block { get; set; }
        public bool IsExited => _process?.HasExited ?? true;
        public string ExitMessage => IsExited ? $"Process exited with code {_process.ExitCode}" : string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        protected async void RaisePropertyChangedEvent([CallerMemberName] string name = null)
        {
            if (name != null)
            {
                await Dispatcher.ResumeForegroundAsync();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        protected async void RaisePropertyChangedEvent(params string[] names)
        {
            if (names?.Length > 0)
            {
                await Dispatcher.ResumeForegroundAsync();
                foreach (string name in names)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                }
            }
        }

        public async Task Refresh()
        {
            if (_process == null)
            {
                IProcessStatic process = ProcessProjectionFactory.ServerManager.ProcessStatic;
                RemoteProcessStartInfo info = new(path)
                {
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
                _process = process.Start(info);
                _process.OutputDataReceived += OnOutputDataReceived;
                _process.ErrorDataReceived += OnErrorDataReceived;
                _process.Exited += OnProcessExited;
                _process.BeginErrorReadLine();
                _process.BeginOutputReadLine();
                _process.EnableRaisingEvents = true;
                await Dispatcher.ResumeForegroundAsync();
                tab.Header = _process.ProcessName;
                RaisePropertyChangedEvent(nameof(IsExited));
            }
            else
            {
                await Dispatcher.ResumeForegroundAsync();
                Block.Blocks.Clear();
                _process.Kill();
                _process.Start();
            }
        }

        public IAsyncAction SendCommandAsync(string command) => _process.StandardInput.WriteLineAsync(command);

        public static bool BoolNegationConverter(bool value) => !value;

        private void OnProcessExited(object sender, IEventArgs e) => RaisePropertyChangedEvent(nameof(IsExited), nameof(ExitMessage));

        private async void OnOutputDataReceived(object sender, CoDataReceivedEventArgs e)
        {
            await Dispatcher.ResumeForegroundAsync();
            Block.Blocks.Add(new Paragraph
            {
                Inlines =
                {
                    new Run
                    {
                        Text = e.Data
                    }
                }
            });
        }

        private async void OnErrorDataReceived(object sender, CoDataReceivedEventArgs e)
        {
            await Dispatcher.ResumeForegroundAsync();
            Block.Blocks.Add(new Paragraph
            {
                Inlines =
                {
                    new Run
                    {
                        Text = e.Data,
                        FontStyle = FontStyle.Italic,
                        Foreground = new SolidColorBrush(Colors.Red)
                    }
                }
            });
        }
    }
}
