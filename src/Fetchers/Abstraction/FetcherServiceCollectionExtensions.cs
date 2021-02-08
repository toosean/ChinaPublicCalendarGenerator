using ChinaPublicCalendarGenerator;
using ChinaPublicCalendarGenerator.Fetchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FetcherServiceCollectionExtensions
    {
        public static IServiceCollection AddFetchers(this IServiceCollection services)
        {
            var typeOfIFetcher = typeof(IFetcher);

            var fetchTypeCollection = new FetcherTypeCollection(typeof(FetcherServiceCollectionExtensions).Assembly.DefinedTypes
                .Where(w => w.ImplementedInterfaces.Contains(typeOfIFetcher))
                .Where(w => !w.IsInterface)
                .Where(w => !w.IsAbstract)
                .ToList());

            services.AddSingleton(fetchTypeCollection);

            foreach (var type in fetchTypeCollection) services.AddScoped(type.Value);

            return services;

        }
    }
}
