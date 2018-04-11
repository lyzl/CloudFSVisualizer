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
using System.ComponentModel;
using System.Threading;
using Windows.UI.Core;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace CloudFSVisualizer
{
    public sealed partial class YarnNodeItemUC : UserControl, INotifyPropertyChanged
    {
        public YarnNode NodeItem { get { return this.DataContext as YarnNode; } }
        private NodeStatus status;

        public NodeStatus Status
        {
            get { return status; }
            set
            {
                status = value;
                OnpropertyChanged("Status");
            }
        }

        public Timer QueryTimer { get; set; }
        public TimeSpan startTimeSpan = TimeSpan.Zero;
        public TimeSpan periodicTimeSpan = TimeSpan.FromMilliseconds(5000);
        private AutoResetEvent _queryLock = new AutoResetEvent(true);

        public event PropertyChangedEventHandler PropertyChanged;

        public YarnNodeItemUC()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, a) => { Bindings.Update(); };

            Status = NodeStatus.Unknown;
            QueryTimer = new Timer(async (e) =>
            {

                _queryLock.WaitOne();
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    if (NodeItem == null)
                    {
                        QueryTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        return;
                    }
                    Status = await NodeManager.GetNodeConnectionStatus(NodeItem);
                });
                _queryLock.Set();
            },
            _queryLock,
            startTimeSpan,
            periodicTimeSpan);
        }

        private void OnpropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
