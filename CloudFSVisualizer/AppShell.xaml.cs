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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace CloudFSVisualizer.Assets
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AppShell : Page
    {
        List<NavigationPage> Pages { get; set; }
        public AppShell()
        {
            this.InitializeComponent();
            Pages = new List<NavigationPage>()
            {
                new NavigationPage(new SymbolIcon(Symbol.Accept),"HDFS Servers",typeof(HDFSServerPage)),
                new NavigationPage(new SymbolIcon(Symbol.Accept),"Yarn Servers",typeof(YarnServerPage)),
                new NavigationPage(new SymbolIcon(Symbol.Accept),"Yarn Servers",typeof(HDFSVisualizationPage))
            };
            Bindings.Update();
        }

        private void AppShellNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                AppContentFrame.Navigate(typeof(SettingPage));
            }
            else
            {
                var selectedItem = args.SelectedItem as NavigationPage;
                AppContentFrame.Navigate(selectedItem.Dest);
                AppShellNavigationView.Header = selectedItem.Desc;
            }
        }
    }
}
