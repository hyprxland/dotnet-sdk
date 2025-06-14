namespace Hyprx.Dev.Collections;

public class Inputs : KeyMap<object?>
{
    public Inputs()
        : base()
    {
    }

    public Inputs(IDictionary<string, object?> dictionary)
        : base(dictionary)
    {
    }

    public Inputs(IEqualityComparer<string>? comparer)
        : base(comparer ?? StringComparer.OrdinalIgnoreCase)
    {
    }

    public Inputs(int capacity)
        : base(capacity)
    {
    }
}