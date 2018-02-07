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
using Renci.SshNet;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CloudFSVisualizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HDFSServerDetailPage : Page
    {
        public List<HDFSNode> HDFSNodeList { get; set; }
        public HDFSServerDetailPage()
        {
            this.InitializeComponent();
            
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var item = e.Parameter as HDFSServer;
            HDFSNodeList = new List<HDFSNode>();
            HDFSNodeList.Add(item.MasterNode);
            foreach (var slaveNode in item.SlaveNode)
            {
                HDFSNodeList.Add(slaveNode);
            }
        }

        private void ServerDetailListGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as HDFSNode;
            Frame.Navigate(typeof(HDFSNodePage), item);
        }

        private void HDFSFilePageButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HDFSFilePage));
        }

        private void HDFSConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            //Frame.Navigate(NodeConfigurationPage, )
        }

        private void ServerDetailListGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var columns = Math.Ceiling(ActualWidth / 400);
            (ServerDetailListGridView.ItemsPanelRoot as ItemsWrapGrid).ItemWidth = e.NewSize.Width / columns;
            
        }
    }
}
