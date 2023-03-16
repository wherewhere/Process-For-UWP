﻿using Microsoft.Toolkit.Uwp.Helpers;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
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

        // Keep reference so it does not get optimized/garbage collected
        public static UISettings UISettings;

        /// <summary>
        /// Gets the current actual theme of the app based on the requested theme of the
        /// root element, or if that value is Default, the requested theme of the Application.
        /// </summary>
        public static ElementTheme ActualTheme
        {
            get
            {
                ElementTheme? value = null;
                if (CurrentApplicationWindow?.Dispatcher?.HasThreadAccess == true)
                {
                    if (CurrentApplicationWindow?.Content is FrameworkElement rootElement
                        && rootElement.RequestedTheme != ElementTheme.Default)
                    {
                        return rootElement.RequestedTheme;
                    }
                }
                else
                {
                    value = UIHelper.AwaitByTaskCompleteSource(() =>
                        CurrentApplicationWindow?.Dispatcher?.AwaitableRunAsync(() =>
                        {
                            return CurrentApplicationWindow?.Content is FrameworkElement rootElement
                                && rootElement.RequestedTheme != ElementTheme.Default
                                ? rootElement.RequestedTheme
                                : (ElementTheme?)null;
                        }, CoreDispatcherPriority.High));
                }
                return value ?? SettingsHelper.Get<ElementTheme>(SettingsHelper.SelectedAppTheme);
            }
        }

        /// <summary>
        /// Gets or sets (with LocalSettings persistence) the RequestedTheme of the root element.
        /// </summary>
        public static ElementTheme RootTheme
        {
            get
            {
                ElementTheme? value = null;
                foreach (Window window in WindowHelper.ActiveWindows)
                {
                    if (window.Dispatcher.HasThreadAccess)
                    {
                        if (window?.Content is FrameworkElement rootElement)
                        {
                            value = rootElement.RequestedTheme;
                        }
                    }
                    else
                    {
                        value = UIHelper.AwaitByTaskCompleteSource(() =>
                            window.Dispatcher.AwaitableRunAsync(() =>
                            {
                                return window?.Content is FrameworkElement rootElement ? rootElement.RequestedTheme : (ElementTheme?)null;
                            }, CoreDispatcherPriority.High));
                    }
                    if (value.HasValue) { return value.Value; }
                }
                return ElementTheme.Default;
            }
            set
            {
                foreach (Window window in WindowHelper.ActiveWindows)
                {
                    _ = window.Dispatcher.AwaitableRunAsync(() =>
                    {
                        if (window?.Content is FrameworkElement rootElement)
                        {
                            rootElement.RequestedTheme = value;
                        }
                    });
                }

                SettingsHelper.Set(SettingsHelper.SelectedAppTheme, value);
                UpdateSystemCaptionButtonColors();
            }
        }

        public static void Initialize()
        {
            // Save reference as this might be null when the user is in another app
            CurrentApplicationWindow = Window.Current ?? App.MainWindow;
            RootTheme = SettingsHelper.Get<ElementTheme>(SettingsHelper.SelectedAppTheme);

            // Registering to color changes, thus we notice when user changes theme system wide
            UISettings = new UISettings();
            UISettings.ColorValuesChanged += UISettings_ColorValuesChanged;
        }

        public static void Initialize(Window window)
        {
            if (window?.Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = ActualTheme;
            }
            UpdateSystemCaptionButtonColors(window);
        }

        private static void UISettings_ColorValuesChanged(UISettings sender, object args)
        {
            UpdateSystemCaptionButtonColors();
        }

        public static bool IsDarkTheme()
        {
            return Window.Current != null
                ? ActualTheme == ElementTheme.Default
                    ? Application.Current.RequestedTheme == ApplicationTheme.Dark
                    : ActualTheme == ElementTheme.Dark
                : ActualTheme == ElementTheme.Default
                    ? UISettings.GetColorValue(UIColorType.Background) == Colors.Black
                    : ActualTheme == ElementTheme.Dark;
        }

        public static bool IsDarkTheme(ElementTheme ActualTheme)
        {
            return Window.Current != null
                ? ActualTheme == ElementTheme.Default
                    ? Application.Current.RequestedTheme == ApplicationTheme.Dark
                    : ActualTheme == ElementTheme.Dark
                : ActualTheme == ElementTheme.Default
                    ? UISettings.GetColorValue(UIColorType.Background) == Colors.Black
                    : ActualTheme == ElementTheme.Dark;
        }

        public static void UpdateSystemCaptionButtonColors()
        {
            bool IsDark = IsDarkTheme();
            bool IsHighContrast = new AccessibilitySettings().HighContrast;

            Color ForegroundColor = IsDark || IsHighContrast ? Colors.White : Colors.Black;
            Color BackgroundColor = IsHighContrast ? Color.FromArgb(255, 0, 0, 0) : IsDark ? Color.FromArgb(255, 32, 32, 32) : Color.FromArgb(255, 243, 243, 243);

            foreach (Window window in WindowHelper.ActiveWindows)
            {
                _ = window.Dispatcher.AwaitableRunAsync(() =>
                {
                    if (UIHelper.HasStatusBar)
                    {
                        StatusBar StatusBar = StatusBar.GetForCurrentView();
                        StatusBar.ForegroundColor = ForegroundColor;
                        StatusBar.BackgroundColor = BackgroundColor;
                        StatusBar.BackgroundOpacity = 0; // 透明度
                    }
                    else
                    {
                        bool ExtendViewIntoTitleBar = CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar;
                        ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
                        TitleBar.ForegroundColor = TitleBar.ButtonForegroundColor = ForegroundColor;
                        TitleBar.BackgroundColor = TitleBar.InactiveBackgroundColor = BackgroundColor;
                        TitleBar.ButtonBackgroundColor = TitleBar.ButtonInactiveBackgroundColor = ExtendViewIntoTitleBar ? Colors.Transparent : BackgroundColor;
                    }
                });
            }
        }

        public static void UpdateSystemCaptionButtonColors(Window window)
        {
            _ = window.Dispatcher.AwaitableRunAsync(() =>
            {
                bool IsDark = window?.Content is FrameworkElement rootElement ? IsDarkTheme(rootElement.RequestedTheme) : IsDarkTheme();
                bool IsHighContrast = new AccessibilitySettings().HighContrast;

                Color ForegroundColor = IsDark || IsHighContrast ? Colors.White : Colors.Black;
                Color BackgroundColor = IsHighContrast ? Color.FromArgb(255, 0, 0, 0) : IsDark ? Color.FromArgb(255, 32, 32, 32) : Color.FromArgb(255, 243, 243, 243);

                if (UIHelper.HasStatusBar)
                {
                    StatusBar StatusBar = StatusBar.GetForCurrentView();
                    StatusBar.ForegroundColor = ForegroundColor;
                    StatusBar.BackgroundColor = BackgroundColor;
                    StatusBar.BackgroundOpacity = 0; // 透明度
                }
                else
                {
                    bool ExtendViewIntoTitleBar = CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar;
                    ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
                    TitleBar.ForegroundColor = TitleBar.ButtonForegroundColor = ForegroundColor;
                    TitleBar.BackgroundColor = TitleBar.InactiveBackgroundColor = BackgroundColor;
                    TitleBar.ButtonBackgroundColor = TitleBar.ButtonInactiveBackgroundColor = ExtendViewIntoTitleBar ? Colors.Transparent : BackgroundColor;
                }
            });
        }
    }
}
