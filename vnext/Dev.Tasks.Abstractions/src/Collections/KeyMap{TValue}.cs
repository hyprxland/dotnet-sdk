namespace Hyprx.Dev.Collections;

public class KeyMap<TValue> : Dictionary<string, TValue>
{
    public KeyMap()
        : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    public KeyMap(IDictionary<string, TValue> dictionary)
        : base(dictionary, StringComparer.OrdinalIgnoreCase)
    {
    }

    public KeyMap(IEqualityComparer<string>? comparer)
        : base(comparer ?? StringComparer.OrdinalIgnoreCase)
    {
    }

    public KeyMap(int capacity)
        : base(capacity, StringComparer.OrdinalIgnoreCase)
    {
    }
}

public class OrderedKeyMap<TValue> : OrderedDictionary<string, TValue>
{
    public OrderedKeyMap()
        : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    public OrderedKeyMap(IDictionary<string, TValue> dictionary)
        : base(dictionary, StringComparer.OrdinalIgnoreCase)
    {
    }

    public OrderedKeyMap(IEqualityComparer<string>? comparer)
        : base(comparer ?? StringComparer.OrdinalIgnoreCase)
    {
    }

    public OrderedKeyMap(int capacity)
        : base(capacity, StringComparer.OrdinalIgnoreCase)
    {
    }
}