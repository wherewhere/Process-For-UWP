using Microsoft.UI.Xaml.Controls;
using ProcessForUWP.Demo.Pages;
using ProcessForUWP.Demo.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using FontIconSource = Microsoft.UI.Xaml.Controls.FontIconSource;

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
            Window.Current.SetTitleBar(CustomDragRegion);
        }

        private void TabView_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is TabView tabView)
            {
                tabView.TabItems.Add(CreateNewTab(0, "cmd"));
                tabView.SelectedIndex = 0;
            }
        }

        private async void TabView_AddTabButtonClick(TabView sender, object args)
        {
            string path = await PickProcess();
            if (!string.IsNullOrWhiteSpace(path))
            {
                sender.TabItems.Add(CreateNewTab(sender.TabItems.Count, path));
                sender.SelectedIndex += 1;
            }
        }

        private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
        }

        private async Task<string> PickProcess()
        {
            FileOpenPicker FileOpen = new();
            FileOpen.FileTypeFilter.Add(".exe");
            FileOpen.SuggestedStartLocation = PickerLocationId.ComputerFolder;

            StorageFile file = await FileOpen.PickSingleFileAsync();
            return file?.Path;
        }

        private TabViewItem CreateNewTab(int index, string path)
        {
            TabViewItem newItem = new()
            {
                Header = $"ProcessEx {index}",
                IconSource = new FontIconSource
                {
                    Glyph = "\uE7C3",
                    FontFamily = (FontFamily)Application.Current.Resources["SymbolThemeFontFamily"]
                }
            };

            // The content of the tab is often a frame that contains a page, though it could be any UIElement.
            Frame frame = new();

            frame.Navigate(typeof(TerminalPage), new TerminalViewModel(path, newItem));

            newItem.Content = frame;

            return newItem;
        }
    }
}
