using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FitsLibrary.Validation.Header;

public class MandatoryHeaderEntriesValidator(IList<string> mandatoryFields) : IValidator<DocumentParts.Header>
{
    private IList<string> MandatoryFields { get; } = mandatoryFields;

    public override Task<ValidationResult> ValidateAsync(DocumentParts.Header objToValidate)
    {
        return Task.Run(() =>
        {
            var hasMandatoryFields = ValidateMandatoryFields(objToValidate);
            if ((bool)objToValidate["SIMPLE"] != true)
            {
                return new ValidationResult(
                        validationSuccessful: false,
                        validationFailureMessage: "SIMPLE has to be set to true to mark this fits file to conform with the standard");
            }

            if (this.MandatoryFields.Contains("NAXIS"))
            {
                var numberOfAxis = GetNumberOfAxis(objToValidate);

                if (numberOfAxis == null)
                {
                    return new ValidationResult(
                        validationSuccessful: false,
                        validationFailureMessage: "The FITS header contains the field 'NAXIS' but it is not of type integer");
                }
                var hasAllRequiredNAXISKeywords = ValidateAllAxisDefinitionsPresent(objToValidate, numberOfAxis);
                hasMandatoryFields = hasMandatoryFields && hasAllRequiredNAXISKeywords;
            }

            return hasMandatoryFields
                ? new ValidationResult(
                        validationSuccessful: true,
                        validationFailureMessage: null)
                : new ValidationResult(
                        validationSuccessful: false,
                        validationFailureMessage: "The FITS header is missing required fields (or they are in the wrong location).");
        });
    }

    private bool ValidateAllAxisDefinitionsPresent(DocumentParts.Header objToValidate, long? numberOfAxis)
    {
        var hasAllRequiredNAXISKeywords = objToValidate.Entries.Count >= this.MandatoryFields.Count + numberOfAxis;
        for (var i = 0; i < numberOfAxis; i++)
        {
            if (objToValidate.Entries[i + this.MandatoryFields.Count].Key != $"NAXIS{(i + 1).ToString(CultureInfo.InvariantCulture)}")
            {
                hasAllRequiredNAXISKeywords = false;
            }
        }

        return hasAllRequiredNAXISKeywords;
    }

    private static long? GetNumberOfAxis(DocumentParts.Header objToValidate)
    {
        var numberOfAxisObject = objToValidate.Entries
            .SingleOrDefault(entry => string.Equals(entry.Key, "NAXIS", StringComparison.Ordinal))?.Value;
        var numberOfAxis = numberOfAxisObject as int? ?? numberOfAxisObject as long?;
        return numberOfAxis;
    }

    private bool ValidateMandatoryFields(DocumentParts.Header objToValidate)
    {
        var hasMandatoryFields = objToValidate.Entries.Count >= this.MandatoryFields.Count;
        if (hasMandatoryFields)
        {
            for (var i = 0; i < this.MandatoryFields.Count; i++)
            {
                if (objToValidate.Entries[i].Key != this.MandatoryFields[i])
                {
                    hasMandatoryFields = false;
                }
            }
        }

        return hasMandatoryFields;
    }
}
