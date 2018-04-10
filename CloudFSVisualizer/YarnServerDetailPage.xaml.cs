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
using System.Threading;
using System.ComponentModel;
using Windows.UI.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CloudFSVisualizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class YarnServerDetailPage : Page, INotifyPropertyChanged
    {
        public YarnServer CurrentServer { get; set; }

        private FSNamesystem fsInfo;
        public FSNamesystem FSInfo
        {
            get { return fsInfo; }
            set
            {
                fsInfo = value;
                OnpropertyChanged("FSInfo");
                //CapacityPieChartSeries.ItemsSource = DiskCapacity;
            }
        }

        public List<YarnNode> YarnNodeList { get; set; }
        public Timer QueryTimer { get; set; }
        public TimeSpan startTimeSpan = TimeSpan.Zero;
        public TimeSpan periodicTimeSpan = TimeSpan.FromMilliseconds(2000);

        public event PropertyChangedEventHandler PropertyChanged;

        private AutoResetEvent _queryLock = new AutoResetEvent(true);

        public YarnServerDetailPage()
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
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var item = e.Parameter as YarnServer;
            CurrentServer = item;
            YarnNodeList = new List<YarnNode>();
            YarnNodeList.Add(item.ResourceManager);
            foreach (var slaveNode in item.NodeManager)
            {
                YarnNodeList.Add(slaveNode);
            }
        }

        private void OnpropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void ServerDetailListGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as YarnNode;
            Frame.Navigate(typeof(YarnNodePage), item);
        }

        private void ServerDetailListGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var columns = Math.Ceiling(ActualWidth / 400);
            //(ServerDetailListGridView.ItemsPanelRoot as ItemsWrapGrid).ItemWidth = e.NewSize.Width / columns;

        }

        private void YarnSchedulerButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void YarnConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HDFSFilePage), CurrentServer);
        }
    }
}
