using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace ProcessForUWP.Demo.Helpers
{
    /// <summary>
    /// Class providing functionality around switching and restoring theme settings
    /// </summary>
    public static class ThemeHelper
    {
        private static Window CurrentApplicationWindow;

        public static bool IsStatusBarSupported { get; } = ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar");

        // Keep reference so it does not get optimized/garbage collected
        public static UISettings UISettings { get; } = new UISettings();
        public static AccessibilitySettings AccessibilitySettings { get; } = new AccessibilitySettings();

        #region RootTheme

        public static Task<ElementTheme> GetRootThemeAsync() =>
            GetRootThemeAsync(Window.Current ?? CurrentApplicationWindow);

        public static async Task<ElementTheme> GetRootThemeAsync(Window window)
        {
            if (window == null)
            {
                return ElementTheme.Default;
            }

            await window.Dispatcher.ResumeForegroundAsync();

            return window.Content is FrameworkElement rootElement
                ? rootElement.RequestedTheme
                : ElementTheme.Default;
        }

        public static async Task SetRootThemeAsync(ElementTheme value)
        {
            await Task.WhenAll(WindowHelper.ActiveWindows.Select(async window =>
            {
                await window.Dispatcher.ResumeForegroundAsync();

                if (window.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = value;
                }
            }));

            UpdateSystemCaptionButtonColors();
        }

        #endregion

        static ThemeHelper()
        {
            // Registering to color changes, thus we notice when user changes theme system wide
            UISettings.ColorValuesChanged += UISettings_ColorValuesChanged;
        }

        public static async void Initialize(Window window)
        {
            if (window == null) { return; }
            // Save reference as this might be null when the user is in another app
            if (CurrentApplicationWindow == null)
            { CurrentApplicationWindow = window; }
            if (window?.Content is FrameworkElement rootElement)
            { rootElement.RequestedTheme = await GetRootThemeAsync(CurrentApplicationWindow); }
            UpdateSystemCaptionButtonColors(window);
        }

        private static void UISettings_ColorValuesChanged(UISettings sender, object args)
        {
            UpdateSystemCaptionButtonColors();
        }

        public static Task<bool> IsDarkThemeAsync() => GetRootThemeAsync().ContinueWith(x => IsDarkTheme(x.Result));

        public static bool IsDarkTheme(ElementTheme actualTheme)
        {
            return Window.Current != null
                ? actualTheme == ElementTheme.Default
                    ? Application.Current.RequestedTheme == ApplicationTheme.Dark
                    : actualTheme == ElementTheme.Dark
                : actualTheme == ElementTheme.Default
                    ? UISettings?.GetColorValue(UIColorType.Foreground).IsColorLight() == true
                    : actualTheme == ElementTheme.Dark;
        }

        public static bool IsColorLight(this Color color) => ((5 * color.G) + (2 * color.R) + color.B) > (8 * 128);

        public static void UpdateExtendViewIntoTitleBar(bool isExtendsTitleBar)
        {
            WindowHelper.ActiveWindows.ForEach(async window =>
            {
                await window.Dispatcher.ResumeForegroundAsync();

                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = isExtendsTitleBar;
            });
        }

        public static async void UpdateSystemCaptionButtonColors()
        {
            bool isDark = await IsDarkThemeAsync();
            bool isHighContrast = AccessibilitySettings.HighContrast;

            Color foregroundColor = isDark || isHighContrast ? Colors.White : Colors.Black;
            Color backgroundColor = isHighContrast ? Color.FromArgb(255, 0, 0, 0) : isDark ? Color.FromArgb(255, 32, 32, 32) : Color.FromArgb(255, 243, 243, 243);

            WindowHelper.ActiveWindows.ForEach(async window =>
            {
                await window.Dispatcher.ResumeForegroundAsync();

                if (IsStatusBarSupported)
                {
                    StatusBar statusBar = StatusBar.GetForCurrentView();
                    statusBar.ForegroundColor = foregroundColor;
                    statusBar.BackgroundColor = backgroundColor;
                    statusBar.BackgroundOpacity = 0; // 透明度
                }
                else
                {
                    bool extendViewIntoTitleBar = CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar;
                    ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
                    titleBar.ForegroundColor = titleBar.ButtonForegroundColor = foregroundColor;
                    titleBar.BackgroundColor = titleBar.InactiveBackgroundColor = backgroundColor;
                    titleBar.ButtonBackgroundColor = titleBar.ButtonInactiveBackgroundColor = extendViewIntoTitleBar ? Colors.Transparent : backgroundColor;
                }
            });
        }

        public static async void UpdateSystemCaptionButtonColors(Window window)
        {
            await window.Dispatcher.ResumeForegroundAsync();

            bool isDark = window?.Content is FrameworkElement rootElement ? IsDarkTheme(rootElement.RequestedTheme) : await IsDarkThemeAsync();
            bool isHighContrast = AccessibilitySettings.HighContrast;

            Color foregroundColor = isDark || isHighContrast ? Colors.White : Colors.Black;
            Color backgroundColor = isHighContrast ? Color.FromArgb(255, 0, 0, 0) : isDark ? Color.FromArgb(255, 32, 32, 32) : Color.FromArgb(255, 243, 243, 243);

            if (IsStatusBarSupported)
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                statusBar.ForegroundColor = foregroundColor;
                statusBar.BackgroundColor = backgroundColor;
                statusBar.BackgroundOpacity = 0; // 透明度
            }
            else
            {
                bool extendViewIntoTitleBar = CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar;
                ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
                titleBar.ForegroundColor = titleBar.ButtonForegroundColor = foregroundColor;
                titleBar.BackgroundColor = titleBar.InactiveBackgroundColor = backgroundColor;
                titleBar.ButtonBackgroundColor = titleBar.ButtonInactiveBackgroundColor = extendViewIntoTitleBar ? Colors.Transparent : backgroundColor;
            }
        }
    }
}
