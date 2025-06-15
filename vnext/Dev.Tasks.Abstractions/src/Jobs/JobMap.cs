using Hyprx.Dev.Collections;

namespace Hyprx.Dev.Jobs;

public class JobMap : DependencyMap<CodeJob>
{
    public JobMap()
        : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    public JobMap(IDictionary<string, CodeJob> dictionary)
        : base(dictionary)
    {
    }

    public JobMap(IEqualityComparer<string>? comparer)
        : base(comparer ?? StringComparer.OrdinalIgnoreCase)
    {
    }

    public JobMap(int capacity)
        : base(capacity)
    {
    }

    public static JobMap Global { get; } = new JobMap();
}