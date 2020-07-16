using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace ChinaPublicCalendarGenerator
{
    class Program
    {
        static int Main(string[] args)
        {
            var arguments = args;
            if (args.FirstOrDefault() == "--input")
            {
                Console.WriteLine("Input string as arguments:");
                arguments = Console.ReadLine().Split(' ');

                if (arguments.Length == 1 && string.IsNullOrWhiteSpace(arguments.FirstOrDefault())) arguments = new string[0];
            }


            var app = new CommandLineApplication<GenerateCommand>();
            app.Conventions.UseDefaultConventions()
                .UseConstructorInjection(ConfigureService());

            return app.Execute(arguments ?? new string[0]);
        }

        static IServiceProvider ConfigureService()
            => new ServiceCollection()
                .AddHttpClient()
                .AddFetchers()
                .AddScoped<IGenerator, IcalNETGenerator>()
                .AddSingleton<IConsole>(PhysicalConsole.Singleton)
                .BuildServiceProvider();

    }
}
