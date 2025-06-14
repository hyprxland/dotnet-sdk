using Hyprx.Dev.Tasks;

namespace Hyprx.Dev.Deployments;

public class DeploymentEventTasks
{
    private readonly Dictionary<string, TaskMap> lookup;

    public DeploymentEventTasks()
    {
        this.lookup = new Dictionary<string, TaskMap>(StringComparer.OrdinalIgnoreCase);
    }

    public DeploymentEventTasks AddTask(string eventName, CodeTask task)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            throw new ArgumentNullException(nameof(eventName));
        }

        if (task == null)
        {
            throw new ArgumentNullException(nameof(task));
        }

        var tasks = this.GetOrCreateTaskMap(eventName);
        tasks.Add(task.Id, task);

        return this;
    }

    public DeploymentEventTasks AddTasks(string eventName, IEnumerable<CodeTask> tasks)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            throw new ArgumentNullException(nameof(eventName));
        }

        if (tasks == null)
        {
            throw new ArgumentNullException(nameof(tasks));
        }

        var map = this.GetOrCreateTaskMap(eventName);
        foreach (var task in tasks)
        {
            map.Add(task.Id, task);
        }

        return this;
    }

    public bool ContainsEvent(string eventName)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            throw new ArgumentNullException(nameof(eventName));
        }

        return this.lookup.ContainsKey(eventName);
    }

    public bool HasTasks(string eventName)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            throw new ArgumentNullException(nameof(eventName));
        }

        return this.lookup.ContainsKey(eventName) && this.lookup[eventName].Count > 0;
    }

    public TaskMap GetOrCreateTaskMap(string eventName)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            throw new ArgumentNullException(nameof(eventName));
        }

        if (!this.lookup.TryGetValue(eventName, out var tasks))
        {
            tasks = new TaskMap();
            this.lookup[eventName] = tasks;
        }

        return tasks;
    }

    public bool TryGetTaskMap(string eventName, out TaskMap? tasks)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            throw new ArgumentNullException(nameof(eventName));
        }

        return this.lookup.TryGetValue(eventName, out tasks);
    }
}