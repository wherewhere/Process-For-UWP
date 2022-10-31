using Microsoft.UI.Xaml.Controls;
using ProcessForUWP.Demo.Helpers;
using ProcessForUWP.Demo.Pages;
using ProcessForUWP.Demo.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
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
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(CustomDragRegion);
            UIHelper.ShellDispatcher = Dispatcher;
            UIHelper.CheckTheme();
        }

        private async void TabView_Loaded(object sender, RoutedEventArgs e)
        {
            string path = await PickProcess();
            (sender as TabView).TabItems.Add(CreateNewTab(0, path));
            (sender as TabView).SelectedIndex = 0;
        }

        private async void TabView_AddTabButtonClick(TabView sender, object args)
        {
            sender.TabItems.Add(CreateNewTab(sender.TabItems.Count, await PickProcess()));
        }

        private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
        }

        private async Task<string> PickProcess()
        {
            FileOpenPicker FileOpen = new FileOpenPicker();
            FileOpen.FileTypeFilter.Add(".exe");
            FileOpen.SuggestedStartLocation = PickerLocationId.ComputerFolder;

            StorageFile file = await FileOpen.PickSingleFileAsync();
            return file != null ? file.Path : null;
        }

        private TabViewItem CreateNewTab(int index, string path)
        {
            TabViewItem newItem = new TabViewItem();

            newItem.Header = $"ProcessEx {index}";
            newItem.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Document };

            // The content of the tab is often a frame that contains a page, though it could be any UIElement.
            Frame frame = new Frame();

            frame.Navigate(typeof(TerminalPage), new TerminalViewModel(path));

            newItem.Content = frame;

            return newItem;
        }
    }
}
