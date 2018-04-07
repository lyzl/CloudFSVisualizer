using Newtonsoft.Json.Linq;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CloudFSVisualizer.Model
{
    public enum NodeStatus { Unknown, Available, Timeout };
    public class Node
    {
        public string Description { get; set; }
        public string Host { get; set; }
        public string User { get; set; }
        public string Pswd { get; set; }
        public string PrivateKey { get; set; }

        protected async Task DownloadFileAsync(string remotePath, StorageFile localFile)
        {
            using (var stream = await localFile.OpenStreamForWriteAsync())
            using (SftpClient sftp = NetworkManager.CreateSFtpChannel(Host,User,Pswd,PrivateKey))
            {
                try
                {
                    await Task.Run(() =>
                    {
                        sftp.Connect();
                        sftp.DownloadFile(remotePath, stream);
                        sftp.Disconnect();
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error when connect remote server" + e.ToString());
                }
            }
        }

        protected async Task<Stream> DownloadFileAsStreamAsync(string remotePath)
        {
            var stream = new MemoryStream();
            using (SftpClient sftp = NetworkManager.CreateSFtpChannel(Host, User, Pswd, PrivateKey))
            {
                try
                {
                    await Task.Run(() =>
                    {
                        sftp.Connect();
                        sftp.DownloadFile(remotePath, stream);
                        sftp.Disconnect();
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error when connect remote server" + e.ToString());
                }
            }
            return stream;
        }

        protected async Task UploadFileAsync(string remotePath, StorageFile localFile)
        {
            using (var stream = await localFile.OpenStreamForReadAsync())
            using (SftpClient sftp = NetworkManager.CreateSFtpChannel(Host, User, Pswd, PrivateKey))
            {
                try
                {
                    await Task.Run(() =>
                    {
                        sftp.Connect();
                        sftp.UploadFile(stream, remotePath);
                        sftp.Disconnect();
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error when connect remote server" + e.ToString());
                }
            }
        }

        protected async Task UploadFileFromStreamAsync(string remotePath, Stream stream)
        {
            using (SftpClient sftp = NetworkManager.CreateSFtpChannel(Host, User, Pswd, PrivateKey))
            {
                try
                {
                    await Task.Run(() =>
                    {
                        sftp.Connect();
                        sftp.UploadFile(stream, remotePath);
                        sftp.Disconnect();
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error when connect remote server" + e.ToString());
                }
            }
        }

        public async Task<List<string>> RunCommandShell(List<string> commandList, SshClient client)
        {
            List<string> result = new List<string>();
            await Task.Run(() =>
             {
                 client.Connect();

                 SshCommand sc;
                 foreach (var command in commandList)
                 {
                     sc = client.CreateCommand(command);
                     sc.Execute();
                     result.Add(sc.Result);
                 }
                 
             });
            return result;
        }
    }

    public abstract class HadoopNode: Node
    {
        public string HadoopHomeDirectory { get; set; }
        public static List<string> AvaliableConfiguration { get; protected set; } = new List<string>
        {
            @"etc/hadoop/core-site.xml",
            @"etc/hadoop/hadoop-env.sh",
            @"etc/hadoop/hdfs-site.xml",
            @"etc/hadoop/slaves",
            @"etc/hadoop/yarn-env.sh",
            @"etc/hadoop/mapred-env.sh",
            @"etc/hadoop/test",
        };
        public static List<string> AvaliableScript { get; protected set; } = new List<string>
        {
            @"sbin/start-all.sh",
            @"sbin/stop-all.sh",
            @"sbin/start-dfs.sh",
            @"sbin/stop-dfs.sh",
            @"sbin/start-balancer.sh",
            @"sbin/stop-balancer.sh",
            @"sbin/test",
        };
        public async Task<Stream> GetConfigurationAsStream(string RelativeFilePath)
        {
            if (AvaliableConfiguration.Contains(RelativeFilePath))
            {
                return await DownloadFileAsStreamAsync(HadoopHomeDirectory + RelativeFilePath);
            }
            else
            {
                throw new Exception("The configuration file is not allowed to access");
            }
        }

        public async Task UploadConfigurationFromStream(string RelativeFilePath, Stream stream)
        {
            if (AvaliableConfiguration.Contains(RelativeFilePath))
            {
                await UploadFileFromStreamAsync(HadoopHomeDirectory + RelativeFilePath, stream);
            }
            else
            {
                throw new Exception("The configuration file is not allowed to access");
            }
        }
    }

    public abstract class HDFSNode: HadoopNode
    {
        public abstract Task<NodeOperatingSystem> OperatingSystemInfo();
    }

    public abstract class YarnNode : HadoopNode
    {

    }

    public class HDFSMasterNode : HDFSNode
    {
        public override async Task<NodeOperatingSystem> OperatingSystemInfo()
        {
            string queryUrl = $@"http://{Host}:50070/jmx?qry=java.lang:type=OperatingSystem";
            var json = await NetworkManager.FetchStringDataFromUri(new Uri(queryUrl));
            JObject rootObject = JObject.Parse(json);
            JToken infoToken = rootObject["beans"].Children().ToList().First();
            var info = infoToken.ToObject<NodeOperatingSystem>();
            return info;
        }
    }

    public class HDFSSlaveNode : HDFSNode
    {
        public override async Task<NodeOperatingSystem> OperatingSystemInfo()
        {
            string queryUrl = $@"http://{Host}:50075/jmx?qry=java.lang:type=OperatingSystem";
            var json = await NetworkManager.FetchStringDataFromUri(new Uri(queryUrl));
            JObject rootObject = JObject.Parse(json);
            JToken infoToken = rootObject["beans"].Children().ToList().First();
            var info = infoToken.ToObject<NodeOperatingSystem>();
            return info;
        }
    }

    public class YarnResourceManager: YarnNode
    {

    }

    public class YarnNodeManager: YarnNode
    {

    }

    public class NodeOperatingSystem
    {
        public string name { get; set; }
        public string modelerType { get; set; }
        public int OpenFileDescriptorCount { get; set; }
        public int MaxFileDescriptorCount { get; set; }
        public long CommittedVirtualMemorySize { get; set; }
        public int TotalSwapSpaceSize { get; set; }
        public int FreeSwapSpaceSize { get; set; }
        public long ProcessCpuTime { get; set; }
        public int FreePhysicalMemorySize { get; set; }
        public int TotalPhysicalMemorySize { get; set; }
        public double SystemCpuLoad { get; set; }
        public double ProcessCpuLoad { get; set; }
        public string Version { get; set; }
        public string Arch { get; set; }
        public double SystemLoadAverage { get; set; }
        public int AvailableProcessors { get; set; }
        public string Name { get; set; }
        public string ObjectName { get; set; }
    }
}
