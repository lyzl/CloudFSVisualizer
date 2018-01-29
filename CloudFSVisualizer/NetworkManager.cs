using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SshNet;
using CloudFSVisualizer.Model;

namespace CloudFSVisualizer
{
    public class NetworkManager
    {
        public static async Task<Stream> FetchStreamDataFromUri(Uri uri)
        {
            var http = new HttpClient();
            var response = await http.GetAsync(uri);
            if (response.IsSuccessStatusCode == true)
            {
                return await response.Content.ReadAsStreamAsync();
            }
            else
            {
                return null;
            }
        }

        public static async Task<string> FetchStringDataFromUri(Uri uri)
        {
            var http = new HttpClient();
            var response = await http.GetAsync(uri);
            if (response.IsSuccessStatusCode == true)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return null;
            }
        }

        public static void CreateSSHToServers(List<Server> servers)
        {
            foreach (var server in servers)
            {
                var host = server.
            }
            var host = 
        }
        public static void CreateSSHToNodes()
        {

        }
    }
}
