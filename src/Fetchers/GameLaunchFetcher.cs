using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChinaPublicCalendarGenerator.Fetchers
{
    [FetchShortName("game-launch")]
    class GameLaunchFetcher : CacheableFetcherBase
    {
        private const string FetchInitUrl = "http://www.vgtime.com/game/release.jhtml?pubtime={0:yyyy-MM}";

        public GameLaunchFetcher(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public IHttpClientFactory HttpClientFactory { get; }

        protected override string? GetCalendarName() => "游戏发售时间表";

        protected override async Task FetchOnCachedAsync(DateTime since, IList<CalendarEvent> cachedEvents)
        {
            for(var monthCounter = 0;monthCounter < 3; monthCounter++)
            {
                var fetchDate = DateTime.Today.AddMonths(monthCounter);

                using (var client = HttpClientFactory.CreateClient())
                using (var stream = await client.GetStreamAsync(string.Format(FetchInitUrl,fetchDate)))
                {
                    var doc = new HtmlDocument();
                    doc.Load(stream);

                    var result = doc.QuerySelectorAll("div.page_list li.vg_ss_box")
                        .Select(item => new CalendarEvent
                        {
                            Title = item.QuerySelector(".info_box h2").InnerText,
                            Begin = DateTime.Parse(item.QuerySelector(".ss_tit").InnerText),
                            IsWholeDay = true,
                            Description = item.QuerySelector(".old_name").InnerText
                                            + "\r\n"
                                            + item.QuerySelectorAll("span.platform_detail")
                                                .Select(span => span.Attributes["data-cn"]?.Value == "true"
                                                                    ? $"[{span.InnerText}|中]"
                                                                    : $"[{span.InnerText}]")
                                                .Aggregate((a, b) => $"{a} {b}")
                        });

                    foreach (var one in result) cachedEvents.Add(one);
                }

            }

        }
    }
}
