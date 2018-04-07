using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudFSVisualizer.Model
{
    public class HadoopAuthentication
    {
        public string User { get; set; }
        public AuthenticationType Type { get; set; }

        public HadoopAuthentication(string user)
        {
            this.User = user;
            Type = AuthenticationType.Off;
        }
    }

    public enum AuthenticationType
    {
        Off, Kerberos, DelegationToken
    }

}
