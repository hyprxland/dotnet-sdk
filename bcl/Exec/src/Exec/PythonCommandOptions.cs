namespace Hyprx.Exec;

public sealed class PythonCommandOptions : ShellCommandOptions
{
    public PythonCommandOptions()
    {
        this.File = "python";
        this.FileExtensions = [".py"];
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

        if (isFilePath || this.UseScriptAsFile)
        {
            string file = string.Empty;
            if (!isFilePath)
            {
                file = this.GenerateDisposableFile(this.Script);
            }
            else
            {
                file = script;
            }

            args.Add(file);
            return [.. args];
        }

        if (args.Count == 0)
        {
            args.Add("-c");
        }

        args.Add(this.Script);
        return [.. args];
    }
}