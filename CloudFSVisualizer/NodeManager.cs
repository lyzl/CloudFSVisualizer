using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudFSVisualizer.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Networking.Sockets;

namespace CloudFSVisualizer
{
    public class NodeManager
    {
        
        public static List<Node> GetNodeList()
        {
            var node = new HadoopNode()
            {
                Description = "default node description",
                Host = "172.18.84.45:50070"
            };
            var nodeList = new List<Node>();
            nodeList.Add(node);
            nodeList.Add(node);
            nodeList.Add(node);
            return nodeList;
        }

        public static async Task<NodeOperatingSystem> FetchNodeOperatingSystemInfo(HadoopNode node)
        {
            string queryUrl = $@"http://{node.Host}:50070/jmx?qry=java.lang:type=OperatingSystem";
            var json = await NetworkManager.FetchStringDataFromUri(new Uri(queryUrl));
            JObject rootObject = JObject.Parse(json);
            JToken infoToken = rootObject["beans"].Children().ToList().First();
            var info = infoToken.ToObject<NodeOperatingSystem>();
            return info;
        }

        public static async Task<NodeStatus> GetNodeConnectionStatus(Node node)
        {

            NodeStatus status = NodeStatus.Unknown;
            string port;
            if (node is HDFSMasterNode)
            {
                port = "50070";
            }
            else if (node is HDFSSlaverNode)
            {
                port = "50075";
            }
            else
            {
                port = "80";
            }

            try
            {
                using (var tcpClient = new StreamSocket())
                {
                    await tcpClient.ConnectAsync(
                        new Windows.Networking.HostName(node.Host),
                        port,
                        SocketProtectionLevel.PlainSocket).AsTask();

                    var localIp = tcpClient.Information.LocalAddress.DisplayName;
                    var remoteIp = tcpClient.Information.RemoteAddress.DisplayName;
                    tcpClient.Dispose();
                }
                status = NodeStatus.Available;
            }
            catch (Exception)
            {
                status = NodeStatus.Timeout;
            }

            return status;
        }

    }
}
