using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;
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
                    : new CalDateTime(levent.Begin.ToUniversalTime());

                CalDateTime calEnd = !levent.End.HasValue
                    ? calStart
                    : levent.IsWholeDay
                    ? new CalDateTime(levent.End.Value.Year, levent.End.Value.Month, levent.End.Value.Day)
                    : new CalDateTime(levent.End.Value.ToUniversalTime());

                if (levent.IsWholeDay)
                {
                    calStart.HasDate = true;
                    calStart.HasTime = false;

                    calEnd.HasDate = true;
                    calEnd.HasTime = false;

                    //https://github.com/toosean/ChinaPublicCalendar/issues/9
                    //非常感谢 JarmoHu 指正
                    calEnd = (CalDateTime)calEnd.AddDays(1);
                }

                using (var sha1 = SHA1.Create())
                {
                    byte[] titleBytes = UTF8Encoding.UTF8.GetBytes(levent.Title);
                    byte[] hashMessage = sha1.ComputeHash(titleBytes);
                    string uid = BitConverter.ToString(hashMessage).Replace("-", "").ToLower();

                    var revent = new Ical.Net.CalendarComponents.CalendarEvent()
                    {
                        Uid = uid,
                        DtStamp = calStart,
                        Start = calStart,
                        End = calEnd,
                        Summary = levent.Title,
                        Description = levent.Description,
                        IsAllDay = levent.IsWholeDay
                    };

                    calendar.Events.Add(revent);
                }
            }

            //for Google Calendar
            if (events.Name != null) calendar.AddProperty("X-WR-CALNAME", events.Name);
            calendar.Method = "PUBLISH";

            using (var memoryStream = new MemoryStream())
            {
                var serializer = new CalendarSerializer();
                serializer.Serialize(calendar, memoryStream, new UTF8Encoding(false));

                return Task.FromResult(memoryStream.ToArray());
            }
        }
    }
}
