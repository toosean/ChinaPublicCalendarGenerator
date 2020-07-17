using System;
using System.Collections.Generic;
using System.Text;

namespace ChinaPublicCalendarGenerator
{
    class CalendarEventCollection : List<CalendarEvent>
    {

        public string? Name { get; set; }

        public CalendarEventCollection() { }
        public CalendarEventCollection(IEnumerable<CalendarEvent> events) : base(events) { }
    }
}
