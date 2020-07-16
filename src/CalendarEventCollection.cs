using System;
using System.Collections.Generic;
using System.Text;

namespace ChinaPublicCalendarGenerator
{
    class CalendarEventCollection : List<CalendarEvent>
    {
        public CalendarEventCollection() { }
        public CalendarEventCollection(IEnumerable<CalendarEvent> events) : base(events) { }
    }
}
