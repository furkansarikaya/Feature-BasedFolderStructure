namespace FeatureBasedFolderStructure.Application.Common.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RequiresClaimAttribute : Attribute
{
    public string ClaimType { get; }
    public string[] ClaimValues { get; }
    public bool RequireAllValues { get; }

    /// <summary>
    /// Belirli bir claim tipine ve değerlerine sahip olma zorunluluğu ekler.
    /// </summary>
    /// <param name="claimType">Gerekli claim tipi</param>
    /// <param name="requireAllValues">True ise tüm değerlerin mevcut olması gerekir, False ise en az bir değer yeterlidir</param>
    /// <param name="claimValues">İzin verilen claim değerleri</param>
    public RequiresClaimAttribute(string claimType, bool requireAllValues = false, params string[] claimValues)
    {
        ClaimType = claimType;
        ClaimValues = claimValues;
        RequireAllValues = requireAllValues;
    }
}