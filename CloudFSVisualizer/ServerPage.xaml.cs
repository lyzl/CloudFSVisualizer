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

namespace CloudFSVisualizer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ServerPage : Page
    {
        List<HDFSServer> serverList;
        public ServerPage()
        {
            this.InitializeComponent();
            serverList = new List<HDFSServer>();
            var hadoopServer = new HDFSServer()
            {
                SType = Server.ServerType.HDFSServer,
                MasterNode = new HDFSMasterNode
                {
                    Host = @"http://172.18.84.45:50070",
                    Description = "master"
                     
                },
                SlaveNode = new List<HDFSSlaverNode>()
                {
                    new HDFSSlaverNode
                    {
                        Host = @"http://172.18.84.37:50075",
                        Description = "slave"
                    }
                }
            };
            serverList.Add(hadoopServer);
            var file = new HDFSFile
            {
                ServerHost = @"http://172.18.84.45:50070",
                Path = "/user/hadoop/files/testFile/LICENSE"
            };

            //ServerManager.StoreServerListToFileAsync(serverList);
            //Bindings.Update();
            //GetServerList();
            
        }

        private async void GetServerList()
        {
            //serverList = await ServerManager.GetServerListFromFileAsync();
        }

        private void ServerListGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as HDFSServer;
            Frame.Navigate(typeof(HDFSServerDetailPage),item);
        }
    }
}
