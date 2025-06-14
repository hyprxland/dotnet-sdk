namespace Hyprx.Exec;

public sealed class PwshCommandOptions : ShellCommandOptions
{
    public PwshCommandOptions()
    {
        this.File = "pwsh";
        this.FileExtensions = [".ps1"];
    }

    protected override CommandArgs FinalizeArgs()
    {
        if (this.Script.IsNullOrWhiteSpace())
        {
            throw new InvalidOperationException("Script must be set to get script arguments.");
        }

        var script = this.Script.Trim();
        var isFilePath = !script.Any(o => o is '\n' or '\r') &&
                          this.FileExtensions.Any(ext => script.EndsWith(ext, StringComparison.OrdinalIgnoreCase));

        var args = this.ScriptArgs.ToList();
        if (args.Count == 0)
        {
            args.AddRange(["-NoLogo", "-NoProfile", "-NonInteractive", "-ExecutionPolicy", "Bypass"]);
        }

        if (isFilePath)
        {
            args.AddRange(["-File", script]);
            return [.. args];
        }

        if (this.UseScriptAsFile)
        {
            script = $@"
$ErrorActionPreference = 'Stop'
{script}
if ($null -eq $LASTEXITCODE) {{
    exit 0
}} else {{
    exit $LASTEXITCODE
}}
";
            var filePath = this.GenerateDisposableFile(script);
            args.AddRange(["-File", filePath]);
        }
        else
        {
            script = $@"
$ErrorActionPreference = 'Stop'
{script}
if ($null -eq $LASTEXITCODE) {{
    exit 0
}} else {{
    exit $LASTEXITCODE
}}
";
            args.AddRange(["-Command", script]);
        }

        if (this.Args.Count > 0)
        {
            args.AddRange(this.Args);
        }

        return [.. args];
    }
}