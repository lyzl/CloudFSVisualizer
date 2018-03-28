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
using Windows.UI;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace CloudFSVisualizer
{
    public sealed partial class BlockPresenterUC : UserControl, INotifyPropertyChanged
    {
        public static Brush DefaultColor = new SolidColorBrush(Colors.LightGray);
        public static Brush HighlightColor = new SolidColorBrush(Colors.LightBlue);

        private List<LocatedBlock> itemsSource;
        public List<LocatedBlock> ItemsSource
        {
            get { return itemsSource; }
            set
            {
                itemsSource = value;
                OnPropertyChanged("ItemsSource");
            }
        }
        private LocatedBlock highlightBlock;
        public LocatedBlock HighlightBlock
        {
            get { return highlightBlock; }
            set
            {
                highlightBlock = value;
                OnPropertyChanged("HighlightBlock");
            }
        }

        private Grid LastSelectedGrid { get; set; }

        public event RoutedEventHandler BlockTapped;
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(LocatedBlock),
                typeof(BlockPresenterUC),
                new PropertyMetadata(
                    null,
                    new PropertyChangedCallback(ItemSource_PropertyChanged)
                    ));

        public static readonly DependencyProperty HighlightProperty =
            DependencyProperty.Register(
                "HighlightBlock",
                typeof(LocatedBlock),
                typeof(BlockPresenterUC),
                new PropertyMetadata(
                    new LocatedBlock(),
                    new PropertyChangedCallback(Highlight_PropertyChanged)
                    ));

        private static void Highlight_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockPresenterUC control)
            {
                (d as BlockPresenterUC).HighlightBlock = e.NewValue as LocatedBlock;
            }
        }

        private static void ItemSource_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockPresenterUC control)
            {
                control.OnPropertyChanged("BlockList");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public BlockPresenterUC()
        {
            this.InitializeComponent();
            HighlightBlock = new LocatedBlock();
            LastSelectedGrid = new Grid();
        }

        private int SearchIndexOfBlock(LocatedBlock item)
        {
            return ItemsSource.FindIndex(p => p.block.blockId == item.block.blockId);
        }

        private void BlockRectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is Grid s)
            {
                s.Background = HighlightColor;
                LastSelectedGrid.Background = DefaultColor;
                LastSelectedGrid = s;
                HighlightBlock = s.DataContext as LocatedBlock;
                BlockTapped?.Invoke(sender, e);
            }
        }
    }
}
