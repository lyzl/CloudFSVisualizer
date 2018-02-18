using CloudFSVisualizer.Model;
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
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Windows.UI.Core;
using System.Threading.Tasks;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Chart;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CloudFSVisualizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HDFSNodePage : Page, INotifyPropertyChanged
    {
        public HDFSNode CurrentNode { get; set; }
        public ObservableCollection<NodeOperatingSystem> SysInfoList { get; set; }
        public Timer QueryTimer { get; set; }
        public TimeSpan startTimeSpan = TimeSpan.Zero;
        public TimeSpan periodicTimeSpan = TimeSpan.FromMilliseconds(1000);
        public event PropertyChangedEventHandler PropertyChanged;

        private AutoResetEvent _queryLock = new AutoResetEvent(false);

        

        public HDFSNodePage()
        {
            this.InitializeComponent();
            SysInfoList = new ObservableCollection<NodeOperatingSystem>();
            for (int i = 0; i < 50; i++)
            {
                SysInfoList.Add(new NodeOperatingSystem());
            }
            //(MemoryLineChart.Series[0] as LineSeries).DependentRangeAxis = new LineSeriesAxis();
            QueryTimer = new Timer(async (e) =>
            {
                //Debug.WriteLine("called");
                _queryLock.WaitOne();
                _queryLock.Reset();

                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    var value = await CurrentNode.OperatingSystemInfo();
                    SysInfoList.RemoveAt(0);
                    SysInfoList.Add(value);
                });
                _queryLock.Set();
            },
            _queryLock,
            startTimeSpan,
            periodicTimeSpan);
            MemoryChartAreaSeries.ValueBinding = new GenericDataPointBinding<NodeOperatingSystem, float>()
            {
                ValueSelector = item => ((float)item.TotalPhysicalMemorySize - (float)item.FreePhysicalMemorySize)
                                        / (float)item.TotalPhysicalMemorySize * 100f,
            };

        }

        private void OnpropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private async void NodeConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            //Frame.Navigate(typeof(NodeConfigurationPage), CurrentNode);

            var template = await CurrentNode.OperatingSystemInfo();
            SysInfoList.Add(template);
            //testCollection.Add(new TestClass() { value = 15 });
            //((LineSeries)this.MemoryLineChart.Series[0]).ItemsSource = SysInfoList;
            //for (int i = 0; i < SysInfoList.Count; i++)
            //{
            //    SysInfoList[i] = template;
            //    SysInfoList[i].FreePhysicalMemorySize = i;
            //}

            //((LineSeries)this.MemoryLineChart.Series[0]).ItemsSource = SysInfoList;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var item = e.Parameter as HDFSNode;
            CurrentNode = item;
            _queryLock.Set();

            //SysInfoList = new ObservableCollection<NodeOperatingSystem>(new NodeOperatingSystem[20]);

        }

    }



    public class UsedMemoryPercentageConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return 0;
            }
            var systemInfo = value as NodeOperatingSystem;
            var total = systemInfo.TotalPhysicalMemorySize;
            var free = systemInfo.FreePhysicalMemorySize;
            //return systemInfo.FreePhysicalMemorySize / 100000;
            if (total == 0)
            {
                return 0;
            }
            else
            {
                return (((float)total - (float)free) / (float)total * 100f);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
