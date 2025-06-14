using Hyprx.Dev.Execution;

namespace Hyprx.Dev.Tasks;

public class DeferredTaskValue<TValue> : Deferred<DeferredTaskValue<TValue>, TaskContext, TValue>
{
    public DeferredTaskValue()
        : base()
    {
    }

    public DeferredTaskValue(TValue value)
        : base(value)
    {
    }

    public DeferredTaskValue(Func<TValue> valueFactory)
        : base(valueFactory)
    {
    }

    public DeferredTaskValue(Func<TaskContext, TValue> valueFactory)
        : base(valueFactory)
    {
    }

    public DeferredTaskValue(Func<TaskContext, CancellationToken, Task<TValue>> taskFunc)
        : base(taskFunc)
    {
    }

    public static implicit operator DeferredTaskValue<TValue>(TValue value)
    {
        return new DeferredTaskValue<TValue>(value);
    }

    public static implicit operator DeferredTaskValue<TValue>(Func<TValue> valueFactory)
    {
        return new DeferredTaskValue<TValue>(valueFactory);
    }

    public static implicit operator DeferredTaskValue<TValue>(Func<TaskContext, TValue> valueFactory)
    {
        return new DeferredTaskValue<TValue>(valueFactory);
    }

    public static implicit operator DeferredTaskValue<TValue>(Func<TaskContext, CancellationToken, Task<TValue>> taskFunc)
    {
        return new DeferredTaskValue<TValue>(taskFunc);
    }
}