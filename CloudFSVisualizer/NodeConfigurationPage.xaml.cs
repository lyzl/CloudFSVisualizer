using CloudFSVisualizer.Model;
using Renci.SshNet;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CloudFSVisualizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NodeConfigurationPage : Page
    {
        public Node CurrentNode { get; set; }
        public SshClient Client { get; set; }
        public NodeConfigurationPage()
        {
            this.InitializeComponent();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            SshCommand sc1 = Client.CreateCommand("sudo reboot");
            sc1.Execute();
            SshCommand sc2 = Client.CreateCommand("hadoop");
            sc2.Execute();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var item = e.Parameter as Node;
            CurrentNode = item;
            //Client = NetworkManager.CreateSSHClinetToNode(CurrentNode);
            Client = new SshClient("172.18.84.34", "hadoop", "hadoop");
            Client.Connect();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Client.Disconnect();
            Client.Dispose();
        }
    }
}
