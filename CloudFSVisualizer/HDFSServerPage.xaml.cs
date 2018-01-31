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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CloudFSVisualizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HDFSServerPage : Page
    {
        public List<HDFSServer> ServerList { get; set; }
        public HDFSServerPage()
        {
            this.InitializeComponent();
            ServerList = new List<HDFSServer>();
            var hadoopServer = new HDFSServer()
            {
                SType = Server.ServerType.HDFSServer,
                MasterNode = new HDFSMasterNode
                {
                    Host = "172.18.84.45",
                    Description = "master"

                },
                SlaveNode = new List<HDFSSlaverNode>()
                {
                    new HDFSSlaverNode
                    {
                        Host = "172.18.84.37",
                        Description = "slave"
                    },
                     new HDFSSlaverNode
                    {
                        Host = "172.18.84.44",
                        Description = "slave"
                    }
                }
            };
            GetServer();
            //ServerList.Add(hadoopServer);
            //ServerManager.StoreHDFSServerListToFileAsync(ServerList);
            //var status = HDFSFileManager.GetFileStatus(new HDFSFile
            //{
            //    ServerHost = @"172.18.84.45:50070",
            //    Path = @"/user/hadoop/files/testFile/LICENSE"
            //});
        }
        public async void GetServer()
        {
            ServerList = await ServerManager.GetHDFSServerListFromFileAsync();
        }

        private void ServerListGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as HDFSServer;
            Frame.Navigate(typeof(HDFSServerDetailPage), item);
        }
    }
}
