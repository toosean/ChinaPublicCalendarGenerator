using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ChinaPublicCalendarGenerator.Fetchers
{
    class FetcherTypeCollection : Dictionary<string,TypeInfo>
    {
        internal FetcherTypeCollection(IEnumerable<TypeInfo> fetchers)
        {
            foreach (var fetcherType in fetchers) Add(GetFetcherName(fetcherType), fetcherType);
        }

        private string GetFetcherName(TypeInfo typeInfo)
        {
            var attr = typeInfo.GetCustomAttribute<FetchShortNameAttribute>();
            return attr?.ShortName ?? typeInfo.Name;
        }
            
    }
}
