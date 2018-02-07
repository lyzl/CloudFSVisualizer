using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudFSVisualizer.Model
{
    public enum NodeStatus { Unknown, Available, Timeout };
    public class Node
    {
        public string Description { get; set; }
        public string Host { get; set; }
        public string User { get; set; }
        public string Pswd { get; set; }
        public Uri PrivateKey { get; set; }
    }

    public class HadoopNode: Node
    {
        
    }

    public abstract class HDFSNode: HadoopNode
    {
        
    }

    public class HDFSMasterNode : HDFSNode
    {

    }

    public class HDFSSlaverNode : HDFSNode
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
