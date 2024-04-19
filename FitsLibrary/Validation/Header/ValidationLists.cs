
using System.Collections.Generic;

namespace FitsLibrary.Validation.Header;

internal static class ValidationLists
{
    public static List<IValidator<DocumentParts.Header>> ImageExtensionHeaderValidators = [
            new KeywordsMustBeUniqueValidator(),
            new MandatoryHeaderEntriesValidator(["XTENSION", "BITPIX", "NAXIS"]),
            new NoInvalidCharactersInKeywordsValidator(),
    ];

    public static List<IValidator<DocumentParts.Header>> PrimaryBlockHeaderValidators = [
            new KeywordsMustBeUniqueValidator(),
            new MandatoryHeaderEntriesValidator(["SIMPLE", "BITPIX", "NAXIS"]),
            new NoInvalidCharactersInKeywordsValidator(),
    ];
}
