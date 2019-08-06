using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Cbs.oData.TableConsole
{
    public class ODataComposer
    {
        #region properties
        private string _url;
        private readonly string _tableId;
        private readonly string _baseUrl;
        private readonly string _catalogUrl;
        private readonly bool _transpose;

        public string Url
        {
            get { return _url; }
        }

        public string TableId
        {
            get { return _tableId; }
        }
        #endregion

        public ODataComposer(string url, bool transpose = false)
        {


            // get user settings
            _url = url;
            _transpose = transpose;
            
            // get base URL's
            _catalogUrl = "https://opendata.cbs.nl/ODataCatalog/";
            _baseUrl = "https://opendata.cbs.nl/ODataApi/odata/";

            // extract the table Id
            if (!string.IsNullOrEmpty(_url) && IsValidUrl(_url))
            {
                string[] urlParts = _url.Split('/');

                int tableIdIndex = 5;

                if (urlParts.Length >= tableIdIndex)
                {
                    _tableId = urlParts[tableIdIndex];
                }
            }
            else
            {
                Console.WriteLine("oData Table URL is not valid");
                return;
            }

            if (!string.IsNullOrEmpty(_tableId))
            {
                DetectPeriods();
            }
            else
            {
                Console.WriteLine("Cannot extract table id from oData URL : {0}", _url);
                return;
            }

        }

        public SeriesTableViewModel GetTable()
        {
            if (string.IsNullOrEmpty(_tableId))
            {
                return null;
            }

            PropertyItems regions = null;

            string json = GetJson(string.Format("{0}Tables?$format=json&$filter=(Identifier%20eq%20%27{1}%27)", _catalogUrl, _tableId));
            TableInfoViewModel tableInfo = Deserialize<TableInfoViewModel>(json);

            json = GetJson(_url);
            SeriesTableViewModel seriesTableViewModel = Deserialize<SeriesTableViewModel>(json);

            string propertiesUrl = string.Format("{0}{1}/DataProperties?$format=json", _baseUrl, _tableId);
            json = GetJson(propertiesUrl);
            DataPropertiesViewModel properties = Deserialize<DataPropertiesViewModel>(json);

            if ((tableInfo == null) || (properties == null) || (seriesTableViewModel == null))
            {
                return null;
            }

            // Parse properties
            var regionAvailable = properties.DataProperties.FirstOrDefault(s => s.Datatype == "GeoDimension");
            if (regionAvailable != null)
            {
                string regionKey = regionAvailable.Key;

                json = GetJson(string.Format("{0}{1}/{2}?$format=json", _baseUrl, _tableId, regionKey));
                regions = Deserialize<PropertyItems>(json);
            }

            foreach (DataColumn col in seriesTableViewModel.Data.Columns)
            {
                var propertyColumn = properties.DataProperties.FirstOrDefault(s => s.Key == col.ColumnName);

                if (propertyColumn != null)
                {
                    col.ColumnName = string.Format("{0} {1}", propertyColumn.Title, string.IsNullOrEmpty(propertyColumn.Unit) ? string.Empty : string.Format("({0})", propertyColumn.Unit));

                    if (propertyColumn.Type == "TimeDimension")
                    {
                        foreach (DataRow row in seriesTableViewModel.Data.Rows)
                        {
                            string periode = row[col.ColumnName].ToString();
                            row[col.ColumnName] = ParsePeriod(periode);
                        }
                    }

                    if ((propertyColumn.Type == "GeoDimension") && (regions != null))
                    {
                        foreach (DataRow row in seriesTableViewModel.Data.Rows)
                        {
                            string region = row[col.ColumnName].ToString();
                            var regionName = regions.Items.FirstOrDefault(x => x.Key == region);
                            if (regionName != null)
                            {
                                row[col.ColumnName] = regionName.Title;
                            }
                        }
                    }
                }
            }

            if (_transpose)
            {
                seriesTableViewModel.Data = Transpose(seriesTableViewModel.Data);
            }

            if ((tableInfo.TableInfos != null) && (tableInfo.TableInfos.Any()))
            {
                seriesTableViewModel.TableInfo = tableInfo.TableInfos[0];
            }
            seriesTableViewModel.TableId = _tableId;

            return seriesTableViewModel;
        }


        #region private helper methods

        /// <summary>
        /// Transform CBS specific formatting
        /// </summary>
        /// <param name="period"></param>
        private static string ParsePeriod(string period)
        {
            if (string.IsNullOrEmpty(period))
                return string.Empty;

            if (period.Contains("JJ"))
            {
                return period.Substring(0, 4);
            }

            if (period.Contains("MM"))
            {
                int i = period.IndexOf("MM", StringComparison.InvariantCulture);
                int m = Convert.ToInt32(period.Substring(period.Length - 2, 2));
                int y = Convert.ToInt32(period.Remove(i));
                DateTime p = new DateTime(y, m, 1);
                return p.ToString("MMM yyyy");
            }
            return period;
        }

        public static string GetJson(string oDataUrl, bool force = true)
        {
            if (string.IsNullOrEmpty(oDataUrl))
            {
                Console.WriteLine("oData URL not provided");
                return string.Empty;
            }

            // force JSON
            if (!oDataUrl.Contains("$format=json") && force)
                oDataUrl = string.Format("{0}&$format=json", oDataUrl);

            var jsonData = string.Empty;
            using (var w = new WebClient())
            {
                try
                {
                    w.Headers["Content-Type"] = "application/json";
                    jsonData = w.DownloadString(oDataUrl);
                }
                catch
                {
                    Console.WriteLine("Cannot download JSON from {0}", oDataUrl);
                    return string.Empty;
                }
            }
            return jsonData;
        }

        /// <summary>
        /// Check for ending slash.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string FormatBaseUrl(string url)
        {
            if (!string.IsNullOrEmpty(url) && !url.EndsWith("/",StringComparison.InvariantCulture))
            {
                return string.Format("{0}/", url);
            }
            return url;
        }

        /// <summary>
        /// Parse URL, get special tags and replace with latest available date
        /// Supported :
        ///     [currentmonth]
        ///     [previousmonth]
        ///     [lastyearmonth]
        ///     [currentyear]
        ///     [lastyear]
        /// </summary>
        private void DetectPeriods()
        {
            if (!string.IsNullOrEmpty(_tableId) && (_url.Contains("Perioden") || _url.Contains("Periods")) && (_url.Contains("previousmonth") || _url.Contains("currentmonth") || _url.Contains("lastyearmonth") || _url.Contains("currentyear") || _url.Contains("lastyear")))
            {
                // English Periods data differs from Dutch
                string periodUrl = _url.Contains("Periods") ? string.Format("{0}{1}/Periods?$format=json", _baseUrl, _tableId) : string.Format("{0}{1}/Perioden?$format=json", _baseUrl, _tableId);

                string json = GetJson(periodUrl);
                PropertyItems propertyItems = Deserialize<PropertyItems>(json);

                if ((propertyItems != null) && (propertyItems.Items != null) && (propertyItems.Items.Any()))
                {
                    List<PropertyItem> searchItems = propertyItems.Items.OrderByDescending(x => x.Key).Where(p => p.Key.Contains("MM")).ToList();

                    if (searchItems.Any())
                    {
                        string currentmonth = searchItems[0].Key;
                        _url = _url.Replace("[currentmonth]", currentmonth);

                        if (propertyItems.Items.Count > 1)
                        {
                            string previousmonth = searchItems[1].Key;
                            _url = _url.Replace("[previousmonth]", previousmonth);
                        }

                        int year = Convert.ToInt32(currentmonth.Substring(0, 4));
                        year = year - 1;

                        string lastyearmonth = currentmonth.Replace(currentmonth.Substring(0, 4), year.ToString());
                        _url = _url.Replace("[lastyearmonth]", lastyearmonth);
                    }

                    searchItems = propertyItems.Items.OrderByDescending(x => x.Key).Where(p => p.Key.Contains("JJ")).ToList();

                    if (searchItems.Any())
                    {
                        _url = _url.Replace("[currentyear]", searchItems[0].Key);

                        if (propertyItems.Items.Count > 1)
                        {
                            _url = _url.Replace("[previousyear]", searchItems[1].Key);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Could not parse period table {0}", periodUrl);
                }
            }
        }

        /// <summary>
        /// Rotate a Data table
        /// </summary>
        public static DataTable Transpose(DataTable dt)
        {
            if (dt == null)
                return null;

            DataTable dtNew = new DataTable();

            for (int i = 0; i <= dt.Rows.Count; i++)
            {
                dtNew.Columns.Add(i.ToString());
            }

            dtNew.Columns[0].ColumnName = " ";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    dtNew.Columns[i + 1].ColumnName = dt.Rows[i].ItemArray[0].ToString();
                }
                catch
                {
                    //
                }
            }

            for (int k = 1; k < dt.Columns.Count; k++)
            {
                DataRow r = dtNew.NewRow();
                r[0] = dt.Columns[k].ToString();
                for (int j = 1; j <= dt.Rows.Count; j++)
                    r[j] = dt.Rows[j - 1][k];
                dtNew.Rows.Add(r);
            }

            return dtNew;
        }


        private static bool IsValidUrl(string url)
        {
            Uri uriResult;
            return Uri.TryCreate(url, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }


        /// <summary>
        /// Deserialize JSON object
        /// </summary>
        public static T Deserialize<T>(string content)
        {
            if (string.IsNullOrEmpty(content))
                return default(T);

            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer()
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace,
                MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };

            using (var stringReader = new StringReader(content))
            {
                using (var jsonTextReader = new JsonTextReader(stringReader))
                {
                    return serializer.Deserialize<T>(jsonTextReader);
                }
            }
        }

        #endregion

    }
}