using Newtonsoft.Json;
using System.Collections.Generic;

namespace Cbs.oData.TableConsole
{
    public class DataPropertiesViewModel
    {
        [JsonProperty("odata.metadata")]
        public string Metadata { get; set; }

        [JsonProperty("value")]
        public IList<DataProperty> DataProperties { get; set; }
    }

    public class DataProperty
    {
        public int ID { get; set; }
        public int Position { get; set; }
        public int ParentID { get; set; }
        public string Type { get; set; }
        public string Key { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Datatype { get; set; }
        public string Unit { get; set; }
        public int Decimals { get; set; }
        public string Default { get; set; }
    }
}