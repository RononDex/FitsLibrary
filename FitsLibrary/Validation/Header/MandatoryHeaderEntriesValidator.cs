using System.Globalization;
using System.Linq;

namespace FitsLibrary.Validation.Header
{
    public class MandatoryHeaderEntriesValidator : BaseValidator<DocumentParts.Header>
    {
        public static readonly string[] MandatoryFields = new[] { "SIMPLE", "BITPIX", "NAXIS", "END" };

        public override ValidationResult Validate(DocumentParts.Header objToValidate)
        {
            var hasMandatoryFields = MandatoryFields.All(key => objToValidate.Entries.Any(entry => string.Equals(entry.Key, key, System.StringComparison.Ordinal)));

            if (!hasMandatoryFields)
            {
                return new ValidationResult(
                    validationSuccessful: false,
                    validationFailureMessage: "The FITS header does not contain required fields.");
            }

            var numberOfAxis = objToValidate.Entries
                .Single(entry => string.Equals(entry.Key, "NAXIS", System.StringComparison.Ordinal)).Value as int?;

            if (numberOfAxis == null)
            {
                return new ValidationResult(
                    validationSuccessful: false,
                    validationFailureMessage: "The FITS header contains the field 'NAXIS' but it is not of type integer");
            }

            var hasAllRequiredNAXISKeywords = Enumerable.Range(1, numberOfAxis!.Value)
                .All(axis =>
                        objToValidate.Entries
                            .Any(entry =>
                                string.Equals(
                                    entry.Key,
                                    $"NAXIS{axis.ToString(CultureInfo.InvariantCulture)}",
                                    System.StringComparison.Ordinal)));

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
