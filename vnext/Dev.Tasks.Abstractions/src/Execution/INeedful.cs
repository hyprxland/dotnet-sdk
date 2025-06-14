namespace Hyprx.Dev.Execution;

public interface INeedful
{
    string Id { get; }

    string[] Needs { get; }
}