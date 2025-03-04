namespace FeatureBasedFolderStructure.Application.Common.Exceptions;

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base("Bu işlemi gerçekleştirme yetkiniz bulunmuyor.") { }
    
    public ForbiddenAccessException(string message) : base(message) { }
    
    public ForbiddenAccessException(string message, Exception innerException) : base(message, innerException) { }
}