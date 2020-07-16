using ChinaPublicCalendarGenerator.Fetchers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ChinaPublicCalendarGenerator
{
    class GeneratorNameValidatorAttribute : ValidationAttribute
    {
        public FetcherTypeCollection? FetcherTypeCollection { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            FetcherTypeCollection = (FetcherTypeCollection)validationContext.GetService(typeof(FetcherTypeCollection));

            if (value is GenerateCommand command && !FetcherTypeCollection.Keys.Contains(command.Fetcher))
            {
                return new ValidationResult("Argument fetcherName invalid.");
            }

            return ValidationResult.Success;
        
        }

    }
}
