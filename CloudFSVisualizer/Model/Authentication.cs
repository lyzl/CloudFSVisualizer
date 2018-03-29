using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudFSVisualizer.Model
{
    public class Authentication
    {
        public string User { get; set; }
        public AuthenticationType type { get; set; }
    }

    public enum AuthenticationType
    {
        Off, Kerberos, DelegationToken
    }

}
