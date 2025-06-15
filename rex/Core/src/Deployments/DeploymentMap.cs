using Hyprx.Rex.Collections;

namespace Hyprx.Rex.Deployments;

public class DeploymentMap : DependencyMap<CodeDeployment>
{
    public DeploymentMap()
        : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    public DeploymentMap(IDictionary<string, CodeDeployment> dictionary)
        : base(dictionary)
    {
    }

    public DeploymentMap(IEqualityComparer<string>? comparer)
        : base(comparer ?? StringComparer.OrdinalIgnoreCase)
    {
    }

    public DeploymentMap(int capacity)
        : base(capacity)
    {
    }

    public static DeploymentMap Default { get; } = new DeploymentMap();
}