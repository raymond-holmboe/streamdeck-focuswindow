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
                ApplicationName = String.Empty,
                TitleFilter = String.Empty,
                RestoreWindow = false
            };

            return instance;
        }

        [JsonProperty(PropertyName = "applicationName")]
        public string ApplicationName { get; set; }

        [JsonProperty(PropertyName = "titleFilter")]
        public string TitleFilter { get; set; }

        [JsonProperty(PropertyName = "restoreWindow")]
        public bool RestoreWindow { get; set; }
    }
}
