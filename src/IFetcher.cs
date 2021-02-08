using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChinaPublicCalendarGenerator
{
    interface IFetcher
    {
        Task<CalendarEventCollection> FetchAsync(DateTime begin, DateTime end);
    }
}
