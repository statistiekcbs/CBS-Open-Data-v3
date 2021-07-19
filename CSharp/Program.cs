using System;
using ConsoleTableExt;

namespace Cbs.oData.TableConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string url = "https://opendata.cbs.nl/ODataApi/odata/37296ned/UntypedDataSet?$select=Perioden,+TotaleBevolking_1,+Mannen_2,+Vrouwen_3";

            ODataComposer oDataComposer = new ODataComposer(url);

            SeriesTableViewModel seriesTableViewModel = oDataComposer.GetTable();

            Console.WriteLine(seriesTableViewModel.TableInfo.ShortTitle);

            Console.WriteLine(new String('-', 40));

            ConsoleTableBuilder
                   .From(seriesTableViewModel.Data)
                   .ExportAndWriteLine();
        }
    }
}
