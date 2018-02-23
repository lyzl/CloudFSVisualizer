using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public List<HDFSSlaveNode> SlaveNode { get; set; }

        public async Task<FSNamesystem> GetFSNamesystemAsync()
        {
            string queryUrl = $@"http://{MasterNode.Host}:50070/jmx?qry=Hadoop:service=NameNode,name=FSNamesystem";
            var json = await NetworkManager.FetchStringDataFromUri(new Uri(queryUrl));
            JObject rootObject = JObject.Parse(json);
            JToken infoToken = rootObject["beans"].Children().ToList().First();
            var info = infoToken.ToObject<FSNamesystem>();
            return info;
        }
    }



    public class FSNamesystem
    {
        public string name { get; set; }
        public string modelerType { get; set; }
        [JsonProperty(PropertyName = "tag.Context")]
        public string Context { get; set; }
        [JsonProperty(PropertyName = "tag.HAState")]
        public string HAState { get; set; }
        [JsonProperty(PropertyName = "tag.TotalSyncTimes")]
        public string TotalSyncTimes { get; set; }
        [JsonProperty(PropertyName = "tag.Hostname")]
        public string Hostname { get; set; }
        public int MissingBlocks { get; set; }
        public int MissingReplOneBlocks { get; set; }
        public int ExpiredHeartbeats { get; set; }
        public int TransactionsSinceLastCheckpoint { get; set; }
        public int TransactionsSinceLastLogRoll { get; set; }
        public int LastWrittenTransactionId { get; set; }
        public long LastCheckpointTime { get; set; }
        public long CapacityTotal { get; set; }
        public double CapacityTotalGB { get; set; }
        public long CapacityUsed { get; set; }
        public double CapacityUsedGB { get; set; }
        public long CapacityRemaining { get; set; }
        public double CapacityRemainingGB { get; set; }
        public long CapacityUsedNonDFS { get; set; }
        public int TotalLoad { get; set; }
        public int SnapshottableDirectories { get; set; }
        public int Snapshots { get; set; }
        public int NumEncryptionZones { get; set; }
        public int LockQueueLength { get; set; }
        public int BlocksTotal { get; set; }
        public int NumFilesUnderConstruction { get; set; }
        public int NumActiveClients { get; set; }
        public int FilesTotal { get; set; }
        public int PendingReplicationBlocks { get; set; }
        public int UnderReplicatedBlocks { get; set; }
        public int CorruptBlocks { get; set; }
        public int ScheduledReplicationBlocks { get; set; }
        public int PendingDeletionBlocks { get; set; }
        public int ExcessBlocks { get; set; }
        public int NumTimedOutPendingReplications { get; set; }
        public int PostponedMisreplicatedBlocks { get; set; }
        public int PendingDataNodeMessageCount { get; set; }
        public int MillisSinceLastLoadedEdits { get; set; }
        public int BlockCapacity { get; set; }
        public int NumLiveDataNodes { get; set; }
        public int NumDeadDataNodes { get; set; }
        public int NumDecomLiveDataNodes { get; set; }
        public int NumDecomDeadDataNodes { get; set; }
        public int VolumeFailuresTotal { get; set; }
        public int EstimatedCapacityLostTotal { get; set; }
        public int NumDecommissioningDataNodes { get; set; }
        public int StaleDataNodes { get; set; }
        public int NumStaleStorages { get; set; }
        public int TotalFiles { get; set; }
        public int TotalSyncCount { get; set; }
        public int NumInMaintenanceLiveDataNodes { get; set; }
        public int NumInMaintenanceDeadDataNodes { get; set; }
        public int NumEnteringMaintenanceDataNodes { get; set; }
    }


}
