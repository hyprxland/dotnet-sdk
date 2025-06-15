// See https://aka.ms/new-console-template for more information

using Hyprx.Dev.Jobs;
using Hyprx.Dev.Tasks.Runners;

using static Hyprx.Dev.Tasks.Runners.ConsoleRunner;
using static Hyprx.Shell;

#pragma warning disable SA1500

Task("hello", () =>
{
    Console.WriteLine("Hello, World!");
});

Task("test", _ => Run("echo Hello from test"), b => b.WithDescription("A test task"));

Job("job1", (b) =>
{
    b.Task("hello", (c) =>
    {
        Echo("Hello from job1 task1");
    });

    b.Task("task2", (c) =>
    {
        Echo("Hello from job1 task2");
    });
});

var options = new ConsoleRunnerOptions()
{
    Targets = args.Length == 0 ? new[] { "hello" } : args,
};

var exitCode = await RunTasksAsync(args);
Exit(exitCode);