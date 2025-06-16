using Hyprx.Rex.Collections;
using Hyprx.Rex.Execution;

namespace Hyprx.Rex.Deployments;

public class CodeDeployment : INeedful
{
    public string Id { get; set; } = string.Empty;

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Uses { get; set; }

    public DeferredDeploymentValue<Inputs> With { get; set; } = new DeferredDeploymentValue<Inputs>(new Inputs());

    public DeferredDeploymentValue<Dictionary<string, string>> Env { get; set; } = new DeferredDeploymentValue<Dictionary<string, string>>(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));

    public DeferredDeploymentValue<string> Cwd { get; set; } = new DeferredDeploymentValue<string>(string.Empty);

    public DeferredDeploymentValue<int> Timeout { get; set; } = new DeferredDeploymentValue<int>(60);

    public DeferredDeploymentValue<bool> If { get; set; } = new DeferredDeploymentValue<bool>(true);

    public DeferredDeploymentValue<bool> Force { get; set; } = new DeferredDeploymentValue<bool>(false);

    public Dictionary<string, IDeploymentEventHandler> EventHandlers { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public DeploymentEventTasks EventTasks { get; set; } = new DeploymentEventTasks();

    public string[] Needs { get; set; } = Array.Empty<string>();
}