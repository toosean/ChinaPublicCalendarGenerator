using ChinaPublicCalendarGenerator.Fetchers;
using ChinaPublicCalendarGenerator.Fetchers.Abstraction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ChinaPublicCalendarGenerator
{
    class GeneratorNameValidatorAttribute : ValidationAttribute
    {

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var fetcherTypeCollection = (FetcherTypeCollection)validationContext.GetService(typeof(FetcherTypeCollection))!;

            if (value is GenerateCommand command)
            {
                if (fetcherTypeCollection.Keys.Contains(command.Fetcher))
                {
                    return new ValidationResult("Argument fetcherName invalid.");
                }

                if(!DateTime.TryParse($"{command.SinceDateArg.Substring(0, 4)}-{command.SinceDateArg.Substring(4, 2)}-{command.SinceDateArg.Substring(6, 2)}", out _))
                {
                    return new ValidationResult("Argument SinceDateArg format is incorrect.");
                }
            }

            return ValidationResult.Success;
        
        }
    }
}
