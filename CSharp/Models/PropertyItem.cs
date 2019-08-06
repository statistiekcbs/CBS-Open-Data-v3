using Newtonsoft.Json;
using System.Collections.Generic;

namespace Cbs.oData.TableConsole
{
    public class PropertyItems
    {
        [JsonProperty("odata.metadata")]
        public string Metadata { get; set; }

        [JsonProperty("value")]
        public IList<PropertyItem> Items { get; set; }
    }

    public class PropertyItem
    {
        public string Key { get; set; } // "2006MM01",
        public string Title { get; set; } // "2006 december",
        public string Description { get; set; } // null,
        public string Status { get; set; } // "Voorlopig"
    }
}