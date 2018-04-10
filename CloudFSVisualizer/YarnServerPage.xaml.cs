using CloudFSVisualizer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class YarnServerPage : Page
    {
        public List<YarnServer> ServerList { get; set; }
        public YarnServerPage()
        {
            
            ServerList = new List<YarnServer>();
            var Server = new YarnServer()
            {
                SType = ServerType.HDFSServer,
                Authentication = new HadoopAuthentication("root"),
                ResourceManager = new YarnResourceManager
                {
                    Host = "h1.lingdra.com",
                    User = "root",
                    Pswd = "123",
                    Description = "master",
                    HadoopHomeDirectory = "/usr/local/hadoop/",
                    PrivateKey = ApplicationData.Current.LocalFolder.Path + "/id_rsa_2048",


                },
                NodeManager = new List<YarnNodeManager>()
                {
                    new YarnNodeManager
                    {
                        Host = "h2.lingdra.com",
                        Description = "slave",
                        PrivateKey= ApplicationData.Current.LocalFolder.Path + "/id_rsa_2048"
                    },
                     new YarnNodeManager
                    {
                        Host = "h3.lingdra.com",
                        Description = "slave",
                        PrivateKey= ApplicationData.Current.LocalFolder.Path + "/id_rsa_2048"
                    },
                     new YarnNodeManager
                    {
                        Host = "h4.lingdra.com",
                        Description = "slave",
                        PrivateKey= ApplicationData.Current.LocalFolder.Path + "/id_rsa_2048"
                    }
                }
            };
            //GetServer();
            ServerList.Add(Server);
            ServerList.Add(Server);
            //ServerManager.StoreHDFSServerListToFileAsync(ServerList);
            this.InitializeComponent();
        }

        private void ServerListGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as YarnServer;
            Frame.Navigate(typeof(YarnServerDetailPage), item);
        }

        private void ServerListGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var columns = Math.Ceiling(ActualWidth / 400);
            //(ServerListGridView.ItemsPanelRoot as ItemsWrapGrid).ItemWidth = e.NewSize.Width / columns;
        }
    }
}
