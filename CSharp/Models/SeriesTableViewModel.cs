using Newtonsoft.Json;
using System.Data;

namespace Cbs.oData.TableConsole
{
    /// <summary>
    /// Parse oData 3 Typed or Untyped data
    /// </summary>
    public class SeriesTableViewModel
    {
        [JsonProperty("odata.metadata")]
        public string Metadata { get; set; }

        [JsonProperty("value")]
        public DataTable Data { get; set; }

        [JsonIgnore]
        public TableInfo TableInfo { get; set; }

        [JsonIgnore]
        public string TableId { get; set; }


    }
}