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

        public static async Task<NodeStatus> GetNodeConnectionStatus(HadoopNode node)
        {

            NodeStatus status = NodeStatus.Unknown;

            try
            {
                using (var tcpClient = new StreamSocket())
                {
                    await tcpClient.ConnectAsync(
                        new Windows.Networking.HostName(node.Host),
                        node.ServicePort.ToString(),
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
