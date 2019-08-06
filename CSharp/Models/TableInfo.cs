using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Cbs.oData.TableConsole
{

    public class TableInfoViewModel
    {
        [JsonProperty("odata.metadata")]
        public string Metadata { get; set; }

        [JsonProperty("value")]
        public IList<TableInfo> TableInfos { get; set; }
    }


    public class TableInfo
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string ShortTitle { get; set; }
        public string Identifier { get; set; }
        public string Summary { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Updated { get; set; }
        public string ReasonDelivery { get; set; }
        public string ExplanatoryText { get; set; }
        public string Language { get; set; }
        public string Catalog { get; set; }
        public string Frequency { get; set; }
        public string Period { get; set; }
        public string ShortDescription { get; set; }
        public string DefaultPresentation { get; set; }
        public string GraphTypes { get; set; }
        public string OutputStatus { get; set; }
        public string Source { get; set; }
        public DateTime? MetaDataModified { get; set; }
        public string ApiUrl { get; set; }
        public string FeedUrl { get; set; }
        public string DefaultSelection { get; set; }
        public int RecordCount { get; set; }
        public int ColumnCount { get; set; }
        public int SearchPriority { get; set; }
    }

}

