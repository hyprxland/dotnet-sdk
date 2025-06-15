using Hyprx.Dev.Collections;
using Hyprx.Dev.Tasks;

namespace Hyprx.Dev.Jobs;

public class CodeJobData
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public StringMap Env { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public Outputs Outputs { get; set; } = new Outputs();

    public Inputs Inputs { get; set; } = new Inputs();

    public bool Force { get; set; } = false;

    public bool If { get; set; } = true;

    public int Timeout { get; set; } = 0;

    public string Cwd { get; set; } = string.Empty;

    public TaskMap Tasks { get; set; } = new TaskMap();
}
