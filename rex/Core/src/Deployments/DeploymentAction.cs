namespace Hyprx.Rex.Deployments;

public readonly struct DeploymentAction : IEquatable<string>
{
    private static readonly HashSet<DeploymentAction> s_set = new();

    public DeploymentAction(string name)
    {
        DeploymentAction? existing = null;
        foreach (var action in s_set)
        {
            if (action.Name is null)
            {
                continue;
            }

            if (action.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                existing = action;
                break;
            }
        }

        if (existing is null)
        {
            s_set.Add(this);
        }

        this.Name = existing?.Name ?? name.ToLowerInvariant();
    }

    public static IReadOnlyCollection<DeploymentAction> Values => s_set;

    public static DeploymentAction Deploy { get; } = new("deploy");

    public static DeploymentAction Rollback { get; } = new("rollback");

    public static DeploymentAction Destroy { get; } = new("destroy");

    public static implicit operator string(DeploymentAction action) => action.Name;

    public static implicit operator DeploymentAction(string name) => new(name);

    public string Name { get; }

    public static DeploymentAction For(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        foreach (var action in s_set)
        {
            if (action.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return action;
            }
        }

        return new DeploymentAction(name);
    }

    public override string ToString() => this.Name;

    public bool Equals(string? other)
    {
        if (other is null)
        {
            return false;
        }

        return this.Name.Equals(other, StringComparison.OrdinalIgnoreCase);
    }
}