namespace FeatureBasedFolderStructure.Domain.Common;

public interface IEntity<TKey> where TKey : IEquatable<TKey>
{
    TKey Id { get; set; }
}