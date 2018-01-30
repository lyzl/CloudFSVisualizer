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

        public static SshClient CreateSSHClinetToNode(Node node)
        {
            var SSHHost = node.Host;
            var SSHUser = node.User;
            var SSHPswd = node.pswd;
            var auth = new PasswordAuthenticationMethod(SSHUser, SSHPswd);
            var info = new ConnectionInfo(SSHHost, SSHUser, auth);
            return new SshClient(info);
        }

        public static async void UploadFileToNode(StorageFile storageFile, Node node, string remotePath)
        {
            var SSHHost = node.Host;
            var SSHUser = node.User;
            var SSHPswd = node.pswd;
            var auth = new PasswordAuthenticationMethod(SSHUser, SSHPswd);
            var info = new ConnectionInfo(SSHHost, SSHUser, auth);
            using (var client = new SftpClient(info))
            using (var fileStream = await storageFile.OpenStreamForReadAsync())
            {

                client.UploadFile(fileStream, remotePath);
            }
        }

        public static async void DownloadFileFromNode(StorageFile storageFile, Node node, string remotePath)
        {
            var SSHHost = node.Host;
            var SSHUser = node.User;
            var SSHPswd = node.pswd;
            var auth = new PasswordAuthenticationMethod(SSHUser, SSHPswd);
            var info = new ConnectionInfo(SSHHost, SSHUser, auth);
            using (var client = new SftpClient(info))
            using (var fileStream = await storageFile.OpenStreamForWriteAsync())
            {
                client.DownloadFile(remotePath, fileStream);
            }
        }
    }
}
