using System.Diagnostics;

using Hyprx.Exec;

namespace Hyprx;

public static partial class Shell
{
    private static readonly Lazy<string[]> s_argv = new(() =>
    {
        var args = System.Environment.GetCommandLineArgs();
        if (args.Length == 0)
            return Array.Empty<string>();

        // Remove the first argument which is the executable path
        var argv = new string[args.Length - 1];
        for (int i = 1; i < args.Length; i++)
        {
            argv[i - 1] = args[i];
        }

        return argv;
    });

    private static Lazy<bool> s_interactive = new(() =>
    {
        if (Environment.GetEnvironmentVariable("DEBIAN_FRONTEND") == "noninteractive")
            return false;

        if (Environment.GetEnvironmentVariable("CI") == "true")
            return false;

        if (Console.IsOutputRedirected)
            return false;

        if (Console.IsInputRedirected)
            return false;

        foreach (var item in Environment.GetCommandLineArgs())
        {
            if (item.EqualsFold("--non-interactive") ||
                item.EqualsFold("-NonInteractive") ||
                item.EqualFold("--quiet") ||
                item.EqualsFold("-q") ||
                item.EqualsFold("--silent"))
            {
                return false;
            }
        }

        if (!Environment.UserInteractive)
                return false;

        return true;
    });

    public static bool IsInteractive
    {
        get => s_interactive.Value;
        set => s_interactive = new Lazy<bool>(() => value);
    }

    public static string[] Argv => s_argv.Value;

    public static string ExecPath => System.Environment.CommandLine;

    public static void Exit(int exitCode = 0)
    {
        if (exitCode < 0)
            throw new ArgumentOutOfRangeException(nameof(exitCode), "Exit code must be a non-negative integer.");

        System.Environment.Exit(exitCode);
    }

    public static Command Cmd(CommandArgs args)
        => new Command(args);

    public static Command Cmd(CommandOptions options)
        => new Command(options);

    public static Output Run(CommandArgs args)
        => new Command(args).Run();

    public static Output Run(CommandOptions options)
        => new Command(options).Run();

    public static ValueTask<Output> RunAsync(CommandArgs args, CancellationToken cancellationToken = default)
        => new Command(args).RunAsync(cancellationToken);

    public static ValueTask<Output> RunAsync(CommandOptions options, CancellationToken cancellationToken = default)
        => new Command(options).RunAsync(cancellationToken);

    public static Output Bash(string script)
        => new BashCommand().RunScript(script);

    public static ValueTask<Output> BashAsync(string script, CancellationToken cancellationToken = default)
        => new BashCommand().RunScriptAsync(script, cancellationToken);

    public static string? ReadLine()
        => Console.ReadLine();

    public static int ReadCharAsInt()
        => Console.Read();

    public static char ReadChar()
    {
        var key = Console.Read();
        if (key < 0)
            return char.MinValue;

        return (char)key;
    }

    public static ConsoleKeyInfo ReadKey()
        => Console.ReadKey();

    public static ConsoleKeyInfo ReadKey(bool intercept)
        => Console.ReadKey(intercept);

    public static string ResolvePath(string path)
    {
        if (path.Length == 1)
        {
            if (path[0] == '.')
                return System.Environment.CurrentDirectory;

            if (path[0] == '~')
            {
                var profile = Env.Vars.Home;
                if (profile.IsNullOrWhiteSpace())
                    profile = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);

                return profile;
            }

            return path;
        }

        if (path.Length == 2)
        {
            if (path[1] is '/' or '\\')
            {
                if (path[0] == '.')
                    return System.Environment.CurrentDirectory;

                if (path[0] == '~')
                {
                    var profile = Env.Vars.Home;
                    if (profile.IsNullOrWhiteSpace())
                        profile = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);

                    return profile;
                }
            }

            if (path[0] is '.' && path[1] is '.')
            {
                return Path.GetDirectoryName(System.Environment.CurrentDirectory) ?? string.Empty;
            }
        }

        if (path[1] is '/' or '\\')
        {
            if (path[0] == '.')
                path = Path.Combine(System.Environment.CurrentDirectory, path[2..]);

            if (path[0] == '~')
            {
                var profile = Env.Vars.Home;
                if (profile.IsNullOrWhiteSpace())
                    profile = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);

                path = Path.Combine(profile, path[2..]);
            }
        }

        path = Env.Expand(path);
        path = Path.GetFullPath(path);
        return path;
    }

#if NET8_0_OR_GREATER
    public static string ResolvePath(string path, string baseDir)
    {
        if (path.Length == 1)
        {
            if (path[0] == '.')
                return baseDir;

            if (path[0] == '~')
            {
                var profile = Env.Vars.Home;
                if (profile.IsNullOrWhiteSpace())
                    profile = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);

                return profile;
            }

            return path;
        }

        if (path.Length == 2)
        {
            if (path[1] is '/' or '\\')
            {
                if (path[0] == '.')
                    return baseDir;

                if (path[0] == '~')
                {
                    var profile = Env.Vars.Home;
                    if (profile.IsNullOrWhiteSpace())
                        profile = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);

                    return profile;
                }
            }

            if (path[0] is '.' && path[1] is '.')
            {
                return Path.GetDirectoryName(baseDir) ?? string.Empty;
            }
        }

        if (path[1] is '/' or '\\')
        {
            if (path[0] == '.')
                path = Path.Combine(baseDir, path[2..]);

            if (path[0] == '~')
            {
                var profile = Env.Vars.Home;
                if (profile.IsNullOrWhiteSpace())
                    profile = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);

                path = Path.Combine(profile, path[2..]);
            }
        }

        path = Env.Expand(path);
        path = Path.GetFullPath(path, baseDir);
        return path;
    }
#endif

    public static string? Which(string command, IEnumerable<string>? prependPaths = null, bool useCache = true)
        => PathFinder.Which(command, prependPaths, useCache);

    public static string? FindExecutable(string command)
        => PathFinder.Default.Find(command);

    public static string FindExecutableOrThrow(string command)
       => PathFinder.Default.FindOrThrow(command);

    public static void RegisterExecutablePathHint(string name, Action<PathHint> registerHint)
        => PathFinder.Default.RegisterOrUpdate(name, registerHint);

    public static void SetGlobalWriteCommand(Action<ProcessStartInfo>? writeCommand)
        => Command.SetGlobalWriteCommand(writeCommand);
}