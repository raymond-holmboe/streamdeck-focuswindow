using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Synkrono.FocusWindow
{
    public class ProcessListItem
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
