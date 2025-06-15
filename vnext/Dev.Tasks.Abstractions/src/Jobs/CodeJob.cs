using Hyprx.Dev.Collections;
using Hyprx.Dev.Execution;
using Hyprx.Dev.Tasks;

namespace Hyprx.Dev.Jobs;

public class CodeJob : INeedful
{
    public CodeJob(string id)
    {
        this.Id = id ?? throw new ArgumentNullException(nameof(id));
        this.Name = id;
    }

    public string Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public DeferredJobValue<StringMap> Env { get; set; } = new StringMap();

    public DeferredJobValue<int> Timeout { get; set; } = 0;

    public DeferredJobValue<bool> If { get; set; } = false;

    public DeferredJobValue<Inputs> With { get; set; } = new Inputs();

    public DeferredJobValue<bool> Force { get; set; } = false;

    public TaskMap Tasks { get; set; } = new();

    public DeferredJobValue<string> Cwd { get; set; } = string.Empty;

    public string[] Needs { get; set; } = [];
}