namespace EfCoreExtras.EntityTypes.Keys;

public readonly struct EntityKey : IEquatable<EntityKey>
{
    private readonly int _hashCode;
    private readonly object[] _values;

    public EntityKey(object[] values)
    {
        if (values.Length == 0)
            throw new InvalidOperationException("Key values cannot be empty.");

        var hash = new HashCode();
        for (var i = 0; i < values.Length; i++)
            hash.Add(values[i]);

        _hashCode = hash.ToHashCode();
        _values = values;
    }

    public string Value => string.Join('|', _values);

    public static EntityKey FromValues(params object[] values)
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
        if (_hashCode != other._hashCode)
            return false;

        if (_values.Length != other._values.Length)
            return false;

        for (int i = 0; i < _values.Length; i++)
        {
            if (!Equals(_values[i], other._values[i]))
                return false;
        }

        return true;
    }

    public override int GetHashCode() => _hashCode;

    public override string ToString() => Value;
}
