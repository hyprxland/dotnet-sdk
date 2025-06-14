using Hyprx.Dev.Collections;

namespace Hyprx.Dev.Tasks;

public class TaskHandlerMap : KeyMap<ITaskHandler>
{
    public TaskHandlerMap()
    {
    }

    public TaskHandlerMap(int capacity)
        : base(capacity)
    {
    }

    public TaskHandlerMap(IEqualityComparer<string> comparer)
        : base(comparer)
    {
    }

    public TaskHandlerMap(IDictionary<string, ITaskHandler> dictionary)
        : base(dictionary)
    {
    }

    public static TaskHandlerMap Global { get; } = new TaskHandlerMap();
}