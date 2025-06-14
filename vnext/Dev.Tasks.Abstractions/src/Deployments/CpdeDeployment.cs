using Hyprx.Dev.Collections;
using Hyprx.Dev.Execution;

namespace Hyprx.Dev.Deployments;

public class CodeDeployment : INeedful
{
    public string Id { get; set; } = string.Empty;

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Uses { get; set; }

    public DeferredDeploymentValue<Inputs> Inputs { get; set; } = new DeferredDeploymentValue<Inputs>();

    public DeferredDeploymentValue<Dictionary<string, string>> Env { get; set; } = new DeferredDeploymentValue<Dictionary<string, string>>(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));

    public DeferredDeploymentValue<string> Cwd { get; set; } = new DeferredDeploymentValue<string>(string.Empty);

    public DeferredDeploymentValue<int> Timeout { get; set; } = new DeferredDeploymentValue<int>(60);

    public DeferredDeploymentValue<bool> If { get; set; } = new DeferredDeploymentValue<bool>(true);

    public DeferredDeploymentValue<bool> Force { get; set; } = new DeferredDeploymentValue<bool>(false);

    public string[] Needs { get; set; } = Array.Empty<string>();
}