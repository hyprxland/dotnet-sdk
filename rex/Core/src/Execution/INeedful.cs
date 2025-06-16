namespace Hyprx.Rex.Execution;

public interface INeedful
{
    string Id { get; }

    string[] Needs { get; }
}