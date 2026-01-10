namespace WinterSnow.Core.Domain.Common;

/// <summary>
/// nopCommerce-style base entity (integer identity key).
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
}

