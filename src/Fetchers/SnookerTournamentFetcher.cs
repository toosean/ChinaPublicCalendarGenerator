﻿using HtmlAgilityPack;
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
    [FetchShortName("snooker-tournament")]
    class SnookerTournamentFetcher : IFetcher
    {
        private const string DataCachedFile = "SnookerTournamentCache";
        private const string DouBanUrl = "https://wst.tv/full-calendar/";

        private static readonly Dictionary<string, int> MonthLableMapper
            = new Dictionary<string, int>
            {
                ["Jan"] = 1,
                ["Feb"] = 2,
                ["Mar"] = 3,
                ["Apr"] = 4,
                ["May"] = 5,
                ["Jun"] = 6,
                ["Jul"] = 7,
                ["Aug"] = 8,
                ["Sep"] = 9,
                ["Oct"] = 10,
                ["Nov"] = 11,
                ["Dec"] = 12
            };

        public IHttpClientFactory HttpClientFactory { get; }

        public SnookerTournamentFetcher(IHttpClientFactory httpClientFactory)
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

                var regex = new Regex(@"(\d{1,2})( \w{3} | )- (\d{1,2}) (\w{3})");

                foreach (var tr in doc.QuerySelectorAll(".calendar-table tbody tr.even"))
                {
                    var tdCollection = tr.Descendants("td").ToArray();

                    var title = tdCollection[1].InnerText.Trim();
                    var date = tdCollection[0].InnerText.Trim();

                    var match = regex.Match(date);

                    if (match.Success)
                    {
                        int? startmon = match.Groups[2].Length > 1 ? MonthLableMapper[match.Groups[2].Value.Trim()] : (int?)null;
                        int? stopmon = match.Groups[4].Length > 0 ? MonthLableMapper[match.Groups[4].Value.Trim()] : (int?)null;

                        if (startmon == null && stopmon != null) startmon = stopmon;
                        if (startmon != null && stopmon == null) stopmon = startmon;
                        if (startmon == null || stopmon == null) throw new NotSupportedException();

                        var actBeginDate = new DateTimeOffset(DateTime.Now.Year,
                            startmon.Value,
                            Convert.ToInt32(match.Groups[1].Value)
                            , 0, 0, 0, TimeSpan.Zero);

                        var actEndDate = new DateTimeOffset(DateTime.Now.Year,
                            stopmon.Value,
                            Convert.ToInt32(match.Groups[3].Value)
                            , 0, 0, 0, TimeSpan.Zero);


                        cached.Add(new CalendarEvent
                        {
                            Title = title,
                            Begin = actBeginDate.LocalDateTime.Date,
                            End = actEndDate.LocalDateTime.Date,
                            IsWholeDay = true
                        });
              
                    }
                }
            }

            File.WriteAllText(DataCachedFile, JsonSerializer.Serialize(cached));

            return new CalendarEventCollection(cached.Where(w => w.Begin >= since)) { Name = "斯诺克比赛日历" };
        }
    }
}
