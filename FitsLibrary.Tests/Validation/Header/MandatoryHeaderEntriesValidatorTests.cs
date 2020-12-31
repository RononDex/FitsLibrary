using System.Collections.Generic;
using FitsLibrary.DocumentParts.Objects;
using FitsLibrary.Validation.Header;
using FluentAssertions;
using NUnit.Framework;

namespace FitsLibrary.Tests.Validation.Header
{
    public class MandatoryHeaderEntriesValidatorTests
    {
        [Test]
        public void Validate_WithNoHeaderEntries_ValidationUnsucessful()
        {
            // Arrange
            var testee = new MandatoryHeaderEntriesValidator();

            // Act
            var result = testee.Validate(new FitsLibrary.DocumentParts.Header());

            // Assert
            result.ValidationSucessful.Should().Be(false);
            result.ValidationFailureMessage.Should().Be("The FITS header does not contain required fields.");
        }

        [Test]
        public void Validate_WithAllMandatoryKeywordsWith0Axis_ValidationSucessful()
        {
            // Arrange
            var testee = new MandatoryHeaderEntriesValidator();

            // Act
            var result = testee.Validate(
                    new FitsLibrary.DocumentParts.Header(
                        new List<HeaderEntry>
                        {
                            new HeaderEntry(
                                key: "SIMPLE",
                                value: true,
                                comment: null),
                            new HeaderEntry(
                                    key: "BITPIX",
                                    value: 16,
                                    comment: null),
                            new HeaderEntry(
                                    key: "NAXIS",
                                    value: 0,
                                    comment: null),
                            new HeaderEntry(
                                    key: "END",
                                    value: 16,
                                    comment: null),
                        }));

            // Assert
            result.ValidationSucessful.Should().Be(true);
            result.ValidationFailureMessage.Should().BeNull();
        }

        [Test]
        public void Validate_WithAllMandatoryKeywordsWith3Axis_ValidationSuccessful()
        {
            // Arrange
            var testee = new MandatoryHeaderEntriesValidator();

            // Act
            var result = testee.Validate(
                    new FitsLibrary.DocumentParts.Header(
                        new List<HeaderEntry>
                        {
                            new HeaderEntry(
                                key: "SIMPLE",
                                value: true,
                                comment: null),
                            new HeaderEntry(
                                    key: "BITPIX",
                                    value: 16,
                                    comment: null),
                            new HeaderEntry(
                                    key: "NAXIS",
                                    value: 3,
                                    comment: null),
                            new HeaderEntry(
                                    key: "NAXIS1",
                                    value: 1000,
                                    comment: null),
                            new HeaderEntry(
                                    key: "NAXIS2",
                                    value: 1000,
                                    comment: null),
                            new HeaderEntry(
                                    key: "NAXIS3",
                                    value: 1000,
                                    comment: null),
                            new HeaderEntry(
                                    key: "END",
                                    value: 16,
                                    comment: null),
                        }));

            // Assert
            result.ValidationSucessful.Should().Be(true);
            result.ValidationFailureMessage.Should().BeNull();
        }

        [Test]
        public void Validate_WithAllMandatoryKeywordsWith3AxisButNotAllNAXIS_ValidationFails()
        {
            // Arrange
            var testee = new MandatoryHeaderEntriesValidator();

            // Act
            var result = testee.Validate(
                    new FitsLibrary.DocumentParts.Header(
                        new List<HeaderEntry>
                        {
                            new HeaderEntry(
                                key: "SIMPLE",
                                value: true,
                                comment: null),
                            new HeaderEntry(
                                    key: "BITPIX",
                                    value: 16,
                                    comment: null),
                            new HeaderEntry(
                                    key: "NAXIS",
                                    value: 3,
                                    comment: null),
                            new HeaderEntry(
                                    key: "NAXIS1",
                                    value: 1000,
                                    comment: null),
                            new HeaderEntry(
                                    key: "NAXIS2",
                                    value: 1000,
                                    comment: null),
                            new HeaderEntry(
                                    key: "END",
                                    value: 16,
                                    comment: null),
                        }));

            // Assert
            result.ValidationSucessful.Should().Be(false);
            result.ValidationFailureMessage.Should().Be("The FITS header does not contain required fields.");
        }

        [Test]
        public void Validate_WithAllMandatoryKeywordsButNAXISIsNotTypeInt_ValidationFails()
        {
            // Arrange
            var testee = new MandatoryHeaderEntriesValidator();

            // Act
            var result = testee.Validate(
                    new FitsLibrary.DocumentParts.Header(
                        new List<HeaderEntry>
                        {
                            new HeaderEntry(
                                key: "SIMPLE",
                                value: true,
                                comment: null),
                            new HeaderEntry(
                                    key: "BITPIX",
                                    value: 16,
                                    comment: null),
                            new HeaderEntry(
                                    key: "NAXIS",
                                    value: "test",
                                    comment: null),
                            new HeaderEntry(
                                    key: "END",
                                    value: 16,
                                    comment: null),
                        }));

            // Assert
            result.ValidationSucessful.Should().Be(false);
            result.ValidationFailureMessage.Should().Be("The FITS header contains the field 'NAXIS' but it is not of type integer");
        }
    }
}

