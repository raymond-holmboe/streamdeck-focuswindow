using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synkrono.FocusWindow
{
    public class PluginSettings
    {
        public static PluginSettings CreateDefaultSettings()
        {
            PluginSettings instance = new PluginSettings
            {
                Process = string.Empty,
                Processes = new List<ProcessListItem>(),
                ChildWindows = new List<ChildWindow>(),
                ChildWindow = string.Empty,
                RestoreWindow = false
            };

            return instance;
        }

        [JsonProperty(PropertyName = "processes")]
        public List<ProcessListItem> Processes { get; set; }

        [JsonProperty(PropertyName = "process")]
        public string Process { get; set; }

        [JsonProperty(PropertyName = "childWindows")]
        public List<ChildWindow> ChildWindows { get; set; }

        [JsonProperty(PropertyName = "childWindow")]
        public string ChildWindow { get; set; }

        [JsonProperty(PropertyName = "restoreWindow")]
        public bool RestoreWindow { get; set; }
    }
}
