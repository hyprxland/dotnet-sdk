using Hyprx.Rex.Collections;

namespace Hyprx.Rex.Tasks;

public class TaskMap : DependencyMap<CodeTask>
{
    public TaskMap()
        : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    public TaskMap(IDictionary<string, CodeTask> dictionary)
        : base(dictionary)
    {
    }

    public TaskMap(IEqualityComparer<string>? comparer)
        : base(comparer ?? StringComparer.OrdinalIgnoreCase)
    {
    }

    public TaskMap(int capacity)
        : base(capacity)
    {
    }

    public static TaskMap Global { get; } = new();
}