using FS.EntityFramework.Library.Common;

namespace FeatureBasedFolderStructure.Domain.ValueObjects.Catalogs;

public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    public Money()
    {
        
    }
    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public static Money operator +(Money first, Money second)
    {
        if (first.Currency != second.Currency)
            throw new ArgumentException("Cannot add money with different currencies");

        return new Money(first.Amount + second.Amount, first.Currency);
    }

    public static Money operator -(Money first, Money second)
    {
        if (first.Currency != second.Currency)
            throw new ArgumentException("Cannot subtract money with different currencies");

        return new Money(first.Amount - second.Amount, first.Currency);
    }
}