#nullable disable

using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChinaPublicCalendarGenerator.Fetchers
{
    [FetchShortName("nba-tournament")]
    class NBATournamentFetcher : CacheableFetcherBase
    {
        private const string Url = "https://china.nba.com/static/data/season/schedule_{0:yyyy}_{0:MM}.json";

        public IHttpClientFactory HttpClientFactory { get; }

        public NBATournamentFetcher(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        protected override string? GetCalendarName() => "NBA赛程";

        protected override async Task<IEnumerable<CalendarEvent>> FetchOnCachedAsync(DateTime since)
        {
            var result = new List<CalendarEvent>();

            for (var index = 0; index < 3; index++)
            {
                using (var client = HttpClientFactory.CreateClient())
                {
                    var jsonContent = await client.GetStringAsync(string.Format(Url, DateTime.Today.AddMonths(index)));

                    JToken root = JToken.Parse(jsonContent);

                    result.AddRange(root["payload"]["dates"]
                        .SelectMany(s => s["games"])
                        .Select(token => new CalendarEvent
                        {
                            Title = string.Format("{0} vs {1}"
                                , token["awayTeam"]["profile"]["name"].Value<string>()
                                , token["homeTeam"]["profile"]["name"].Value<string>()),
                            Begin = DateTimeOffset.FromUnixTimeMilliseconds( token["profile"]["utcMillis"].Value<long>()).LocalDateTime,
                            End = DateTimeOffset.FromUnixTimeMilliseconds(token["profile"]["utcMillis"].Value<long>()).LocalDateTime.AddMinutes(90)
                        }));
                }
            }

            return result;
        }
    }
}
