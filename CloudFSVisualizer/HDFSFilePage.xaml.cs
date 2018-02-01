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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CloudFSVisualizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HDFSFilePage : Page, INotifyPropertyChanged
    {
        private List<HDFSFile> filelist;

        public List<HDFSFile> FileList
        {
            get { return filelist; }
            set
            {
                filelist = value;
                OnpropertyChanged("file");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public List<int> PresentList { get; set; }
        public HDFSFilePage()
        {
            this.InitializeComponent();
            PresentList = new List<int> { 1, 2, 3 };
            GetFileSAtatuesAsync();
            
        }

        void OnpropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public async void GetFileSAtatuesAsync()
        {
            //await NetworkManager.FetchStringDataFromUri(new Uri("http://172.18.84.38:50070/webhdfs/v1//user/hadoop/files/testFile/?op=LISTSTATUS"));
            var file = new HDFSFile()
            {
                ServerHost = "172.18.84.38",
                Path = "/user/hadoop/files/testFile/"
            };
            FileList = await file.SubFile;
            Bindings.Update();
        }

        private async void HDFSFileListView_ItemClickAsync(object sender, ItemClickEventArgs e)
        {
            var items = e.ClickedItem as HDFSFile;
            FileList.Clear();
            foreach (var item in await items.SubFile)
            {
                filelist.Add(item);
            }
        }
    }
}
