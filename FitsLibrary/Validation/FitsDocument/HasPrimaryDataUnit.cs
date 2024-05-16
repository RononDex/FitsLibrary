
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Validation.FitsDocument;

internal class HasPrimaryDataUnit : IValidator<FitsLibrary.FitsDocument>
{
    public override async Task<ValidationResult> ValidateAsync(FitsLibrary.FitsDocument objToValidate)
    {
        if (objToValidate.HeaderDataUnits.Count >= 1
                && objToValidate.HeaderDataUnits[0].Type == HeaderDataUnitType.PRIMARY)
        {
            return new ValidationResult(true, null);
        }

        return new ValidationResult(false, "The FitsDocument needs to at least contain a Primary Hdu and it has to be at the first index");
    }
}
