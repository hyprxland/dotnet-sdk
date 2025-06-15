using Hyprx.Rex.Execution;

namespace Hyprx.Rex.Deployments;

public class DeferredDeploymentValue<TValue> : Deferred<DeferredDeploymentValue<TValue>, DeploymentContext, TValue>
{
    public DeferredDeploymentValue()
        : base()
    {
    }

    public DeferredDeploymentValue(TValue value)
        : base(value)
    {
    }

    public DeferredDeploymentValue(Func<TValue> valueFactory)
        : base(valueFactory)
    {
    }

    public DeferredDeploymentValue(Func<DeploymentContext, TValue> valueFactory)
        : base(valueFactory)
    {
    }

    public DeferredDeploymentValue(Func<DeploymentContext, CancellationToken, Task<TValue>> taskFunc)
        : base(taskFunc)
    {
    }

    public static implicit operator DeferredDeploymentValue<TValue>(TValue value)
    {
        return new DeferredDeploymentValue<TValue>(value);
    }

    public static implicit operator DeferredDeploymentValue<TValue>(Func<TValue> valueFactory)
    {
        return new DeferredDeploymentValue<TValue>(valueFactory);
    }

    public static implicit operator DeferredDeploymentValue<TValue>(Func<DeploymentContext, TValue> valueFactory)
    {
        return new DeferredDeploymentValue<TValue>(valueFactory);
    }

    public static implicit operator DeferredDeploymentValue<TValue>(Func<DeploymentContext, CancellationToken, Task<TValue>> taskFunc)
    {
        return new DeferredDeploymentValue<TValue>(taskFunc);
    }
}