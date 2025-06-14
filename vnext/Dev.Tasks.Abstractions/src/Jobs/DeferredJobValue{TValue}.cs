using Hyprx.Dev.Execution;

namespace Hyprx.Dev.Jobs;

public class DeferredJobValue<TValue> : Deferred<DeferredJobValue<TValue>, JobContext, TValue>
{
    public DeferredJobValue()
        : base()
    {
    }

    public DeferredJobValue(TValue value)
        : base(value)
    {
    }

    public DeferredJobValue(Func<TValue> valueFactory)
        : base(valueFactory)
    {
    }

    public DeferredJobValue(Func<JobContext, TValue> valueFactory)
        : base(valueFactory)
    {
    }

    public DeferredJobValue(Func<JobContext, CancellationToken, Task<TValue>> taskFunc)
        : base(taskFunc)
    {
    }

    public static implicit operator DeferredJobValue<TValue>(TValue value)
    {
        return new DeferredJobValue<TValue>(value);
    }

    public static implicit operator DeferredJobValue<TValue>(Func<TValue> valueFactory)
    {
        return new DeferredJobValue<TValue>(valueFactory);
    }

    public static implicit operator DeferredJobValue<TValue>(Func<JobContext, TValue> valueFactory)
    {
        return new DeferredJobValue<TValue>(valueFactory);
    }

    public static implicit operator DeferredJobValue<TValue>(Func<JobContext, CancellationToken, Task<TValue>> taskFunc)
    {
        return new DeferredJobValue<TValue>(taskFunc);
    }
}