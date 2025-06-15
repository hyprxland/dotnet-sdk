namespace Hyprx.Rex.Collections;

public class Outputs : KeyMap<object?>
{
    public Outputs()
        : base()
    {
    }

    public Outputs(IDictionary<string, object?> dictionary)
        : base(dictionary)
    {
    }

    public Outputs(IEqualityComparer<string>? comparer)
        : base(comparer ?? StringComparer.OrdinalIgnoreCase)
    {
    }

    public Outputs(int capacity)
        : base(capacity)
    {
    }
}