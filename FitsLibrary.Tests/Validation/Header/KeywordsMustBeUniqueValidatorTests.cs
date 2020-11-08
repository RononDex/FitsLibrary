using System.Collections.Generic;
using FitsLibrary.DocumentParts.Objects;
using FitsLibrary.Validation.Header;
using FluentAssertions;
using NUnit.Framework;

namespace FitsLibrary.Tests.Validation.Header
{
    public class KeywordsMustBeUniqueValidatorTests
    {
        [Test]
        public void Validate_WithOneHeaderEntry_ValidationSccessful()
        {
            // Arrange
            var testee = new KeywordsMustBeUniqueValidator();
            var header = new FitsLibrary.DocumentParts.Header(new List<HeaderEntry>
            {
                new HeaderEntry("KEY1", "SomeValue", "SomeComment"),
            });

            // Act
            var validationResult = testee.Validate(header);

            // Assert
            validationResult.ValidationSucessful.Should().Be(true);
            validationResult.ValidationFailureMessage.Should().BeNull();
        }

        [Test]
        public void Validate_WithTwoHeaderEntriesBeingUnique_ValidationSucessful()
        {
            // Arrange
            var testee = new KeywordsMustBeUniqueValidator();
            var header = new FitsLibrary.DocumentParts.Header(new List<HeaderEntry>{
                new HeaderEntry("KEY1", "SomeValue", "SomeComment"),
                new HeaderEntry("KEY2", "SomeValue", "SomeComment"),
            });

            // Act
            var validationResult = testee.Validate(header);

            // Assert
            validationResult.ValidationSucessful.Should().Be(true);
            validationResult.ValidationFailureMessage.Should().BeNull();
        }

        [Test]
        public void Validate_WithTrheeHeaderEntriesWhereTwoAreNotUnique_ValidationFails()
        {
            // Arrange
            var testee = new KeywordsMustBeUniqueValidator();
            var header = new FitsLibrary.DocumentParts.Header(new List<HeaderEntry>{
                new HeaderEntry("KEY1", "SomeValue", "SomeComment"),
                new HeaderEntry("KEY1", "SomeValue", "SomeComment"),
                new HeaderEntry("KEY2", "SomeValue", "SomeComment"),
            });

            // Act
            var validationResult = testee.Validate(header);

            // Assert
            validationResult.ValidationSucessful.Should().Be(false);
            validationResult.ValidationFailureMessage.Should().Be(
                "Non unique KEYWORDS found. The header entries KEY1 are contained more than once within the header.");
        }

        [Test]
        [TestCase("TEST1", false)]
        [TestCase("SomeRandomKeyWord", false)]
        [TestCase("", true)]
        [TestCase("COMMENT", true)]
        public void Validate_WithDupliaceHeaderEntry_ValidationAsExpected(string duplicateKeyEntry, bool expectedValidationSucessful)
        {
            // Arrange
            var testee = new KeywordsMustBeUniqueValidator();
            var header = new FitsLibrary.DocumentParts.Header(new List<HeaderEntry>{
                new HeaderEntry(duplicateKeyEntry, "SomeValue", "SomeComment"),
                new HeaderEntry(duplicateKeyEntry, "SomeValue2", "SomeComment2"),
                new HeaderEntry("SomeOtherValue", "SomeValue", "SomeComment"),
            });

            // Act
            var validationResult = testee.Validate(header);

            // Assert
            validationResult.ValidationSucessful.Should().Be(expectedValidationSucessful);
            if (!expectedValidationSucessful)
            {
                validationResult.ValidationFailureMessage.Should().Be(
                    $"Non unique KEYWORDS found. The header entries {duplicateKeyEntry} are contained more than once within the header.");
            }
        }
    }
}
