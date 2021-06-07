using ChinaPublicCalendarGenerator.Fetchers.Abstraction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChinaPublicCalendarGenerator.Fetchers
{
    [FetchShortName("tradition-festival")]
    class ChinaTraditionFestivalFetcher : JsonReaderFetcherBase
    {
        protected override string GetCalendarName() => "中国传统节日";

        protected override string GetJsonPath(int year)
            => string.Format("Data/ChinaTraditionFestival_{0:0000}.json", year);
    }
}
