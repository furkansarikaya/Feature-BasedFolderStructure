namespace FeatureBasedFolderStructure.Application.Common.Exceptions;

public class NotFoundException(string name, object key) : ApplicationExceptionBase($"{name} ({key}) bulunamadı.");