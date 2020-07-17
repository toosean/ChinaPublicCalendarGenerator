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
    class MovieComingFetcher : IFetcher
    {
        private const string DataCachedFile = "MovieComingCache";
        private const string DouBanUrl = "https://movie.douban.com/coming";

        public IHttpClientFactory HttpClientFactory { get; }

        public MovieComingFetcher(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }


        public async Task<CalendarEventCollection> FetchAsync(DateTime since)
        {
            var cached = File.Exists(DataCachedFile)
                ? JsonSerializer.Deserialize<List<CalendarEvent>>(File.ReadAllText(DataCachedFile))
                : new List<CalendarEvent>();

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

                    var match = regex.Match(date);

                    if (match.Success)
                    {
                        var actDate = new DateTime(DateTime.Now.Year,
                            Convert.ToInt32(match.Groups[1].Value),
                            Convert.ToInt32(match.Groups[2].Value));

                        var cachedItem = cached.FirstOrDefault(f => f.Title == title);
                        if (cachedItem == null)
                        {
                            cached.Add(new CalendarEvent
                            {
                                Title = title,
                                Begin = actDate,
                                IsWholeDay = true
                            });
                        }
                        else
                        {
                            cachedItem.Begin = actDate;
                        }
                    }
                }
            }

            File.WriteAllText(DataCachedFile, JsonSerializer.Serialize(cached));

            return new CalendarEventCollection(cached.Where(w => w.Begin >= since)){ Name = "内地电影上映日期" };
        }
    }
}
