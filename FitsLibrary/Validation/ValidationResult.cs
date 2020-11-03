namespace FitsLibrary.Validation
{
    public class ValidationResult
    {
        public ValidationResult(bool validationSucessful, string? validationFailureMessage)
        {
            ValidationSucessful = validationSucessful;
            ValidationFailureMessage = validationFailureMessage;
        }

        public string? ValidationFailureMessage { get; private set; }

        public bool ValidationSucessful { get; private set; }
    }
}
