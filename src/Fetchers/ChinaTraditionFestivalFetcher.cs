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
        protected override string GetJsonPath(int year)
            => string.Format("Data/ChinaSolarTerms_{0:0000}.json", year);
    }
}
