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
    class EpicFreeGameFetcher : CacheableFetcherBase
    {
        private const string EPICDataUrl = "https://store-site-backend-static.ak.epicgames.com/freeGamesPromotions";

        protected override string? GetCalendarName() => "EPIC 免费游戏";

        public IHttpClientFactory HttpClientFactory { get; }

        public EpicFreeGameFetcher(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        protected override async Task<IEnumerable<CalendarEvent>> FetchBaseCachedAsync(DateTime begin, DateTime end)
        {
            var result = new List<CalendarEvent>();

            using (var client = HttpClientFactory.CreateClient())
            {
                var html = await client.GetStringAsync(EPICDataUrl);
                var root = JToken.Parse(html);

                var elements = root["data"]?["Catalog"]?["searchStore"]?["elements"] ?? throw new Exception();

                //即将到来的免费游戏是第二个元素
                var element = elements.Skip(1).First();
                var offers = element["promotions"]?["upcomingPromotionalOffers"]?[0]?["promotionalOffers"]?[0];

                var startDate = DateTime.Parse(offers?["startDate"]?.ToString() ?? throw new Exception());
                var endDate = DateTime.Parse(offers?["endDate"]?.ToString() ?? throw new Exception());

                result.Add(new CalendarEvent
                {
                    Title = element["title"]?.ToString() ?? throw new Exception(),
                    Begin = startDate.Date,
                    End = endDate.Date,
                    IsWholeDay = true
                });
            }

            return result;
        }


    }
}
