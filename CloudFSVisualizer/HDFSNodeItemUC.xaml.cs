﻿using CloudFSVisualizer.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace CloudFSVisualizer
{
    public sealed partial class HDFSNodeItemUC : UserControl, INotifyPropertyChanged
    {
        public HDFSNode NodeItem { get { return this.DataContext as HDFSNode; } }
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

        public HDFSNodeItemUC()
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
    public class NodeStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var status = value as NodeStatus?;
            AcrylicBrush brush = new AcrylicBrush();


            if (value != null)
            {
                switch (status)
                {
                    case (NodeStatus.Unknown):
                        brush.TintColor = Colors.LightGray;
                        break;
                    case (NodeStatus.Available):
                        brush.TintColor = Colors.Azure;
                        break;
                    case (NodeStatus.Timeout):
                        brush.TintColor = Colors.MistyRose;
                        break;
                    default:
                        brush.TintColor = Colors.LightGray;
                        break;

                }
            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
