using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SshNet;
using CloudFSVisualizer.Model;
using Renci.SshNet;
using Windows.Storage;
using CloudFSVisualizer.Assets;

namespace CloudFSVisualizer
{
    public class NetworkManager
    {
        public static async Task<Stream> FetchStreamDataFromUri(Uri uri)
        {
            Stream responseStream = null;

            using (var http = new HttpClient())
            {
                try
                {
                    var response = await http.GetAsync(uri);
                    if (response.IsSuccessStatusCode == true)
                    {
                        responseStream = await response.Content.ReadAsStreamAsync();
                    }
                }
                catch (Exception e)
                {
                    if (AppShell.Current != null)
                    {
                        AppShell.Current.NotifyMessage($"Network Connection error: \n{e.Message}");
                    }
                }

            }
            return responseStream;

        }

        public static async Task<string> FetchStringDataFromUri(Uri uri)
        {
            string responseString = null;

            using (var http = new HttpClient())
            {
                try
                {
                    var response = await http.GetAsync(uri);
                    if (response.IsSuccessStatusCode == true)
                    {
                        responseString = await response.Content.ReadAsStringAsync();
                    }
                }
                catch (Exception e)
                {
                    if (AppShell.Current != null)
                    {
                        AppShell.Current.NotifyMessage($"Network Connection error: \n{e.Message}");
                    }
                }

            }
            return responseString;
        }

        public static SshClient CreateSSHClient(string host, string user, string pswd, string keyFile)
        {
            var connectionInfo = _PrepareConnection(host, user, pswd, keyFile);
            return new SshClient(connectionInfo);
        }

        public static SftpClient CreateSFtpChannel(string host, string user, string pswd, string keyFile)
        {
            var connectionInfo = _PrepareConnection(host, user, pswd, keyFile);
            return new SftpClient(connectionInfo);
        }

        private static ConnectionInfo _PrepareConnection(string host, string user, string pswd, string keyFile)
        {
            var auth = new PasswordAuthenticationMethod(user, pswd);
            var info = new ConnectionInfo(host, user, auth);
            PrivateKeyFile key = new PrivateKeyFile(keyFile);
            var keyFiles = new[] { key };
            var methods = new List<AuthenticationMethod>();
            if (user != null && pswd != null)
            {
                methods.Add(new PasswordAuthenticationMethod(user, pswd));
            }
            if (user != null && pswd != null)
            {
                methods.Add(new PrivateKeyAuthenticationMethod(user, keyFiles));
            }
            var connectionInfo = new ConnectionInfo(host, user, methods.ToArray());
            return connectionInfo;
        }
    }
}
