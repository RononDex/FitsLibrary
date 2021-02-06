using System.Threading.Tasks;
using FitsLibrary.Validation.Header;
using FluentAssertions;
using NUnit.Framework;

namespace FitsLibrary.Tests.Validation.Header
{
    public class KeywordsMustBeUniqueValidatorTests
    {
        [Test]
        public async Task Validate_WithOneHeaderEntry_ValidationSccessfulAsync()
        {
            // Arrange
            var testee = new KeywordsMustBeUniqueValidator();
            var header = new HeaderBuilder()
                .WithHeaderEntry("KEY1", "SomeValue", "SomeComment")
                .Build();

            // Act
            var validationResult = await testee.ValidateAsync(header);

            // Assert
            validationResult.ValidationSuccessful.Should().Be(true);
            validationResult.ValidationFailureMessage.Should().BeNull();
        }

        [Test]
        public async Task Validate_WithTwoHeaderEntriesBeingUnique_ValidationSucessfulAsync()
        {
            // Arrange
            var testee = new KeywordsMustBeUniqueValidator();
            var header = new HeaderBuilder()
                .WithHeaderEntry("KEY1", "SomeValue", "SomeComment")
                .WithHeaderEntry("KEY2", "SomeValue", "SomeComment")
                .Build();

            // Act
            var validationResult = await testee.ValidateAsync(header);

            // Assert
            validationResult.ValidationSuccessful.Should().Be(true);
            validationResult.ValidationFailureMessage.Should().BeNull();
        }

        [Test]
        public async Task Validate_WithTrheeHeaderEntriesWhereTwoAreNotUnique_ValidationFailsAsync()
        {
            // Arrange
            var testee = new KeywordsMustBeUniqueValidator();
            var header = new HeaderBuilder()
                .WithHeaderEntry("KEY1", "SomeValue", "SomeComment")
                .WithHeaderEntry("KEY1", "SomeValue", "SomeComment")
                .WithHeaderEntry("KEY2", "SomeValue", "SomeComment")
                .Build();

            // Act
            var validationResult = await testee.ValidateAsync(header);

            // Assert
            validationResult.ValidationSuccessful.Should().Be(false);
            validationResult.ValidationFailureMessage.Should().Be(
                "Non unique KEYWORDS found. The header entries KEY1 are contained more than once within the header.");
        }

        [Test]
        [TestCase("TEST1", false)]
        [TestCase("SomeRandomKeyWord", false)]
        [TestCase("", true)]
        [TestCase("COMMENT", true)]
        [TestCase("HISTORY", true)]
        public async Task Validate_WithDupliaceHeaderEntry_ValidationAsExpectedAsync(string duplicateKeyEntry, bool expectedValidationSucessful)
        {
            // Arrange
            var testee = new KeywordsMustBeUniqueValidator();
            var header = new HeaderBuilder()
                .WithHeaderEntry(duplicateKeyEntry, "SomeValue", "SomeComment")
                .WithHeaderEntry(duplicateKeyEntry, "SomeValue2", "SomeComment")
                .WithHeaderEntry("KEY2", "SomeValue", "SomeComment")
                .Build();

            // Act
            var validationResult = await testee.ValidateAsync(header);

            // Assert
            validationResult.ValidationSuccessful.Should().Be(expectedValidationSucessful);
            if (!expectedValidationSucessful)
            {
                validationResult.ValidationFailureMessage.Should().Be(
                    $"Non unique KEYWORDS found. The header entries {duplicateKeyEntry} are contained more than once within the header.");
            }
        }
    }
}
