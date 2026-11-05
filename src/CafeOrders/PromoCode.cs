namespace CafeOrders;

// R12: Replace Primitive with Value Object
public readonly record struct PromoCode(string Value)
{
    public static readonly PromoCode None = new(string.Empty);

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public static implicit operator PromoCode(string? code) => new(code ?? string.Empty);
    public static implicit operator string(PromoCode promoCode) => promoCode.Value;

    public override string ToString() => Value ?? string.Empty;
}
