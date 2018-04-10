using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CloudFSVisualizer.Model;
using Windows.UI.Core;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace CloudFSVisualizer.Assets
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AppShell : Page
    {
        public static AppShell Current { get; set; }
        private static string LastNotifiedMessage { get; set; }
        public List<NavigationViewItem> Pages { get; set; }
        public AppShell()
        {
            this.InitializeComponent();
            Current = this;
            Pages = new List<NavigationViewItem>()
            {
                new NavigationViewItem{Content = "HDFS Servers", Tag = typeof(HDFSServerPage), Icon = new FontIcon{ Glyph = "\xE130"} },
                new NavigationViewItem{Content = "Yarn Servers", Tag = typeof(YarnServerPage), Icon = new FontIcon{ Glyph = "\xE130"} }
            };
            AppContentFrame.Navigated += AppContentFrame_Navigated;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
            AppContentFrame.CanGoBack ?
            AppViewBackButtonVisibility.Visible :
            AppViewBackButtonVisibility.Collapsed;
        }

        public void NotifyMessage(string message)
        {
            if (LastNotifiedMessage != message)
            {
                LastNotifiedMessage = message;
                InAppNotification.Show(message);
            }
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (AppContentFrame.CanGoBack)
            {
                e.Handled = true;
                AppContentFrame.GoBack();
            }
        }

        private void AppContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                ((Frame)sender).CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;
        }

        private void AppShellNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                AppContentFrame.Navigate(typeof(SettingPage));
            }
            else
            {
                var selectedItem = args.SelectedItem as NavigationViewItem;
                AppContentFrame.Navigate(selectedItem.Tag as Type);
                AppShellNavigationView.Header = selectedItem.Content;
            }
        }

        private void AppContentFrame_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AppShellNavigationView.IsPaneOpen = false;
        }
    }
}
