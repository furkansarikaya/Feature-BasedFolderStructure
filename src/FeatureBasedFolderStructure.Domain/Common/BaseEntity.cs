namespace FeatureBasedFolderStructure.Domain.Common;

public abstract class BaseEntity<TKey> : IEntity<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; } = default!;
    private List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
