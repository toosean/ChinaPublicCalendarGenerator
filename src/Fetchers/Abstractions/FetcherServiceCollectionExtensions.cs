using ChinaPublicCalendarGenerator;
using ChinaPublicCalendarGenerator.Fetchers;
using ChinaPublicCalendarGenerator.Fetchers.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FetcherServiceCollectionExtensions
    {
        public static IServiceCollection AddFetchers(this IServiceCollection services)
            => FetcherServiceCollectionExtensions.AddFetchers(services, typeof(FetcherServiceCollectionExtensions).Assembly);

        public static IServiceCollection AddFetchers(this IServiceCollection services, params Assembly[] assemblies)
        {
            var typeOfIFetcher = typeof(IFetcher);
            var typeOfFetchShortNameAttribute = typeof(FetchShortNameAttribute);

            var fetchTypeCollection = new FetcherTypeCollection(
                    assemblies.Select(s => s.DefinedTypes)
                        .SelectMany(s => s)
                        .Where(w => w.CustomAttributes.Any(a => a.AttributeType == typeOfFetchShortNameAttribute))
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
