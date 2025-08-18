using System.Text.Json;

namespace EfCoreExtras.Keys;

public readonly struct EntityKey : IEquatable<EntityKey>
{
    private readonly int _hashCode;

    public EntityKey(object?[] values)
    {
        if (values.Length == 0)
            throw new InvalidOperationException("Key values cannot be empty.");

        var hash = new HashCode();
        foreach (var keyValue in values)
        {
            if (keyValue == null)
                throw new NullReferenceException($"No key value passed into an '{nameof(EntityKey)}' is allowed to be null.");

            hash.Add(keyValue);
        }

        _hashCode = hash.ToHashCode();

        var value = JsonSerializer.Serialize(values);
        if (string.IsNullOrWhiteSpace(value))
            throw new NullReferenceException("The value of an entity key cannot be null or an empty string.");

        Value = value;
    }

    public string Value { get; }

    public static EntityKey FromValues(params object?[] values)
    {
        return new EntityKey(values);
    }

    public static bool operator ==(EntityKey left, EntityKey right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(EntityKey left, EntityKey right)
    {
        return !(left == right);
    }

    public override bool Equals(object? obj)
    {
        return obj is EntityKey other && Equals(other);
    }

    public bool Equals(EntityKey other)
    {
        return _hashCode == other._hashCode && Value == other.Value;
    }

    public override int GetHashCode() => _hashCode;

    public override string ToString() => Value;
}
