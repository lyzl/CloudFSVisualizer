using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CloudFSVisualizer.Model
{
    public class HDFSFile
    {
        private FileStatus status;
        public string ServerHost { get; set; }
        public string Path { get; set; }

        private AsyncLazy<List<HDFSFile>> subFile;
        public AsyncLazy<List<HDFSFile>> SubFile
        {
            get
            {
                if (subFile == null)
                {
                    subFile = new AsyncLazy<List<HDFSFile>>(async () =>
                    {
                        return await HDFSFileManager.ListDirectory(this);
                    });
                }
                return subFile;
            }
        }

        public FileStatus Status
        {
            get
            {
                if (status == null)
                {
                    status = HDFSFileManager.GetFileStatus(this).Result;
                }
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


        
    }
    public class FileStatus
    {
        public long accessTime { get; set; }
        public int blockSize { get; set; }
        public int childrenNum { get; set; }
        public int fileId { get; set; }
        public string group { get; set; }
        public int length { get; set; }
        public long modificationTime { get; set; }
        public string owner { get; set; }
        public string pathSuffix { get; set; }
        public string permission { get; set; }
        public int replication { get; set; }
        public int storagePolicy { get; set; }
        public string type { get; set; }
    }

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
