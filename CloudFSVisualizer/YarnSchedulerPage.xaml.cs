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
using System.ComponentModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using CloudFSVisualizer.Assets;
using Renci.SshNet;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CloudFSVisualizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public class JobDataGridPresentData
    {
        public YarnApp OriginData;
        public string id { get; set; }
        public string user { get; set; }
        public string name { get; set; }
        public string queue { get; set; }
        public string state { get; set; }
        public string finalStatus { get; set; }
        public double progress { get; set; }

        public JobDataGridPresentData(YarnApp app)
        {
            this.OriginData = app;
            this.id = app.id;
            this.user = app.user;
            this.name = app.user;
            this.queue = app.queue;
            this.state = app.state;
            this.finalStatus = app.finalStatus;
            this.progress = app.progress;
        }
    }
    public sealed partial class YarnSchedulerPage : Page, INotifyPropertyChanged
    {
        public AppShell appshell { get; set; }
        public YarnServer CurrentServer { get; set; }

        private List<YarnApp> currentAppList;
        public List<YarnApp> CurrentAppList
        {
            get { return currentAppList; }
            set
            {
                currentAppList = value;
                OnpropertyChanged("CurrentApps");
            }
        }

        private List<JobDataGridPresentData> presentList;
        public List<JobDataGridPresentData> PresentList
        {
            get { return presentList; }
            set
            {
                presentList = value;
                OnpropertyChanged("PresentList");
            }
        }

        private YarnApp currentApp;
        public YarnApp CurrentApp
        { 
            get { return currentApp; }
            set
            {
                currentApp = value;
                OnpropertyChanged("CurrentApp");
            }
        }
        private StorageFile currentAppFile;

        public StorageFile CurrentAppFile
        {
            get { return currentAppFile; }
            set
            {
                currentAppFile = value;
                OnpropertyChanged("CurrentAppFile");
            }
        }
        public SshClient Client { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public YarnSchedulerPage()
        {
            this.InitializeComponent();
            appshell = AppShell.Current;
        }

        void OnpropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void JobDataGrid_SelectionChanged(object sender, Telerik.UI.Xaml.Controls.Grid.DataGridSelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count() == 0)
            {
                return;
            }
            var item = e.AddedItems.First() as JobDataGridPresentData;
            CurrentApp = item.OriginData;
            JobDetailGrid.Visibility = Visibility.Visible;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var item = e.Parameter as YarnServer;
            CurrentServer = item;
            GetApplications();
            Client = NetworkManager.CreateSSHClient(
                CurrentServer.ResourceManager.Host,
                CurrentServer.ResourceManager.User,
                CurrentServer.ResourceManager.Pswd,
                CurrentServer.ResourceManager.PrivateKey);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Client.Dispose();
        }

        public async void GetApplications()
        {
            CurrentAppList = await CurrentServer.GetApplications();
            var pList = new List<JobDataGridPresentData>();
            foreach (var app in currentAppList)
            {
                pList.Add(new JobDataGridPresentData(app));
            }
            PresentList = pList;
        }

        private void NewAppDroppingGrid_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
        }

        private async void NewAppDroppingGrid_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var storageItems = await e.DataView.GetStorageItemsAsync();
                var file = storageItems.First() as StorageFile;
                if (file.FileType == ".jar")
                {
                    CurrentAppFile = file;
                }
            }
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentAppFile != null)
            {
                var homePath = CurrentServer.ResourceManager.HadoopHomeDirectory;
                var uploadNode = CurrentServer.ResourceManager;
                using (var stream = await CurrentAppFile.OpenStreamForReadAsync())
                {
                    await uploadNode.UploadAppFileFromStream(currentAppFile.Name, stream);
                    var argument = AdditionalArgumentTextBox.Text;
                    await uploadNode.RunCommandShell(
                        new List<string> {
                            $@"{homePath}bin/hadoop jar {homePath}upload/{CurrentAppFile.Name} {argument}"
                        }, 
                        Client);
                }
            }
            appshell.NotifyMessage("Create Application Success.");
            CurrentAppFile = null;
            GetApplications();
        }
    }

    public class ApplicationDroppingGridTextConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "Submit new Application";
            }
            else
            {
                return (value as StorageFile).Name;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
