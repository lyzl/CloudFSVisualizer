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

namespace CloudFSVisualizer
{
    public class HDFSFileManager
    {
        public static async Task CreateHDFSFile(HDFSServer server, string filePath, Authentication auth)
        {
            
            string url = $@"http://{server.MasterNode.Host}:50070/webhdfs/v1/{filePath}?user.name={auth.User}&op=CREATE";
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };

            using (var client = new HttpClient(handler))
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, url);
                var response = await client.PutAsync(url, null);
                var responseHeader = response.Headers;
            }
            
        }

    }

}
