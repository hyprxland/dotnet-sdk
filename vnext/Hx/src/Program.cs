// See https://aka.ms/new-console-template for more information

using Hyprx.Dev.Tasks.Runners;

using static Hyprx.Dev.Tasks.Runners.ConsoleRunner;
using static Hyprx.Shell;

Task("hello", () =>
{
    Console.WriteLine("Hello, World!");
});

Task("test", () =>
{
    Run("echo Hello from test");
});

var options = new ConsoleRunnerOptions()
{
    Targets = args.Length == 0 ? new[] { "hello" } : args,
};

var exitCode = await RunAsync(options);
Exit(exitCode);