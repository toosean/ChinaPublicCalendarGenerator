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

        protected abstract Task<IEnumerable<CalendarEvent>> FetchOnCachedAsync(DateTime since);

        public async Task<CalendarEventCollection> FetchAsync(DateTime since)
        {
            var cachedFilePath = GetCachedPath();

            var cached = File.Exists(cachedFilePath)
                ? JsonSerializer.Deserialize<List<CalendarEvent>>(File.ReadAllText(cachedFilePath))
                : new List<CalendarEvent>();

            CacheMergeFrom(cached, await FetchOnCachedAsync(since));

            File.WriteAllText(GetCachedPath(), JsonSerializer.Serialize(cached));

            return new CalendarEventCollection(cached.Where(w => w.Begin >= since)) { Name = GetCalendarName() };
        }

        protected virtual void CacheMergeFrom(IList<CalendarEvent> cachedEvents, IEnumerable<CalendarEvent> events
            , Func<CalendarEvent, CalendarEvent, bool>? comparer = null)
        {

            var actComparer = comparer ?? DefaultEventCpmparer;

            foreach (var _event in events)
            {
                for (var index = cachedEvents.Count - 1; index >= 0; index--)
                {
                    if (actComparer(cachedEvents[index], _event))
                    {
                        cachedEvents.RemoveAt(index);
                        cachedEvents.Insert(index, _event);
                    }
                }
                cachedEvents.Add(_event);
            }
        }

        private bool DefaultEventCpmparer(CalendarEvent a, CalendarEvent b) => a.Title == b.Title;

    }
}
