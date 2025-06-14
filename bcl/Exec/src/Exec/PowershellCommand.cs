namespace Hyprx.Exec;

public sealed class PowershellCommand : ShellCommand<PowershellCommand, PowershellCommandOptions>
{
    public PowershellCommand()
    {
    }

    public PowershellCommand(string script)
        : this()
    {
        this.Options.Script = script;
    }

    /// <summary>
    /// Registers the PowerShell Core (pwsh) path hint with the default path finder.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   The environment variable <c>PWSH_EXE</c> can be used to specify the path to the PowerShell Core executable
    ///   and it takes precedence over the default paths.
    /// </para>
    /// <para>
    ///   The default paths for windows look under Program Files for PowerShell versions 6 and 7, including
    ///   preview versions. For Linux, it checks common locations like `/usr/bin/pwsh` and `/usr/local/bin/pwsh`.
    /// </para>
    /// </remarks>
    public static void RegisterPathHint()
    {
        PathFinder.Default.RegisterOrUpdate("powershell", (hint) =>
        {
            hint.Executable = "powershell";
            hint.Variable = "POWERSHELL_EXE";
            hint.Windows = [
                 "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe",
            ];
            hint.Linux = [
                "/usr/bin/pwsh",
                "/usr/local/bin/pwsh",
                "$HOME/.local/bin/pwsh",
            ];
        });
    }
}