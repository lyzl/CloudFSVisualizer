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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace CloudFSVisualizer
{
    public sealed partial class BlockPresenterUC : UserControl, INotifyPropertyChanged
    {
        private LocatedBlock currentBlock;
        public LocatedBlock CurrentBlock
        {
            get { return currentBlock; }
            set
            {
                currentBlock = value;
                OnPropertyChanged("CurrentBlock");
            }
        }
        private List<LocatedBlock> itemsSource;
        public List<LocatedBlock> ItemsSource
        {
            get { return itemsSource; }
            set
            {
                itemsSource = value;
                OnPropertyChanged("BlockList");
            }
        }
        private LocatedBlock selectedBlock;
        public LocatedBlock SelectedBlock
        {
            get { return selectedBlock; }
            set
            {
                selectedBlock = value;

            }
        }


        public HDFSSlaveNode Nodes { get; set; }
        public static readonly DependencyProperty ItemsSourceProperty =
               DependencyProperty.Register(
                     "ItemsSource",
                      typeof(ServerLocatedBlocks),
                      typeof(BlockPresenterUC),
                      new PropertyMetadata(
                          null,
                          new PropertyChangedCallback(ItemSource_PropertyChanged)
                          ));

        private static void ItemSource_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as BlockPresenterUC;

            if (control != null)
            {
                control.OnPropertyChanged("BlockList");
            }
                //control.OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public BlockPresenterUC()
        {
            this.InitializeComponent();
            CurrentBlock = new LocatedBlock();
            SelectedBlock = new LocatedBlock();
        }

        private void BlocksGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as LocatedBlock;
        }

        private void BlockRectangle_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var item = sender as Grid;
            var dataObject = item.DataContext as LocatedBlock;
            currentBlock = dataObject;
            //CurrentBlock = item;
        }
    }
}
