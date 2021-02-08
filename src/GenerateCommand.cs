using ChinaPublicCalendarGenerator.Fetchers;
using ChinaPublicCalendarGenerator.Fetchers.Abstraction;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChinaPublicCalendarGenerator
{
    [GeneratorNameValidator]
    class GenerateCommand
    {
        [Argument(0, Description = "The name of the execute fetcher.")]
        [Required]
        public string Fetcher { get; set; } = string.Empty;

        [Option(ShortName = "d", Description = "Generated from a certain date, format like yyyyMMdd.")]
        public string SinceDateArg { get; set; } = DateTime.Today.ToString("yyyyMMdd");

        [Option(ShortName ="range",Description = "Generated from a certain date with range.")]
        public uint RangeDays { get; set; } = 30;

        [Option(ShortName = "o", Description = "The output path of the generated content.")]
        [LegalFilePath]
        public string? OutputPath { get; set; } = null;

        [Option(ShortName = "r", Description = "Force to overwrite the file in the output path.")]
        public bool ForceOverwriteOutput { get; set; }

        public IGenerator Generator { get; }
        public IConsole Console { get; }
        public FetcherTypeCollection FetcherTypeCollection { get; }
        public IServiceProvider ServiceProvider { get; }


        public GenerateCommand(IGenerator generator, IConsole console, FetcherTypeCollection fetcherTypeCollection, IServiceProvider serviceProvider)
        {
            Generator = generator ?? throw new ArgumentNullException(nameof(generator));
            Console = console ?? throw new ArgumentNullException(nameof(console));
            FetcherTypeCollection = fetcherTypeCollection ?? throw new ArgumentNullException(nameof(fetcherTypeCollection));
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task<int> OnExecuteAsync(CommandLineApplication _)
        {
            var sinceDate = DateTime.Parse($"{SinceDateArg.Substring(0, 4)}-{SinceDateArg.Substring(4, 2)}-{SinceDateArg.Substring(6, 2)}");
            var fetcher = (IFetcher)ServiceProvider.GetRequiredService(FetcherTypeCollection[Fetcher]);

            var events = await fetcher.FetchAsync(sinceDate, sinceDate.AddDays(RangeDays));
            var buffer = await Generator.GeneratorAsync(events);

            if (OutputPath == null)
            {
                Console.WriteLine(Encoding.UTF8.GetString(buffer));
            }
            else if (!File.Exists(OutputPath) || ForceOverwriteOutput || Prompt.GetYesNo("Output file exists, do you want to overwrite?", false))
            {
                File.WriteAllBytes(OutputPath, buffer);
            }

            return 0;
        }
    }
}
