using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudFSVisualizer.Model;
using Newtonsoft.Json;

namespace CloudFSVisualizer
{
    public class HDFSFileManager
    {
        public static async Task<FileStatus> GetFileStatus( HDFSFile file)
        {
            var address = $@"http://{file.ServerHost}/webhdfs/v1/{file.Path}?op=GETFILESTATUS";
            var json = await NetworkManager.FetchStringDataFromUri(new Uri(address));
            var dyn = JsonConvert.DeserializeObject<dynamic>(json);
            var status = dyn["FileStatus"];
            return status;
        }
    }
}
