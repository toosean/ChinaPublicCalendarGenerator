using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChinaPublicCalendarGenerator.Fetchers
{
    abstract class CacheableFetcherBase : IFetcher
    {
        protected virtual string GetCachedPath()
            => this.GetType().Name + "Cached";

        protected virtual string? GetCalendarName() => null;

        protected abstract Task FetchOnCachedAsync(DateTime since, IList<CalendarEvent> cachedEvents);

        public async Task<CalendarEventCollection> FetchAsync(DateTime since)
        {
            var cachedFilePath = GetCachedPath();

            var cached = File.Exists(cachedFilePath)
                ? JsonSerializer.Deserialize<List<CalendarEvent>>(File.ReadAllText(cachedFilePath))
                : new List<CalendarEvent>();

            await FetchOnCachedAsync(since, cached);

            File.WriteAllText(GetCachedPath(), JsonSerializer.Serialize(cached));

            return new CalendarEventCollection(cached.Where(w => w.Begin >= since)) { Name = GetCalendarName() };
        }
    }
}
