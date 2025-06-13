namespace Hyprx;

/// <summary>
/// Options for environment variable expansion.
/// </summary>
public class EnvExpandOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to expand environment
    /// variables using Windows-style syntax (e.g., %VAR%).
    /// </summary>
    public bool WindowsExpansion { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to expand environment
    /// variables using Unix-style syntax (e.g., $VAR or ${VAR}).
    /// </summary>
    public bool UnixExpansion { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to allow Unix-style
    /// variable assignment (e.g., ${VAR:=value}).
    /// </summary>
    public bool UnixAssignment { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to use a custom error message
    /// for Unix-style variable expansion errors.
    /// </summary>
    public bool UnixCustomErrorMessage { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to enable Unix-style
    /// argument expansion (e.g., $1, $2, etc.).
    /// </summary>
    public bool UnixArgsExpansion { get; set; }

    /// <summary>
    /// Gets or sets a function to retrieve environment variable values.
    /// </summary>
    public Func<string, string?>? GetVariable { get; set; }

    /// <summary>
    /// Gets or sets a function to set environment variable values.
    /// </summary>
    public Action<string, string>? SetVariable { get; set; }
}