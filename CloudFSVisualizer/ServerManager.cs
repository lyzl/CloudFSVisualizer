using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using CloudFSVisualizer.Model;
using Windows.Storage;
using Windows.UI.Popups;
using Newtonsoft.Json;

namespace CloudFSVisualizer
{
    public class ServerManager
    {
        public static async Task<List<HDFSServer>> GetHDFSServerListFromFileAsync()
        {
            try
            { 
                var localFolder = ApplicationData.Current.LocalFolder;
                if (File.Exists(string.Format("{0}\\{1}",localFolder.Path, "HDFSServersInfo.json")))
                {
                    var serversInfoFile = await localFolder.GetFileAsync("HDFSServersInfo.json");
                    var serializer = new JsonSerializer();
                    List<HDFSServer> servers;
                    using (var stream = await serversInfoFile.OpenStreamForReadAsync())
                    using (var sr = new StreamReader(stream))
                    using (var jsonTextReader = new JsonTextReader(sr))
                    {
                        servers = serializer.Deserialize<List<HDFSServer>>(jsonTextReader);
                    }
                    return servers;
                }
                else
                {
                    return new List<HDFSServer>();
                }
                //else
                //{
                //    // write default to serverList file
                //    await localFolder.CreateFileAsync("HDFSServersInfo.json");
                //    var serversInfoFile = await localFolder.GetFileAsync("HDFSServersInfo.json");
                //    using (var stream = await serversInfoFile.OpenStreamForWriteAsync())
                //    {
                //        await Windows.Storage.FileIO.WriteTextAsync(serversInfoFile, "{}");
                //    }
                //    return new List<HDFSServer>();
                //}
            }
            catch (Exception)
            {
                return null;
            }       
        }
        public static async void StoreHDFSServerListToFileAsync(List<HDFSServer> servers)
        {
            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                StorageFile serversInfoFile ;
                serversInfoFile = await localFolder.GetFileAsync("HDFSServersInfo.json");
                if (File.Exists(string.Format("{0}\\{1}",localFolder.Path, "HDFSServersInfo.json")))
                {
                    serversInfoFile = await localFolder.GetFileAsync("HDFSServersInfo.json");
                }
                else
                {
                    serversInfoFile = await localFolder.CreateFileAsync("HDFSServersInfo.json");
                }
                var path = serversInfoFile.Path;
                var serializer = new JsonSerializer();
                using (var stream = await serversInfoFile.OpenStreamForWriteAsync())
                using (var sr = new StreamWriter(stream))
                using (var jsonTextWriter = new JsonTextWriter(sr))
                {
                    serializer.Serialize(jsonTextWriter, servers);
                }
            }
            catch (Exception e)
            {
                await new MessageDialog("An error occoured when saving file.\n" + e.Message).ShowAsync();
            }
            return;
        }
    }
}
