namespace FitsLibrary.Validation
{
    public class ValidationResult
    {
        public ValidationResult(bool validationSuccessful, string? validationFailureMessage)
        {
            ValidationSuccessful = validationSuccessful;
            ValidationFailureMessage = validationFailureMessage;
        }

        public string? ValidationFailureMessage { get; }

        public bool ValidationSuccessful { get; }
    }
}
