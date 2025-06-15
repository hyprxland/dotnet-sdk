using Hyprx.Dev.Jobs;

namespace Hyprx.Dev.Tasks.Runners;

public class ConsoleRunnerOptions
{
    public string[] Targets { get; set; } = [];

    public bool List { get; set; } = false;

    public bool DryRun { get; set; } = false;

    public bool Verbose { get; set; } = false;

    public bool Debug { get; set; } = false;

    public string Cmd { get; set; } = "auto";

    public int Timeout { get; set; } = 0;

    public List<string> EnvFiles { get; set; } = new();

    public Dictionary<string, string> Env { get; set; } = new();

    public string Context { get; set; } = "default";

    public IServiceProvider? Services { get; set; }

    public TaskMap Tasks { get; set; } = TaskMap.Global;

    public JobMap Jobs { get; set; } = JobMap.Global;
}