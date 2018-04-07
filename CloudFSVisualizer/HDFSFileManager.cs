using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CloudFSVisualizer.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Storage;

namespace CloudFSVisualizer
{
    public class HDFSFileManager
    {
        public static async Task UploadHDFSFile(HDFSServer server, StorageFile localFile, string remotePath)
        {
            string url = @"http://" +
                server.MasterNode.Host +
                @":50070/webhdfs/v1" +
                remotePath +
                Uri.EscapeDataString(localFile.Name) + 
                @"?user.name=" + 
                server.Authentication.User + 
                "&op=CREATE";
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            using (var client = new HttpClient(handler))
            using (var stream = await localFile.OpenStreamForReadAsync())
            {
                // see https://hadoop.apache.org/docs/stable/hadoop-project-dist/hadoop-hdfs/WebHDFS.html#Create_and_Write_to_a_File for more info.
                var request1 = new HttpRequestMessage(HttpMethod.Put, url);
                var response1 = await client.SendAsync(request1);
                var redirectUrl = response1.Headers.Location.AbsoluteUri;

                var fileContent = new StreamContent(stream);
                var request2 = new HttpRequestMessage(HttpMethod.Put, redirectUrl) { Content = fileContent };
                var response2 = await client.SendAsync(request2);
            }
            
        }

    }

}
