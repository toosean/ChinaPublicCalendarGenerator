using ChinaPublicCalendarGenerator.Fetchers.Abstraction;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChinaPublicCalendarGenerator.Fetchers
{
    [FetchShortName("epic")]
    class EpicFreeGameFetcher : IFetcher
    {
        private const string EPICDataUrl = "https://store-site-backend-static.ak.epicgames.com/freeGamesPromotions";

        public IHttpClientFactory HttpClientFactory { get; }

        public EpicFreeGameFetcher(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<CalendarEventCollection> FetchAsync(DateTime begin, DateTime end)
        {
            var result = new List<CalendarEvent>();

            using (var client = HttpClientFactory.CreateClient())
            {
                var html = await client.GetStringAsync(EPICDataUrl);
                var root = JToken.Parse(html);

                var elements = root["data"]?["Catalog"]?["searchStore"]?["elements"] ?? throw new Exception();

                result.AddRange(elements

                .Where(element => element["promotions"]?.Any() ?? false)
                .Select(element => new
                {
                    element = element,
                    offers = ((element["promotions"]?["promotionalOffers"]?.Any() ?? false) ?
                                element["promotions"]?["promotionalOffers"] :
                                element["promotions"]?["upcomingPromotionalOffers"])
                             ?[0]?["promotionalOffers"]?[0]
                }).Select(e => new CalendarEvent
                {
                    Title = e.element["title"]?.ToString() ?? throw new Exception(),
                    Begin = DateTimeOffset.Parse(e.offers?["startDate"]?.ToString() ?? throw new Exception(), null, System.Globalization.DateTimeStyles.AssumeUniversal).LocalDateTime,
                    End = DateTimeOffset.Parse(e.offers?["endDate"]?.ToString() ?? throw new Exception(), null, System.Globalization.DateTimeStyles.AssumeUniversal).LocalDateTime
                }).OrderBy(o => o.Begin));
            }

            return new CalendarEventCollection(result) { Name = "EPIC 免费游戏" };
        }
    }
}
