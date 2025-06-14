using Hyprx.Dev.Collections;
using Hyprx.Dev.Execution;

namespace Hyprx.Dev.Tasks;

public class CodeTask : INeedful
{
    public CodeTask(string id)
    {
        this.Id = id;
        this.Name = id;
        this.Description = $"Task {id}";
    }

    public string Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public string? Uses { get; set; }

    public string[] Needs { get; set; } = [];

    public DeferredTaskValue<int> Timeout { get; set; } = 0;

    public DeferredTaskValue<StringMap> Env { get; set; } = new StringMap();

    public DeferredTaskValue<Inputs> With { get; set; } = new Inputs();

    public DeferredTaskValue<string> Cwd { get; set; } = string.Empty;

    public DeferredTaskValue<bool> Force { get; set; } = false;

    public DeferredTaskValue<bool> If { get; set; } = true;
}