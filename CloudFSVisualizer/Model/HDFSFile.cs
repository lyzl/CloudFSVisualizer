using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CloudFSVisualizer.Model
{
    public class HDFSFile
    {
        
        public string ServerHost { get; set; }
        public string Path { get; set; }

        private AsyncLazy<List<HDFSFile>> subFile;
        public AsyncLazy<List<HDFSFile>> SubFile
        {
            get
            {
                subFile = new AsyncLazy<List<HDFSFile>>(async () =>
                {
                    return await HDFSFileManager.ListDirectory(this);
                });
                return subFile;
            }
        }

        private AsyncLazy<FileStatus> status;
        public AsyncLazy<FileStatus> Status
        {
            get
            {
                status = new AsyncLazy<FileStatus>(async () =>
                {
                    return await GetFileStatusAsync();
                });
                return status;
            }
            set { status = value; }
        }

        public HDFSFile (HDFSFile parentFile, string pathSuffix)
        {
            ServerHost = parentFile.ServerHost;
            Path = $@"{parentFile.Path}/{pathSuffix}";
        }
        public HDFSFile() { }

        public async Task<LocatedBlocks> GetBlocksAsync()
        {
            var address = $@"http://{ServerHost}:50070/webhdfs/v1/{Path}?op=GET_BLOCK_LOCATIONS";
            var json = await NetworkManager.FetchStringDataFromUri(new Uri(address));
            JObject rootObject = JObject.Parse(json);
            JToken statusToken = rootObject["LocatedBlocks"];
            var status = statusToken.ToObject<LocatedBlocks>();
            return status;
        }

        public async Task<FileStatus> GetFileStatusAsync()
        {
            var address = $@"http://{ServerHost}:50070/webhdfs/v1/{Path}?op=GETFILESTATUS";
            var json = await NetworkManager.FetchStringDataFromUri(new Uri(address));
            JObject rootObject = JObject.Parse(json);
            JToken statusToken = rootObject["FileStatus"];
            var status = statusToken.ToObject<FileStatus>();
            return status;
        }


    }
    public class FileStatus
    {
        public long accessTime { get; set; }
        public long blockSize { get; set; }
        public int childrenNum { get; set; }
        public long fileId { get; set; }
        public string group { get; set; }
        public long length { get; set; }
        public long modificationTime { get; set; }
        public string owner { get; set; }
        public string pathSuffix { get; set; }
        public string permission { get; set; }
        public int replication { get; set; }
        public int storagePolicy { get; set; }
        public string type { get; set; }
    }

    public class Block
    {
        public int blockId { get; set; }
        public string blockPoolId { get; set; }
        public int generationStamp { get; set; }
        public long numBytes { get; set; }
    }

    public class BlockToken
    {
        public string urlString { get; set; }
    }

    public class Location
    {
        public string adminState { get; set; }
        public object blockPoolUsed { get; set; }
        public long cacheCapacity { get; set; }
        public long cacheUsed { get; set; }
        public object capacity { get; set; }
        public object dfsUsed { get; set; }
        public string hostName { get; set; }
        public long infoPort { get; set; }
        public long infoSecurePort { get; set; }
        public string ipAddr { get; set; }
        public long ipcPort { get; set; }
        public long lastBlockReportMonotonic { get; set; }
        public object lastBlockReportTime { get; set; }
        public object lastUpdate { get; set; }
        public long lastUpdateMonotonic { get; set; }
        public string name { get; set; }
        public string networkLocation { get; set; }
        public object remaining { get; set; }
        public string storageID { get; set; }
        public long xceiverCount { get; set; }
        public long xferPort { get; set; }
    }

    public class LastLocatedBlock
    {
        public Block block { get; set; }
        public BlockToken blockToken { get; set; }
        public List<object> cachedLocations { get; set; }
        public bool isCorrupt { get; set; }
        public List<Location> locations { get; set; }
        public long startOffset { get; set; }
        public List<string> storageTypes { get; set; }
    }

    public class LocatedBlock
    {
        public Block block { get; set; }
        public BlockToken blockToken { get; set; }
        public List<object> cachedLocations { get; set; }
        public bool isCorrupt { get; set; }
        public List<Location> locations { get; set; }
        public object startOffset { get; set; }
        public List<string> storageTypes { get; set; }
    }

    public class LocatedBlocks
    {
        public long fileLength { get; set; }
        public bool isLastBlockComplete { get; set; }
        public bool isUnderConstruction { get; set; }
        public LastLocatedBlock lastLocatedBlock { get; set; }
        public List<LocatedBlock> locatedBlocks { get; set; }
    }

    public class ServerLocatedBlocks
    {
        public string HostName { get; set; }
        public LocatedBlock PresentBlock { get; set; }
        public List<LocatedBlock> LocatedBlockList { get; set; }
    }
    /*
    public class RootObject
    {
        public LocatedBlocks LocatedBlocks { get; set; }
    }
    */

    public sealed class AsyncLazy<T>
    {
        /// <summary>
        /// The underlying lazy task.
        /// </summary>
        private readonly Lazy<Task<T>> instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncLazy&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="factory">The delegate that is invoked on a background thread to produce the value when it is needed.</param>
        public AsyncLazy(Func<T> factory)
        {
            instance = new Lazy<Task<T>>(() => Task.Run(factory));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncLazy&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="factory">The asynchronous delegate that is invoked on a background thread to produce the value when it is needed.</param>
        public AsyncLazy(Func<Task<T>> factory)
        {
            instance = new Lazy<Task<T>>(() => Task.Run(factory));
        }

        /// <summary>
        /// Asynchronous infrastructure support. This method permits instances of <see cref="AsyncLazy&lt;T&gt;"/> to be await'ed.
        /// </summary>
        public TaskAwaiter<T> GetAwaiter()
        {
            return instance.Value.GetAwaiter();
        }

        /// <summary>
        /// Starts the asynchronous initialization, if it has not already started.
        /// </summary>
        public void Start()
        {
            var unused = instance.Value;
        }
    }
}
