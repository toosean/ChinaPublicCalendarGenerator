using Ical.Net;
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
                var revent = new Ical.Net.CalendarComponents.CalendarEvent()
                {
                    Start = new CalDateTime(levent.Begin),
                    End = levent.End.HasValue ? new CalDateTime(levent.End.Value) : null,
                    Summary = levent.Title,
                    IsAllDay = levent.IsWholeDay
                };

                calendar.Events.Add(revent);
            }

            using (var memoryStream = new MemoryStream())
            {
                var serializer = new CalendarSerializer();
                serializer.Serialize(calendar, memoryStream, Encoding.UTF8);

                return Task.FromResult(memoryStream.ToArray());
            }
        }
    }
}
