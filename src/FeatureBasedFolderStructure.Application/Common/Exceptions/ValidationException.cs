namespace FeatureBasedFolderStructure.Application.Common.Exceptions;

public class ValidationException : ApplicationExceptionBase
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
        : base("Bir veya birden fazla doğrulama hatası oluştu.")
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }
}