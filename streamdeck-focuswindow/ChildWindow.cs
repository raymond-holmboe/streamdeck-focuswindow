using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Synkrono.FocusWindow
{
    public class ChildWindow
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        public IntPtr WindowsHandle { get; set; }
    }
}
