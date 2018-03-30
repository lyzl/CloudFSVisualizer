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
using System.Threading;
using Windows.UI.Core;
using System.ComponentModel;
using Telerik.UI.Xaml.Controls.Chart;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CloudFSVisualizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HDFSServerDetailPage : Page, INotifyPropertyChanged
    {
        public HDFSServer CurrentServer { get; set; }
        private List<double> testList;

        public List<double> TestList
        {
            get { return testList; }
            set {testList = value; }
        }

        private FSNamesystem fsInfo;
        public FSNamesystem FSInfo
        {
            get { return fsInfo; }
            set
            {
                fsInfo = value;
                OnpropertyChanged("FSInfo");
                CapacityPieChartSeries.ItemsSource = DiskCapacity;
            }
        }
        public List<Tuple<string, double>> DiskCapacity
        {
            get
            {
                if (fsInfo == null)
                {
                    return null;
                }
                return new List<Tuple<string, double>>()
                {
                    new Tuple<string, double>("CapacityUsed",fsInfo.CapacityUsed / 0x40000000),
                    new Tuple<string, double>("CapacityRemaining",fsInfo.CapacityRemaining / 0x40000000),
                    new Tuple<string, double>("CapacityUsedNonDFS",fsInfo.CapacityUsedNonDFS / 0x40000000),
                    new Tuple<string, double>("Reserved",(fsInfo.CapacityTotal - fsInfo.CapacityUsed - fsInfo.CapacityRemaining - fsInfo.CapacityUsedNonDFS) / 0x40000000)
                };
            }
        }


        public List<HDFSNode> HDFSNodeList { get; set; }
        public Timer QueryTimer { get; set; }
        public TimeSpan startTimeSpan = TimeSpan.Zero;
        public TimeSpan periodicTimeSpan = TimeSpan.FromMilliseconds(2000);

        public event PropertyChangedEventHandler PropertyChanged;

        private AutoResetEvent _queryLock = new AutoResetEvent(true);

        public HDFSServerDetailPage()
        {
            this.InitializeComponent();
            QueryTimer = new Timer(async (e) =>
            {

                _queryLock.WaitOne();
                _queryLock.Reset();
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    if (CurrentServer == null)
                    {
                        QueryTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        return;
                    }
                    FSInfo = await CurrentServer.GetFSNamesystemAsync();
                });
                _queryLock.Set();
            },
            _queryLock,
            startTimeSpan,
            periodicTimeSpan);
            //TestList = new List<double>() { 1, 2, 3 };
            //DiskCapacity = new List<double>() { 1, 2, 3 };
            CapacityPieChartSeries.ItemsSource = DiskCapacity;

            //CapacityPieChartSeries.ValueBinding = new GenericDataPointBinding<Tuple<string, double>, double>()
            //{
            //    ValueSelector = item => item.Item2,
            //};
            //var Label = new ChartSeriesLabelDefinition();
            //ChartSeriesLabelDefinition.Strategy = new GenericDataPointBinding<Tuple<string, double>, double>()
            //{
            //    ValueSelector = item => item.Item2,
            //};
            //CapacityPieChartSeries.ValueBinding = new GenericDataPointBinding<int,int>()
            //{
            //    ValueSelector = item => item,
            //};
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var item = e.Parameter as HDFSServer;
            CurrentServer = item;
            HDFSNodeList = new List<HDFSNode>();
            HDFSNodeList.Add(item.MasterNode);
            foreach (var slaveNode in item.SlaveNode)
            {
                HDFSNodeList.Add(slaveNode);
            }
        }

        private void OnpropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void ServerDetailListGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as HDFSNode;
            Frame.Navigate(typeof(HDFSNodePage), item);
        }

        private void HDFSFilePageButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HDFSFilePage), CurrentServer);
        }

        private async void HDFSConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            await HDFSFileManager.CreateHDFSFile(CurrentServer, "testfile", new Authentication { User = "root" });
        }

        private void ServerDetailListGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var columns = Math.Ceiling(ActualWidth / 400);
            (ServerDetailListGridView.ItemsPanelRoot as ItemsWrapGrid).ItemWidth = e.NewSize.Width / columns;
            
        }
    }
}
