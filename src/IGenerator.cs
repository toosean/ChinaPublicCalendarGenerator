using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChinaPublicCalendarGenerator
{
    interface IGenerator
    {
        Task<byte[]> GeneratorAsync(CalendarEventCollection events);
    }
}
