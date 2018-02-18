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
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using Windows.UI.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CloudFSVisualizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HDFSFilePage : Page, INotifyPropertyChanged
    {
        public HDFSServer CurrentServer { get; set; }

        private Dictionary<string, List<LocatedBlock>> locationDict;
        public Dictionary<string, List<LocatedBlock>> LocationDict {
            get { return locationDict; }
            set
            {
                locationDict = value;
                OnpropertyChanged("LocationDict");
            }
        }

        private List<LocatedBlock> blockList;

        public List<LocatedBlock> BlockList
        {
            get { return blockList; }
            set
            {
                blockList = value;
                OnpropertyChanged("BlockList");
            }
        }
        private List<HDFSFile> fileList;

        public List<HDFSFile> FileList
        {
            get { return fileList; }
            set
            {
                fileList = value;
                OnpropertyChanged("FileList");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public HDFSFilePage()
        {
            this.InitializeComponent();
            BlockList = new List<LocatedBlock>();
            LocationDict = new Dictionary<string, List<LocatedBlock>>();
            FileList = new List<HDFSFile>();
        }

        void OnpropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public async void GetRootFolderAsync()
        {
            var file = new HDFSFile()
            {
                ServerHost = CurrentServer.MasterNode.Host,
                Path = "/"
            };
            FileList = await file.SubFile;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var item = e.Parameter as HDFSServer;
            CurrentServer = item;
            GetRootFolderAsync();
        }

        private async void HDFSFileListView_ItemClickAsync(object sender, ItemClickEventArgs e)
        {
            var dict = new Dictionary<string, List<LocatedBlock>>();
            var item = e.ClickedItem as HDFSFile;

            if ((await item.Status).type == "FILE")
            {
                BlockList = (await item.GetBlocksAsync()).locatedBlocks;
                foreach (var block in BlockList)
                {
                    foreach (var location in block.locations)
                    {
                        if (!dict.ContainsKey(location.hostName))
                        {
                            dict.Add(location.hostName, new List<LocatedBlock>() { block });
                        }
                        else
                        {
                            dict[location.hostName].Add(block);
                        }
                    }
                }
                LocationDict = dict;
            }
            else
            {
                FileList = await item.SubFile;
            }
            //this.Bindings.Update();
        }

    }
}
