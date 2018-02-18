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
    public class HDFSFileManager
    {


        public static async Task<List<HDFSFile>> ListDirectory(HDFSFile file)
        {
            var address = $@"http://{file.ServerHost}:50070/webhdfs/v1/{file.Path}?op=LISTSTATUS";
            var json = await NetworkManager.FetchStringDataFromUri(new Uri(address));
            JObject rootObject = JObject.Parse(json);
            IList<JToken> statusTokens = rootObject["FileStatuses"]["FileStatus"].Children().ToList();
            List<FileStatus> statusList = new List<FileStatus>();
            List<HDFSFile> fileList = new List<HDFSFile>();
            foreach (var token in statusTokens)
            {
                var result = token.ToObject<FileStatus>();
                fileList.Add(new HDFSFile(file, result.pathSuffix));
            }

            return fileList;
        }

    }
}
