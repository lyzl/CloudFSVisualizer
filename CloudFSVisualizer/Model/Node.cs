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
            await Task.Run(async () =>
             {
                 client.Connect();

                 SshCommand sc;
                 foreach (var command in commandList)
                 {
                     sc = client.CreateCommand(command);
                     var SCresult = sc.BeginExecute();
                     while (SCresult.IsCompleted == false)
                     {
                         await Task.Delay(TimeSpan.FromSeconds(1));
                     }
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
            @"sbin/start-yarn.sh",
            @"sbin/stop-yarn.sh",
            @"sbin/test",
        };

        public virtual int ServicePort { get; protected set; }

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

        public async Task<NodeOperatingSystem> GetOperatingSystemInfoAsync()
        {
            string queryUrl = $@"http://{Host}:{ServicePort}/jmx?qry=java.lang:type=OperatingSystem";
            var json = await NetworkManager.FetchStringDataFromUri(new Uri(queryUrl));
            if (json == null)
            {
                return null;
            }
            JObject rootObject = JObject.Parse(json);
            JToken infoToken = rootObject["beans"].Children().ToList().First();
            var info = infoToken.ToObject<NodeOperatingSystem>();
            return info;
        }
    }

    public abstract class HDFSNode: HadoopNode 
    {
        
    }

    public abstract class YarnNode : HadoopNode
    {
        
    }

    public class HDFSMasterNode : HDFSNode
    {
        public override int ServicePort { get; protected set; } = 50070;
    }

    public class HDFSSlaveNode : HDFSNode
    {
        public override int ServicePort { get; protected set; } = 50075;
    }

    public class YarnResourceManager: YarnNode
    {
        public override int ServicePort { get; protected set; } = 8088;

        public async Task UploadAppFileFromStream(string FileName, Stream stream)
        {
            await UploadFileFromStreamAsync($@"{HadoopHomeDirectory}upload/{FileName}", stream);
        }
    }

    public class YarnNodeManager: YarnNode
    {
        public override int ServicePort { get; protected set; } = 8042;
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

    public class NodeInfo
    {
        public string hadoopVersionBuiltOn { get; set; }
        public string nodeManagerBuildVersion { get; set; }
        public long lastNodeUpdateTime { get; set; }
        public int totalVmemAllocatedContainersMB { get; set; }
        public int totalVCoresAllocatedContainers { get; set; }
        public bool nodeHealthy { get; set; }
        public string healthReport { get; set; }
        public int totalPmemAllocatedContainersMB { get; set; }
        public string nodeManagerVersionBuiltOn { get; set; }
        public string nodeManagerVersion { get; set; }
        public string id { get; set; }
        public string hadoopBuildVersion { get; set; }
        public string nodeHostName { get; set; }
        public string hadoopVersion { get; set; }
    }
}
