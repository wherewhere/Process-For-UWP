using Microsoft.UI.Xaml.Controls;
using ProcessForUWP.Core;
using ProcessForUWP.Demo.Helpers;
using ProcessForUWP.UWP;
using System;
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
    public class TerminalViewModel(string path, TabViewItem tab)
    {
        private IProcess _process;

        public CoreDispatcher Dispatcher => tab.Dispatcher;

        public RichTextBlock Block { get; set; }

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
                _process.BeginErrorReadLine();
                _process.BeginOutputReadLine();
                await Dispatcher.ResumeForegroundAsync();
                tab.Header = _process.ProcessName;
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
