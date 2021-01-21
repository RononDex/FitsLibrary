using System.Threading.Tasks;
using FitsLibrary.DocumentParts;
using FitsLibrary.Validation.Header;
using FluentAssertions;
using NUnit.Framework;

namespace FitsLibrary.Tests.Validation.Header
{
    public class MandatoryHeaderEntriesValidatorTests
    {
        [Test]
        public async Task Validate_WithNoHeaderEntries_ValidationUnsucessfullAsync()
        {
            // Arrange
            var testee = new MandatoryHeaderEntriesValidator();
            var header = new HeaderBuilder()
                .WithEmptyHeader()
                .Build();

            // Act
            var result = await testee.ValidateAsync(header);

            // Assert
            result.ValidationSucessful.Should().Be(false);
            result.ValidationFailureMessage.Should().Be("The FITS header does not contain required fields.");
        }

        [Test]
        public async Task Validate_WithAllMandatoryKeywordsWith0Axis_ValidationSucessfulAsync()
        {
            // Arrange
            var testee = new MandatoryHeaderEntriesValidator();
            var header = new HeaderBuilder()
                .WithValidFitsFormat()
                .WithContentDataType(DataContentType.SHORT)
                .WithNumberOfAxis(0)
                .WithEndEntry()
                .Build();

            // Act
            var result = await testee.ValidateAsync(header);

            // Assert
            result.ValidationSucessful.Should().Be(true);
            result.ValidationFailureMessage.Should().BeNull();
        }

        [Test]
        public async Task Validate_WithAllMandatoryKeywordsWith3Axis_ValidationSuccessfulAsync()
        {
            // Arrange
            var testee = new MandatoryHeaderEntriesValidator();
            var header = new HeaderBuilder()
                .WithValidFitsFormat()
                .WithContentDataType(DataContentType.SHORT)
                .WithNumberOfAxis(3)
                .WithAxisOfSize(1, 1000)
                .WithAxisOfSize(2, 1000)
                .WithAxisOfSize(3, 1000)
                .WithEndEntry()
                .Build();

            // Act
            var result = await testee.ValidateAsync(header);

            // Assert
            result.ValidationSucessful.Should().Be(true);
            result.ValidationFailureMessage.Should().BeNull();
        }

        [Test]
        public async Task Validate_WithAllMandatoryKeywordsWith3AxisButWronglyDefinedSizes_ValidationFailsAsync()
        {
            // Arrange
            var testee = new MandatoryHeaderEntriesValidator();
            var header = new HeaderBuilder()
                .WithValidFitsFormat()
                .WithContentDataType(DataContentType.SHORT)
                .WithNumberOfAxis(3)
                .WithAxisOfSize(1, 1000)
                .WithAxisOfSize(2, 1000)
                .WithAxisOfSize(4, 1000)
                .WithEndEntry()
                .Build();

            // Act
            var result = await testee.ValidateAsync(header);

            // Assert
            result.ValidationSucessful.Should().Be(false);
            result.ValidationFailureMessage.Should().Be("The FITS header does not contain required fields.");
        }

        [Test]
        public async Task Validate_WithAllMandatoryKeywordsWith3AxisButNotAllNAXIS_ValidationFailsAsync()
        {
            // Arrange
            var testee = new MandatoryHeaderEntriesValidator();
            var header = new HeaderBuilder()
                .WithValidFitsFormat()
                .WithContentDataType(DataContentType.SHORT)
                .WithNumberOfAxis(3)
                .WithAxisOfSize(1, 1000)
                .WithAxisOfSize(2, 1000)
                .WithEndEntry()
                .Build();

            // Act
            var result = await testee.ValidateAsync(header);

            // Assert
            result.ValidationSucessful.Should().Be(false);
            result.ValidationFailureMessage.Should().Be("The FITS header does not contain required fields.");
        }

        [Test]
        public async Task Validate_WithAllMandatoryKeywordsButNAXISIsNotTypeInt_ValidationFailsAsync()
        {
            // Arrange
            var testee = new MandatoryHeaderEntriesValidator();
            var header = new HeaderBuilder()
                .WithValidFitsFormat()
                .WithContentDataType(DataContentType.SHORT)
                .WithNumberOfAxis("test")
                .WithEndEntry()
                .Build();

            // Act
            var result = await testee.ValidateAsync(header);

            // Assert
            result.ValidationSucessful.Should().Be(false);
            result.ValidationFailureMessage.Should().Be("The FITS header contains the field 'NAXIS' but it is not of type integer");
        }
    }
}

