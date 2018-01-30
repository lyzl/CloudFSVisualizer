using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudFSVisualizer.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public static async Task<NodeOperatingSystem> FetchNodeOperatingSystemInfo(HadoopNode node, string query)
        {
            string queryUrl;
            NodeOperatingSystem Info;

            if (query == null)
            {
                queryUrl = node.Host;
            }
            else
            {
                queryUrl = string.Format(@"http://{0}/jmx?qry={1}", node.Host, query);
            }

            var serializer = new JsonSerializer();
            using (var stream = await NetworkManager.FetchStreamDataFromUri(new Uri(query)))
            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                if (stream != null)
                {
                    jsonTextReader.Read();
                    Info = serializer.Deserialize<NodeOperatingSystem>(jsonTextReader);
                }
                else
                {
                    Info = null;
                }
            }
            return Info;
        }

    }
}
