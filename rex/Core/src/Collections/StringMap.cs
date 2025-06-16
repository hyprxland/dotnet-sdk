namespace Hyprx.Rex.Collections;

public class StringMap : KeyMap<string>
{
    public StringMap()
        : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    public StringMap(IDictionary<string, string> dictionary)
        : base(dictionary)
    {
    }

    public StringMap(IEqualityComparer<string>? comparer)
        : base(comparer ?? StringComparer.OrdinalIgnoreCase)
    {
    }

    public StringMap(int capacity)
        : base(capacity)
    {
    }
}