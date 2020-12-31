using System;
using System.Globalization;
using System.Linq;

namespace FitsLibrary.Validation.Header
{
    public class MandatoryHeaderEntriesValidator : IValidator<DocumentParts.Header>
    {
        public static readonly string[] MandatoryFields = new[] { "SIMPLE", "BITPIX", "NAXIS" };

        public override ValidationResult Validate(DocumentParts.Header objToValidate)
        {
            var hasMandatoryFields = MandatoryFields.All(key => objToValidate.Entries.Any(entry => string.Equals(entry.Key, key, StringComparison.Ordinal)));

            if (!hasMandatoryFields)
            {
                return new ValidationResult(
                    validationSuccessful: false,
                    validationFailureMessage: "The FITS header does not contain required fields.");
            }

            var numberOfAxisObject = objToValidate.Entries
                .SingleOrDefault(entry => string.Equals(entry.Key, "NAXIS", StringComparison.Ordinal))?.Value;
            var numberOfAxis = numberOfAxisObject as int? ?? numberOfAxisObject as long?;

            if (numberOfAxis == null)
            {
                return new ValidationResult(
                    validationSuccessful: false,
                    validationFailureMessage: "The FITS header contains the field 'NAXIS' but it is not of type integer");
            }

            var hasAllRequiredNAXISKeywords = Enumerable.Range(1, Convert.ToInt32(numberOfAxis!.Value))
                .All(axis =>
                        objToValidate.Entries
                            .Any(entry =>
                                string.Equals(
                                    entry.Key,
                                    $"NAXIS{axis.ToString(CultureInfo.InvariantCulture)}",
                                    StringComparison.Ordinal)));

            return hasAllRequiredNAXISKeywords
                ? new ValidationResult(
                        validationSuccessful: true,
                        validationFailureMessage: null)
                : new ValidationResult(
                        validationSuccessful: false,
                        validationFailureMessage: "The FITS header does not contain required fields.");

        }
    }
}
