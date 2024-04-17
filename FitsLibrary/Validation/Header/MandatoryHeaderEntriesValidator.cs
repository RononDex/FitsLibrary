using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FitsLibrary.Validation.Header
{
    public class MandatoryHeaderEntriesValidator : IValidator<DocumentParts.Header>
    {
        public static readonly string[] MandatoryFields = new[] { "SIMPLE", "BITPIX", "NAXIS" };

        public override Task<ValidationResult> ValidateAsync(DocumentParts.Header objToValidate)
        {
            return Task.Run(() =>
            {
                var hasMandatoryFields = objToValidate.Entries.Count >= MandatoryFields.Length;
                if (hasMandatoryFields)
                {
                    for (int i = 0; i < MandatoryFields.Length; i++)
                    {
                        if (objToValidate.Entries[i].Key != MandatoryFields[i])
                        {
                            hasMandatoryFields = false;
                        }
                    }
                }

                if (!hasMandatoryFields)
                {
                    return new ValidationResult(
                        validationSuccessful: false,
                        validationFailureMessage: "The FITS header is missing required fields (or they are in the wrong location).");
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
                var hasAllRequiredNAXISKeywords = objToValidate.Entries.Count >= MandatoryFields.Length + numberOfAxis;
                for (var i = 0; i < numberOfAxis; i++)
                {
                    if (objToValidate.Entries[i + MandatoryFields.Length].Key != $"NAXIS{(i + 1).ToString(CultureInfo.InvariantCulture)}")
                    {
                        hasAllRequiredNAXISKeywords = false;
                    }
                }

                return hasAllRequiredNAXISKeywords
                    ? new ValidationResult(
                            validationSuccessful: true,
                            validationFailureMessage: null)
                    : new ValidationResult(
                            validationSuccessful: false,
                            validationFailureMessage: "The FITS header does not contain required fields.");
            });
        }
    }
}
