using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChinaPublicCalendarGenerator.Fetchers
{
    abstract class JsonReaderFetcherBase : IFetcher
    {
        protected abstract string GetJsonPath(int year);

        public Task<CalendarEventCollection> FetchAsync(DateTime since)
        {
            var result = new List<CalendarEvent>();

            for(var year = since.Year;year <= DateTime.Now.Year; year++)
            {
                var content = File.ReadAllBytes(string.Format(GetJsonPath(year), year));
                var events = JsonSerializer.Deserialize<CalendarEvent[]>(content);

                result.AddRange(events);
            }

            return Task.FromResult(new CalendarEventCollection(result));

        }
            
    }
}
