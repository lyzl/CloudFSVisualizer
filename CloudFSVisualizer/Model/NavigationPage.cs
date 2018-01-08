using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CloudFSVisualizer.Model
{
    class NavigationPage
    {
        public IconElement Icon { get; set; }
        public String Desc { get; set; }
        public Type Dest { get; set; }

        NavigationPage(IconElement icon, String desc, Type dest)
        {
            this.Icon = icon;
            this.Desc = desc;
            this.Dest = dest;
        }
    }
}
