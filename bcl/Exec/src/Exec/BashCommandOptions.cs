using Hyprx.Extras;

namespace Hyprx.Exec;

public sealed class BashCommandOptions : ShellCommandOptions
{
    public BashCommandOptions()
    {
        this.File = "bash";
        this.FileExtensions = [".sh"];
    }

    protected override CommandArgs FinalizeArgs()
    {
        if (this.Script.IsNullOrWhiteSpace())
        {
            throw new InvalidOperationException("Script must be set to get script arguments.");
        }

        var script = this.Script.Trim();
        var isFilePath = !this.Script.Any(o => o is '\n' or '\r') &&
                          this.FileExtensions.Any(ext => script.EndsWith(ext, StringComparison.OrdinalIgnoreCase));

        var args = this.ScriptArgs.ToList();
        string? file = null;
        if (args.Count == 0)
        {
            args.AddRange(["-noprofile", "--norc", "-e", "-o", "pipefail"]);
        }

        if (!isFilePath && this.UseScriptAsFile)
        {
            isFilePath = true;
            file = this.GenerateDisposableFile(this.Script);
        }
        else if (isFilePath)
        {
            file = script;
        }

        if (isFilePath && !file.IsNullOrWhiteSpace())
        {
            if (OperatingSystem.IsWindows())
            {
                var exe = PathFinder.Which("bash");
                if (!exe.IsNullOrWhiteSpace() && exe.EndsWithFold("System32\\bash.exe"))
                {
                    if (!Path.IsPathRooted(file))
                    {
                        file = Path.GetFullPath(file);
                    }

                    file = $"/mnt/" + char.ToLowerInvariant(file[0]) + file[2..].Replace('\\', '/');
                }
            }

            args.Add(file);
            return [.. args];
        }

        args.AddRange(["-c", this.Script]);
        return [.. args];
    }
}