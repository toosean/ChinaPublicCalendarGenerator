using ChinaPublicCalendarGenerator.Fetchers.Abstraction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChinaPublicCalendarGenerator.Fetchers
{
    [FetchShortName("international-festival")]
    class ChinaInternationalFestivalFetcher : JsonReaderFetcherBase
    {
        protected override string GetCalendarName() => "中国公众/国际节日";

        protected override string GetJsonPath(int year)
            => string.Format("Data/ChinaInternationalFestival_{0:0000}.json", year);
    }
}
