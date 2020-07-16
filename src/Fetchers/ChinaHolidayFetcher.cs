using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChinaPublicCalendarGenerator.Fetchers
{
    [FetchShortName("holiday")]
    class ChinaHolidayFetcher : IFetcher
    {
        private const string JSONFilePath = "Data/ChinaHoliday_{0:0000}.json";

        public Task<CalendarEventCollection> FetchAsync(DateTime since)
        {
            var result = new List<CalendarEvent>();

            for(var year = since.Year;year <= DateTime.Now.Year; year++)
            {
                var content = File.ReadAllBytes(string.Format(JSONFilePath, year));
                var events = JsonSerializer.Deserialize<CalendarEvent[]>(content);

                result.AddRange(events);
            }

            return Task.FromResult(new CalendarEventCollection(result));

        }
            
    }
}
