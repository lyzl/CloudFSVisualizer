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
using System.Text;
using Windows.UI.Popups;
using System.Threading.Tasks;
using Renci.SshNet;
using Windows.UI.Core;
using CloudFSVisualizer.Assets;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CloudFSVisualizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HDFSConfigurationPage : Page
    {
        public HDFSNode CurrentNode { get; set; }
        public SshClient CurrentNodeSshClient { get; set; }
        public List<string> AvaliableConfigurationFileList { get; set; }
        public List<string> AvaliableScriptFileList { get; set; }
        public HDFSConfigurationPage()
        {
            this.InitializeComponent();
            AvaliableConfigurationFileList = HDFSNode.AvaliableConfiguration;
            AvaliableScriptFileList = HDFSNode.AvaliableScript;
            FileTypeComboBox.SelectedIndex = 0;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            CurrentNode = e.Parameter as HDFSNode;
            CurrentNodeSshClient = NetworkManager.CreateSSHClient(
                CurrentNode.Host, 
                CurrentNode.User, 
                CurrentNode.Pswd,
                CurrentNode.PrivateKey);
            CurrentNodeSshClient.Connect();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (CurrentNodeSshClient != null)
            {
                CurrentNodeSshClient.Disconnect();
            }
        }

        private async void SavingButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(ContentTextBox.Text);
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                if (FileComboBox.SelectedItem is string path)
                {
                    await CurrentNode.UploadConfigurationFromStream(path, stream);
                }
            }
        }

        private void ExecutingButton_Click(object sender, RoutedEventArgs e)
        {
            if (FileComboBox.SelectedItem is string filePath)
            {
                FileComboBox.IsEnabled = false;
                FileTypeComboBox.IsEnabled = false;
                ExecutingButton.IsEnabled = false;

                Task.Run(async () => {
                    try
                    {
                        var cmd = CurrentNodeSshClient.CreateCommand(CurrentNode.HadoopHomeDirectory + filePath);
                        var result = cmd.BeginExecute();
                        using (var stdOutReader = new StreamReader(cmd.OutputStream))
                        using (var stdErrReader = new StreamReader(cmd.ExtendedOutputStream))
                        {
                            while (!result.IsCompleted || !stdOutReader.EndOfStream || !stdErrReader.EndOfStream)
                            {
                                string outLine = stdOutReader.ReadLine();
                                if (outLine != null)
                                {
                                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                    {
                                        ContentTextBox.Text += outLine + "\n";
                                    });
                                }
                                string errLine = stdErrReader.ReadLine();
                                if (errLine != null)
                                {
                                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                    {
                                        ContentTextBox.Text += errLine + "\n";
                                    });
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (AppShell.Current != null)
                        {
                            AppShell.Current.NotifyMessage($"Error occoured when excuting commands:\n{ex.Message}");
                        }
                    }


                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        FileComboBox.IsEnabled = true;
                        FileTypeComboBox.IsEnabled = true;
                        ExecutingButton.IsEnabled = true;
                    });
                });

            }
        }

        private void ContentTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void FileTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems.First() is string item)
                {
                    switch (item)
                    {
                        case "Configuration":
                            FileComboBox.ItemsSource = HDFSNode.AvaliableConfiguration;
                            ExecutingButton.IsEnabled = false;
                            SavingButton.IsEnabled = true;
                            break;
                        case "Script":
                            FileComboBox.ItemsSource = HDFSNode.AvaliableScript;
                            ExecutingButton.IsEnabled = true;
                            SavingButton.IsEnabled = false;
                            break;
                        default:
                            break;
                    }
                }
            }
            
        }

        private async void FileComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<object> addedItemlist = new List<object>(e.AddedItems);
            if (addedItemlist.Count > 0)
            {
                if (addedItemlist.First() is string item && FileTypeComboBox.SelectedItem is string file)
                {
                    ContentTextBox.Text = string.Empty;
                    switch (file)
                    {
                        case "Configuration":
                            using (var stream = await CurrentNode.GetConfigurationAsStream(item))
                            using (var reader = new StreamReader(stream, Encoding.UTF8))
                            {
                                stream.Position = 0;
                                ContentTextBox.Text = reader.ReadToEnd();
                            }
                            break;
                        case "Script":
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
