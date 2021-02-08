using ChinaPublicCalendarGenerator.Fetchers.Abstraction;
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
    [FetchShortName("movie-coming")]
    class MovieComingFetcher : CacheableFetcherBase
    {
        private const string DouBanUrl = "https://movie.douban.com/coming";

        public IHttpClientFactory HttpClientFactory { get; }

        public MovieComingFetcher(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        protected override string? GetCalendarName() => "内地电影上映日期";

        protected override async Task<IEnumerable<CalendarEvent>> FetchOnCachedAsync(DateTime since)
        {

            var result = new List<CalendarEvent>();

            using (var client = HttpClientFactory.CreateClient())
            using (var stream = await client.GetStreamAsync(DouBanUrl))
            {
                var doc = new HtmlDocument();
                doc.Load(stream);

                var regex = new Regex(@"(\d+)月(\d+)日");

                foreach (var tr in doc.QuerySelectorAll(".coming_list tbody tr"))
                {
                    var tdCollection = tr.Descendants("td").ToArray();

                    var title = tdCollection[1].QuerySelector("a").InnerText;
                    var date = tdCollection[0].InnerText.Trim();

                    var link = tdCollection[1].QuerySelector("a").Attributes["href"];

                    var match = regex.Match(date);

                    if (match.Success)
                    {
                        var actDate = new DateTime(DateTime.Now.Year,
                            Convert.ToInt32(match.Groups[1].Value),
                            Convert.ToInt32(match.Groups[2].Value));

                        result.Add(new CalendarEvent
                        {
                            Title = title,
                            Begin = actDate,
                            Description = link.Value,
                            IsWholeDay = true
                        });
                    }
                }
            }

            return result;
        }
    }
}
