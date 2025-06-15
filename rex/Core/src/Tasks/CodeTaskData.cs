using Hyprx.Rex.Collections;

namespace Hyprx.Rex.Tasks;

public class CodeTaskData
{
    public CodeTaskData(string id)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(id, nameof(id));
        this.Id = id;
        this.Name = id;
    }

    public string Id { get; set; }

    public string Name { get; set; }

    public string? Uses { get; set; }

    public string? Description { get; set; }

    public Inputs Inputs { get; set; } = new Inputs();

    public bool Force { get; set; } = false;

    public bool If { get; set; } = true;

    public StringMap Env { get; set; } = new StringMap();

    public string Cwd { get; set; } = string.Empty;

    public int Timeout { get; set; } = 0;

    public string[] Needs { get; set; } = [];
}