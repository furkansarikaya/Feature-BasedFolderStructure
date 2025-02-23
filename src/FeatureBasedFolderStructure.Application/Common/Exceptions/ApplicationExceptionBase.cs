namespace FeatureBasedFolderStructure.Application.Common.Exceptions;

public abstract class ApplicationExceptionBase : Exception
{
    protected ApplicationExceptionBase(string message) : base(message)
    {
    }

    protected ApplicationExceptionBase(string message, Exception innerException) : base(message, innerException)
    {
    }
}