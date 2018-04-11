using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudFSVisualizer.Model
{

    public enum ServerType { defaultServer, HDFSServer, YarnServer }
    public abstract class Server
    {
        public ServerType SType { get; set; }
        public string Description { get; set; }
    }

    public abstract class HadoopServer : Server
    {
        public HadoopAuthentication Authentication { get; set; }
    }

    public class HDFSServer : HadoopServer
    {
        public HDFSMasterNode MasterNode { get; set; }
        public List<HDFSSlaveNode> SlaveNode { get; set; }

        public async Task<FSNamesystem> GetFSNamesystemAsync()
        {
            string queryUrl = $@"http://{MasterNode.Host}:50070/jmx?qry=Hadoop:service=NameNode,name=FSNamesystem";
            var json = await NetworkManager.FetchStringDataFromUri(new Uri(queryUrl));
            if (json == null)
            {
                return null;
            }
            JObject rootObject = JObject.Parse(json);
            JToken infoToken = rootObject["beans"].Children().ToList().First();
            var info = infoToken.ToObject<FSNamesystem>();
            return info;
        }
        public async Task<NameNodeInfo> GetNameNodeInfoAsync()
        {
            string queryUrl = $@"http://{MasterNode.Host}:50070/jmx?qry=Hadoop:service=NameNode,name=NameNodeInfo";
            var json = await NetworkManager.FetchStringDataFromUri(new Uri(queryUrl));
            if (json == null)
            {
                return null;
            }
            JObject rootObject = JObject.Parse(json);
            JToken infoToken = rootObject["beans"].Children().ToList().First();
            var info = infoToken.ToObject<NameNodeInfo>();
            return info;
        }
    }

    public class YarnServer : HadoopServer
    {
        public YarnResourceManager ResourceManager { get; set; }
        public List<YarnNodeManager> NodeManager { get; set; }

        public async Task<ClusterInfo> GetClusterInfoAsync()
        {
            string queryUrl = $@"http://{ResourceManager.Host}:8088/ws/v1/cluster/info";
            var json = await NetworkManager.FetchStringDataFromUri(new Uri(queryUrl));
            if (json == null)
            {
                return null;
            }
            JObject rootObject = JObject.Parse(json);
            JToken infoToken = rootObject["clusterInfo"];
            var info = infoToken.ToObject<ClusterInfo>();
            return info;
        }

        public async Task<ClusterMetrics> GetClusterMetricsAsync()
        {
            string queryUrl = $@"http://{ResourceManager.Host}:8088/ws/v1/cluster/metrics";
            var json = await NetworkManager.FetchStringDataFromUri(new Uri(queryUrl));
            if (json == null)
            {
                return null;
            }
            JObject rootObject = JObject.Parse(json);
            JToken infoToken = rootObject["clusterMetrics"];
            var info = infoToken.ToObject<ClusterMetrics>();
            return info;
        }

        public async Task<List<YarnApp>> GetApplications()
        {
            string queryUrl = $@"http://{ResourceManager.Host}:8088/ws/v1/cluster/apps";
            var json = await NetworkManager.FetchStringDataFromUri(new Uri(queryUrl));
            if (json == null)
            {
                return null;
            }
            JObject rootObject = JObject.Parse(json);
            JToken infoToken = rootObject["apps"]["app"];
            var info = infoToken.ToObject<List<YarnApp>>();
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

    public class DistinctVersion
    {
        public string key { get; set; }
        public int value { get; set; }
    }

    public class NameNodeInfo
    {
        public string name { get; set; }
        public string modelerType { get; set; }
        public long Total { get; set; }
        public bool UpgradeFinalized { get; set; }
        public string ClusterId { get; set; }
        public string Version { get; set; }
        public long Used { get; set; }
        public long Free { get; set; }
        public string Safemode { get; set; }
        public long NonDfsUsedSpace { get; set; }
        public double PercentUsed { get; set; }
        public long BlockPoolUsedSpace { get; set; }
        public double PercentBlockPoolUsed { get; set; }
        public double PercentRemaining { get; set; }
        public int CacheCapacity { get; set; }
        public int CacheUsed { get; set; }
        public int TotalBlocks { get; set; }
        public int TotalFiles { get; set; }
        public int NumberOfMissingBlocks { get; set; }
        public int NumberOfMissingBlocksWithReplicationFactorOne { get; set; }
        public string LiveNodes { get; set; }
        public string DeadNodes { get; set; }
        public string DecomNodes { get; set; }
        public string EnteringMaintenanceNodes { get; set; }
        public string BlockPoolId { get; set; }
        public string NameDirStatuses { get; set; }
        public string NodeUsage { get; set; }
        public string NameJournalStatus { get; set; }
        public string JournalTransactionInfo { get; set; }
        public string NNStarted { get; set; }
        public long NNStartedTimeInMillis { get; set; }
        public string CompileInfo { get; set; }
        public string CorruptFiles { get; set; }
        public int NumberOfSnapshottableDirs { get; set; }
        public int DistinctVersionCount { get; set; }
        public List<DistinctVersion> DistinctVersions { get; set; }
        public string SoftwareVersion { get; set; }
        public string NameDirSize { get; set; }
        public object RollingUpgradeStatus { get; set; }
        public int Threads { get; set; }
    }

    public class ClusterInfo
    {
        public long id { get; set; }
        public long startedOn { get; set; }
        public string state { get; set; }
        public string haState { get; set; }
        public string rmStateStoreName { get; set; }
        public string resourceManagerVersion { get; set; }
        public string resourceManagerBuildVersion { get; set; }
        public string resourceManagerVersionBuiltOn { get; set; }
        public string hadoopVersion { get; set; }
        public string hadoopBuildVersion { get; set; }
        public string hadoopVersionBuiltOn { get; set; }
        public string haZooKeeperConnectionState { get; set; }
    }

    public class ClusterMetrics
    {
        public int appsSubmitted { get; set; }
        public int appsCompleted { get; set; }
        public int appsPending { get; set; }
        public int appsRunning { get; set; }
        public int appsFailed { get; set; }
        public int appsKilled { get; set; }
        public int reservedMB { get; set; }
        public int availableMB { get; set; }
        public int allocatedMB { get; set; }
        public int reservedVirtualCores { get; set; }
        public int availableVirtualCores { get; set; }
        public int allocatedVirtualCores { get; set; }
        public int containersAllocated { get; set; }
        public int containersReserved { get; set; }
        public int containersPending { get; set; }
        public int totalMB { get; set; }
        public int totalVirtualCores { get; set; }
        public int totalNodes { get; set; }
        public int lostNodes { get; set; }
        public int unhealthyNodes { get; set; }
        public int decommissioningNodes { get; set; }
        public int decommissionedNodes { get; set; }
        public int rebootedNodes { get; set; }
        public int activeNodes { get; set; }
        public int shutdownNodes { get; set; }
    }

    public class ResourcesUsed
    {
        public int memory { get; set; }
        public int vCores { get; set; }
    }

    public class LeafQueue:Queue
    {
        public int maxActiveApplications { get; set; }
        public int maxActiveApplicationsPerUser { get; set; }
        public int maxApplications { get; set; }
        public int maxApplicationsPerUser { get; set; }
        public int numActiveApplications { get; set; }
        public int numContainers { get; set; }
        public int numPendingApplications { get; set; }
        public string type { get; set; }
        public int userLimit { get; set; }
        public double userLimitFactor { get; set; }
        public object users { get; set; }
    }

    public class User
    {
        public int numActiveApplications { get; set; }
        public int numPendingApplications { get; set; }
        public ResourcesUsed resourcesUsed { get; set; }
        public string username { get; set; }
    }

    public class Users
    {
        public List<User> user { get; set; }
    }

    public class BranchQueue:Queue
    {
        public Queues queues { get; set; }
        public int maxActiveApplications { get; set; }
        public int maxActiveApplicationsPerUser { get; set; }
        public int maxApplications { get; set; }
        public int maxApplicationsPerUser { get; set; }
        public int numActiveApplications { get; set; }
        public int numContainers { get; set; }
        public int numPendingApplications { get; set; }
        public string type { get; set; }
        public int userLimit { get; set; }
        public double userLimitFactor { get; set; }
        public Users users { get; set; }
    }

    public class RootQueue: Queue
    {
        public Queues queues { get; set; }
    }

    public class Queue
    {
        public double absoluteCapacity { get; set; }
        public double absoluteMaxCapacity { get; set; }
        public double absoluteUsedCapacity { get; set; }
        public double capacity { get; set; }
        public double maxCapacity { get; set; }
        public int numApplications { get; set; }
        public string queueName { get; set; }
        public ResourcesUsed resourcesUsed { get; set; }
        public string state { get; set; }
        public double usedCapacity { get; set; }
        public string usedResources { get; set; }
    }

    public class Queues
    {
        public List<Queue> queue { get; set; }
    }

    public class SchedulerInfo
    {
        public double capacity { get; set; }
        public double maxCapacity { get; set; }
        public string queueName { get; set; }
        public Queues queues { get; set; }
        public string type { get; set; }
        public double usedCapacity { get; set; }
    }

    public class Scheduler
    {
        public SchedulerInfo schedulerInfo { get; set; }
    }

    public class Used
    {
        public int memory { get; set; }
        public int vCores { get; set; }
    }

    public class Reserved
    {
        public int memory { get; set; }
        public int vCores { get; set; }
    }

    public class Pending
    {
        public int memory { get; set; }
        public int vCores { get; set; }
    }

    public class AmUsed
    {
        public int memory { get; set; }
        public int vCores { get; set; }
    }

    public class AmLimit
    {
        public int memory { get; set; }
        public int vCores { get; set; }
    }

    public class ResourceUsagesByPartition
    {
        public string partitionName { get; set; }
        public Used used { get; set; }
        public Reserved reserved { get; set; }
        public Pending pending { get; set; }
        public AmUsed amUsed { get; set; }
        public AmLimit amLimit { get; set; }
    }

    public class ResourceInfo
    {
        public List<ResourceUsagesByPartition> resourceUsagesByPartition { get; set; }
    }

    public class AppTimeout
    {
        public string type { get; set; }
        public string expiryTime { get; set; }
        public int remainingTimeInSeconds { get; set; }
    }

    public class Timeouts
    {
        public List<AppTimeout> timeout { get; set; }
    }

    public class Capability
    {
        public int memory { get; set; }
        public int vCores { get; set; }
    }

    public class ExecutionTypeRequest
    {
        public string executionType { get; set; }
        public bool enforceExecutionType { get; set; }
    }

    public class ResourceRequest
    {
        public int priority { get; set; }
        public string resourceName { get; set; }
        public Capability capability { get; set; }
        public int numContainers { get; set; }
        public bool relaxLocality { get; set; }
        public string nodeLabelExpression { get; set; }
        public ExecutionTypeRequest executionTypeRequest { get; set; }
        public bool enforceExecutionType { get; set; }
    }

    public class YarnApp
    {
        public string id { get; set; }
        public string user { get; set; }
        public string name { get; set; }
        public string queue { get; set; }
        public string state { get; set; }
        public string finalStatus { get; set; }
        public double progress { get; set; }
        public string trackingUI { get; set; }
        public string diagnostics { get; set; }
        public long clusterId { get; set; }
        public string applicationType { get; set; }
        public string applicationTags { get; set; }
        public int priority { get; set; }
        public long startedTime { get; set; }
        public long finishedTime { get; set; }
        public long elapsedTime { get; set; }
        public string amContainerLogs { get; set; }
        public string amHostHttpAddress { get; set; }
        public int allocatedMB { get; set; }
        public int allocatedVCores { get; set; }
        public int reservedMB { get; set; }
        public int reservedVCores { get; set; }
        public int runningContainers { get; set; }
        public int memorySeconds { get; set; }
        public int vcoreSeconds { get; set; }
        public double queueUsagePercentage { get; set; }
        public double clusterUsagePercentage { get; set; }
        public int preemptedResourceMB { get; set; }
        public int preemptedResourceVCores { get; set; }
        public int numNonAMContainerPreempted { get; set; }
        public int numAMContainerPreempted { get; set; }
        public long preemptedMemorySeconds { get; set; }
        public long preemptedVcoreSeconds { get; set; }
        public string logAggregationStatus { get; set; }
        public bool unmanagedApplication { get; set; }
        public string amNodeLabelExpression { get; set; }
        public ResourceInfo resourceInfo { get; set; }
        public Timeouts timeouts { get; set; }
        public List<ResourceRequest> resourceRequests { get; set; }
    }
}
