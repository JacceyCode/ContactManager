using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Services.Helpers
{
    public class ValidationHelper
    {
        internal static void ModelValidation(object obj)
        {
            ValidationContext validationContext = new ValidationContext(obj);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);

            if (!isValid)
            {
                var errorMessages = new StringBuilder();
                foreach (var validationResult in validationResults)
                {
                    errorMessages.AppendLine(validationResult.ErrorMessage);
                }

                throw new ArgumentException(errorMessages.ToString());
            }
        }
    }
}
