using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChinaPublicCalendarGenerator.Fetchers.Abstraction
{
    abstract class JsonReaderFetcherBase : IFetcher
    {
        protected abstract string GetCalendarName();
        protected abstract string GetJsonPath(int year);

        public Task<CalendarEventCollection> FetchAsync(DateTime begin, DateTime end)
        {
            var result = new List<CalendarEvent>();

            for(var year = begin.Year;year <= end.Year; year++)
            {
                var content = File.ReadAllBytes(string.Format(GetJsonPath(year), year));
                var events = JsonSerializer.Deserialize<CalendarEvent[]>(content)!;

                result.AddRange(events);
            }

            return Task.FromResult(new CalendarEventCollection(result)
            {
                Name = GetCalendarName()
            });

        }
            
    }
}
