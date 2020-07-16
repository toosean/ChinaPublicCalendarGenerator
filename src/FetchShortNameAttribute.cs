using System;
using System.Collections.Generic;
using System.Text;

namespace ChinaPublicCalendarGenerator
{
    class FetchShortNameAttribute : Attribute
    {
        public string ShortName { get; }

        public FetchShortNameAttribute(string shortName)
        {
            ShortName = shortName ?? throw new ArgumentNullException(nameof(shortName));
        }
    }
}
