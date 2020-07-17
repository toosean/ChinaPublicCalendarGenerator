﻿using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ChinaPublicCalendarGenerator
{
    class IcalNETGenerator : IGenerator
    {
        public Task<byte[]> GeneratorAsync(CalendarEventCollection events)
        {
            var calendar = new Calendar();

            foreach(var levent in events)
            {

                CalDateTime calStart = levent.IsWholeDay 
                    ? new CalDateTime(levent.Begin.Year, levent.Begin.Month, levent.Begin.Day) 
                    : new CalDateTime(levent.Begin);

                if (levent.IsWholeDay) calStart.HasTime = false;

                CalDateTime calEnd = !levent.End.HasValue 
                    ? calStart 
                    : levent.IsWholeDay 
                    ? new CalDateTime(levent.End.Value.Year, levent.End.Value.Month, levent.End.Value.Day) 
                    : new CalDateTime(levent.End.Value);

                if (levent.IsWholeDay) calEnd.HasTime = false;

                var revent = new Ical.Net.CalendarComponents.CalendarEvent()
                {
                    Start = calStart,
                    End = calEnd,
                    Summary = levent.Title,
                    IsAllDay = levent.IsWholeDay
                };

                calendar.Events.Add(revent);
            }

            if (events.Name != null) calendar.AddProperty("X-WR-CALNAME", events.Name);
            calendar.Method = "PUBLISH";

            using (var memoryStream = new MemoryStream())
            {
                var serializer = new CalendarSerializer();
                serializer.Serialize(calendar, memoryStream, Encoding.UTF8);

                return Task.FromResult(memoryStream.ToArray());
            }
        }
    }
}
