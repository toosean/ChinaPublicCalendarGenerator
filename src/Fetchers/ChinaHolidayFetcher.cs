using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChinaPublicCalendarGenerator.Fetchers
{
    [FetchShortName("holiday")]
    class ChinaHolidayFetcher : JsonReaderFetcherBase
    {
        protected override string GetCalendarName() => "中国节假日";

        protected override string GetJsonPath(int year)
            => string.Format("Data/ChinaHoliday_{0:0000}.json", year);
    }
}
