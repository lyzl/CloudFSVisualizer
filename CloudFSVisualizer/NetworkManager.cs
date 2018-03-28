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
