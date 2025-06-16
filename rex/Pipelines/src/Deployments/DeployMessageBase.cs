using Hyprx.Rex.Messaging;

namespace Hyprx.Rex.Deployments;

public class DeployMessageBase : IMessage
{
    public DeployMessageBase(string kind, CodeDeploymentData data)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(kind, nameof(kind));
        ArgumentNullException.ThrowIfNull(data, nameof(data));
        this.Topic = kind;
        this.Data = data;
    }

    public string Topic { get; }

    public CodeDeploymentData Data { get; }

    public DeploymentAction Action { get; set; } = DeploymentAction.Deploy;

    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}

public class DeployStarted : DeployMessageBase
{
    public DeployStarted(CodeDeploymentData data, DeploymentAction action)
        : base("deploy:started", data)
    {
        this.Action = action;
    }
}

public class DeployFailed : DeployMessageBase
{
    public DeployFailed(CodeDeploymentData data, DeploymentAction action, Exception exception)
        : base("deploy:failed", data)
    {
        ArgumentNullException.ThrowIfNull(exception, nameof(exception));
        this.Action = action;
        this.Exception = exception;
    }

    public Exception Exception { get; }
}

public class DeploySkipped : DeployMessageBase
{
    public DeploySkipped(CodeDeploymentData data, DeploymentAction action)
        : base("deploy:skipped", data)
    {
        this.Action = action;
    }
}

public class DeployCancelled : DeployMessageBase
{
    public DeployCancelled(CodeDeploymentData data, DeploymentAction action)
        : base("deploy:cancelled", data)
    {
        this.Action = action;
    }
}

public class DeployCompleted : DeployMessageBase
{
    public DeployCompleted(CodeDeploymentData data, DeploymentAction action)
        : base("deploy:completed", data)
    {
        this.Action = action;
    }
}

public class DeploymentsFoundCyclicalReferences : IMessage
{
    public DeploymentsFoundCyclicalReferences(List<CodeDeploymentData> deployments)
    {
        ArgumentNullException.ThrowIfNull(deployments, nameof(deployments));
        this.Deployments.AddRange(deployments);
    }

    public string Topic => "deploy:cyclical-references";

    public List<CodeDeploymentData> Deployments { get; } = new();
}

public class DeploymentsFoundMissingDependencies : IMessage
{
    public DeploymentsFoundMissingDependencies(List<(CodeDeploymentData, List<string>)> deployments)
    {
        ArgumentNullException.ThrowIfNull(deployments, nameof(deployments));
        this.Deployments.AddRange(deployments);
    }

    public string Topic => "deploy:missing-dependencies";

    public List<(CodeDeploymentData, List<string>)> Deployments { get; } = new();
}