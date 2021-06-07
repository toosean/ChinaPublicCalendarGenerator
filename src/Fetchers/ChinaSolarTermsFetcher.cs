using ChinaPublicCalendarGenerator.Fetchers.Abstraction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChinaPublicCalendarGenerator.Fetchers
{
    [FetchShortName("solar-terms")]
    class ChinaSolarTermsFetcher : JsonReaderFetcherBase
    {

        protected override string GetCalendarName() => "中国二十四节气";

        protected override string GetJsonPath(int year)
            => string.Format("Data/ChinaSolarTerms_{0:0000}.json", year);
    }
}
