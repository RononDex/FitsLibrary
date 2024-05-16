
using System.Collections.Generic;
using FitsLibrary.Validation.FitsDocument;
using FitsLibrary.Validation.Header;

namespace FitsLibrary.Validation;

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

    public static List<IValidator<FitsLibrary.FitsDocument>> FitsDocumentValidators = [
        new HasPrimaryDataUnit()
    ];
}
