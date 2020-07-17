using System;
using System.Collections.Generic;
using System.Text;

namespace ChinaPublicCalendarGenerator
{
    class CalendarEvent
    {
        public string Title { get; set; } = string.Empty;
        public DateTime Begin { get; set; }
        public DateTime? End { get; set; }
        public bool IsWholeDay { get; set; }

        public string? Description { get; set; }
    }
}
