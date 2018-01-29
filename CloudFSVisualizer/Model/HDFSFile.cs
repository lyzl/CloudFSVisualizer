using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudFSVisualizer.Model
{
    public class HDFSFile
    {
        private FileStatus status;
        public string ServerHost { get; set; }
        public string Path { get; set; }
        
        public FileStatus Status
        {
            get { return status; }
            set { status = value; }
        }


        
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
}
