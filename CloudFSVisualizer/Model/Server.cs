using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudFSVisualizer.Model
{
    public abstract class Server
    {
        public enum ServerType { defaultServer, HDFSServer, YarnServer}
        public ServerType SType { get; set; }
        public string Description { get; set; }
    }

    public class HDFSServer : Server
    {
        public HDFSMasterNode MasterNode { get; set; }
        public List<HDFSSlaverNode> SlaveNode { get; set; }
    }


}
