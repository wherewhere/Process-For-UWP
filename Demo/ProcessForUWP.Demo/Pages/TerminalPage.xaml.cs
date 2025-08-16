using ProcessForUWP.Demo.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace ProcessForUWP.Demo.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class TerminalPage : Page
    {
        private TerminalViewModel Provider;

        public TerminalPage() => InitializeComponent();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is TerminalViewModel ViewModel)
            {
                Provider = ViewModel;
                ViewModel.Block = Message;
                _ = Provider.Refresh();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) => _ = Provider.SendCommandAsync(Input.Text);
    }
}
