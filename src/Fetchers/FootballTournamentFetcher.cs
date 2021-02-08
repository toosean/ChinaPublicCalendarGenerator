using ChinaPublicCalendarGenerator.Fetchers.Abstraction;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChinaPublicCalendarGenerator.Fetchers
{
    [FetchShortName("football-tournament")]
    class FootballTournamentFetcher : CacheableFetcherBase
    {
        private const string Url = "http://goal.sports.163.com/schedule/{0:yyyyMMdd}.html";

        public IHttpClientFactory HttpClientFactory { get; }

        public FootballTournamentFetcher(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        protected override string? GetCalendarName() => "足球赛程";

        protected override async Task<IEnumerable<CalendarEvent>> FetchOnCachedAsync(DateTime since)
        {
            var result = new List<CalendarEvent>();

            //暂时先抓取最近两个月的内容
            DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime end = start.AddMonths(2).AddDays(-1);

            DateTime current = start;

            while (end.Subtract(current).TotalDays >= 0)
            {
                using (var client = HttpClientFactory.CreateClient())
                using (var stream = await client.GetStreamAsync(string.Format(Url, current)))
                {
                    var doc = new HtmlDocument();
                    doc.Load(stream);



                    result.AddRange(doc.QuerySelectorAll("div.leftList2")
                        .Select(headerElement => new
                        {
                            rows = headerElement.NextSiblingElement().QuerySelectorAll("table.daTb01 tr:not([class])")
                                    .Skip(1) //跳过表格标题
                                    .Where(w => !w.Attributes.Contains("id"))
                                    .Where(w => !w.Attributes.Contains("style"))
                                    .Select(row => new
                                    {
                                        Tournament = headerElement.QuerySelector("div.c2 a").InnerText,
                                        Team1 = row.QuerySelector(".c1 a").InnerText,
                                        Team2 = row.QuerySelector(".c2 a").InnerText,
                                        BeginTime = DateTime.Parse($"{current:yyyy-MM-dd} {row.QuerySelector("td:nth-child(2)").InnerText.Trim():00}"),
                                    })
                        })
                        .Select(item =>
                            item.rows.Select(row => new CalendarEvent
                            {
                                Title = $"[{row.Tournament}] {row.Team1} VS {row.Team2}",
                                Begin = row.BeginTime,
                                End = row.BeginTime.AddMinutes(90),
                                IsWholeDay = false,
                            }))
                        .SelectMany(s => s));

                }

                current = current.AddDays(1);
            }

            return result;

        }
    }
}
