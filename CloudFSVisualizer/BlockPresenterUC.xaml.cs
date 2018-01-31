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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace CloudFSVisualizer
{
    public sealed partial class BlockPresenterUC : UserControl
    {
        public HDFSSlaverNode Nodes { get; set; }
        public List<int> Blocks { get; set; }
        public BlockPresenterUC()
        {
            this.InitializeComponent();
            Blocks = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                Blocks.Add(i);
            }
        }
    }
}
