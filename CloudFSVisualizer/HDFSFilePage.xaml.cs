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
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CloudFSVisualizer
{
    public class PresentData
    {
        public HDFSFile OriginFile;
        public string Name { get; set; }
        public string Type { get; set; }
        public long Length { get; set; }
        public long ModificationTime { get; set; }
        public string Owner { get; set; }

        public async Task InitAsync(HDFSFile file)
        {
            OriginFile = file;
            var status = await file.Status;
            this.Type = status.type;
            this.Length = status.length;
            this.ModificationTime = status.modificationTime;
            this.Owner = status.owner;
            this.Name = file.Path.Split('/').Last();
        }
    }

    public sealed partial class HDFSFilePage : Page, INotifyPropertyChanged
    {
        public HDFSServer CurrentServer { get; set; }
        private List<ServerLocatedBlocks> locatedBlockList;
        public List<ServerLocatedBlocks> LocatedBlockList
        {
            get { return locatedBlockList; }
            set
            {
                locatedBlockList = value;
                OnpropertyChanged("LocatedBlockList");
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
        private List<PresentData> presentList;
        public List<PresentData> PresentList
        {
            get { return presentList; }
            set
            {
                presentList = value;
                OnpropertyChanged("PresentList");
            }
        }
        private LocatedBlock currentBlock;
        public LocatedBlock CurrentBlock
        {
            get { return currentBlock; }
            set
            {
                currentBlock = value;
                OnpropertyChanged("CurrentBlock");
            }
        }

        private HDFSFile currentFolder;
        public HDFSFile CurrentFolder
        {
            get { return currentFolder; }
            set
            {
                currentFolder = value;
                OnpropertyChanged("CurrentFolder");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public HDFSFilePage()
        {
            this.InitializeComponent();
            BlockList = new List<LocatedBlock>();
            locatedBlockList = new List<ServerLocatedBlocks>();
            FileList = new List<HDFSFile>();
            presentList = new List<PresentData>();
        }

        void OnpropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public async void GetRootFolderAsync()
        {
            CurrentFolder = new HDFSFile()
            {
                ServerHost = CurrentServer.MasterNode.Host,
                Path = ""
            };
            FileList = await CurrentFolder.SubFile;
            var pList = new List<PresentData>();
            foreach (var file in FileList)
            {
                var presentFile = new PresentData();
                await presentFile.InitAsync(file);
                pList.Add(presentFile);
            }
            PresentList = pList;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var item = e.Parameter as HDFSServer;
            CurrentServer = item;
            GetRootFolderAsync();
        }

        private async void GoToPathButton_Click(object sender, RoutedEventArgs e)
        {
            await UpdateFolderFromAddressAsync();
        }

        private void CreateDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentBlock = new LocatedBlock();
        }

        private async void UploadFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new HDFSFileUploadContentDialog(CurrentFolder.Path + "/");
            var result = await dialog.ShowAsync();
            if (dialog.PickedFile != null)
            {
                await HDFSFileManager.UploadHDFSFile(CurrentServer, dialog.PickedFile, dialog.RemotePath);
            }
        }

        private void BlockPresenter_BlockTapped(object sender, RoutedEventArgs e)
        {
            if (sender is Grid s)
            {
                CurrentBlock = s.DataContext as LocatedBlock;
                foreach (var item in LocatedBlockList)
                {
                    item.PresentBlock = CurrentBlock;
                }
                OnpropertyChanged("CurrentBlock");
            }
        }

        private void HDFSFileGridView_CurrentItemChanged(object sender, EventArgs e)
        {

        }

        private async void HDFSFileGridView_SelectionChanged(object sender, Telerik.UI.Xaml.Controls.Grid.DataGridSelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count() == 0)
            {
                return;
            }
            var list = new List<ServerLocatedBlocks>();
            var item = e.AddedItems.First() as PresentData;
            var fileItem = item.OriginFile;

            if ((await fileItem.Status).type == "FILE")
            {
                BlockList = (await fileItem.GetBlocksAsync()).locatedBlocks;
                foreach (var block in BlockList)
                {
                    foreach (var location in block.locations)
                    {
                        var serverIndex = list.FindIndex(p => p.HostName == location.hostName);
                        if (serverIndex == -1)
                        {
                            list.Add(new ServerLocatedBlocks
                            {
                                HostName = location.hostName,
                                LocatedBlockList = new List<LocatedBlock>() { block }
                            });
                        }
                        else
                        {
                            list[serverIndex].LocatedBlockList.Add(block);
                        }
                    }
                }
                LocatedBlockList = list;
            }
            else
            {
                CurrentFolder = fileItem;
                FileList = await fileItem.SubFile;
                var pList = new List<PresentData>();
                foreach (var file in FileList)
                {
                    var presentFile = new PresentData();
                    await presentFile.InitAsync(file);
                    pList.Add(presentFile);
                }
                PresentList = pList;
            }
        }

        private async void FolderPathTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                await UpdateFolderFromAddressAsync();
            }
        }

        private async Task UpdateFolderFromAddressAsync()
        {
            var Folder = new HDFSFile()
            {
                ServerHost = CurrentServer.MasterNode.Host,
                Path = FolderPathTextBox.Text
            };
            if (await Folder.SubFile != null)
            {
                FileList = await Folder.SubFile;
                var pList = new List<PresentData>();
                foreach (var file in FileList)
                {
                    var presentFile = new PresentData();
                    await presentFile.InitAsync(file);
                    pList.Add(presentFile);
                }
                PresentList = pList;
            }
        }
    }
}
