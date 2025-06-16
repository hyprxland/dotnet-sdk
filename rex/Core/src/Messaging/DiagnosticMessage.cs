namespace Hyprx.Rex.Messaging;

public class DiagnosticMessage : IMessage
{
    public DiagnosticMessage(DiagnosticLevel level, string message, params object?[] args)
    {
        this.Level = level;
        this.Message = message;
        this.Args = args;
    }

    public DiagnosticMessage(DiagnosticLevel level, Exception exception, string? message = null, params object?[] args)
    {
        this.Level = level;
        this.Message = message;
        this.Args = args;
        this.Exception = exception;
    }

    public string Topic => "diagnostics";

    public DiagnosticLevel Level { get; }

    public Exception? Exception { get; init; }

    public string? Message { get; }

    public object?[] Args { get; }

    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;

    public static DiagnosticMessage Trace(string message, params object?[] args)
        => new(DiagnosticLevel.Trace, message, args);

    public static DiagnosticMessage Trace(Exception exception, string? message = null, params object?[] args)
        => new(DiagnosticLevel.Trace, exception, message, args);

    public static DiagnosticMessage Error(string message, params object?[] args)
        => new(DiagnosticLevel.Error, message, args);

    public static DiagnosticMessage Error(Exception exception, string? message = null, params object?[] args)
        => new(DiagnosticLevel.Error, exception, message, args);

    public static DiagnosticMessage Warning(string message, params object?[] args)
        => new(DiagnosticLevel.Warning, message, args);

    public static DiagnosticMessage Warning(Exception exception, string? message = null, params object?[] args)
        => new(DiagnosticLevel.Warning, exception, message, args);

    public static DiagnosticMessage Info(string message, params object?[] args)
        => new(DiagnosticLevel.Info, message, args);

    public static DiagnosticMessage Info(Exception exception, string? message = null, params object?[] args)
        => new(DiagnosticLevel.Info, exception, message, args);

    public static DiagnosticMessage Debug(string message, params object?[] args)
        => new(DiagnosticLevel.Debug, message, args);

    public static DiagnosticMessage Debug(Exception exception, string? message = null, params object?[] args)
        => new(DiagnosticLevel.Debug, exception, message, args);

    public static DiagnosticMessage Fatal(string message, params object?[] args)
        => new(DiagnosticLevel.Fatal, message, args);

    public static DiagnosticMessage Fatal(Exception exception, string? message = null, params object?[] args)
        => new(DiagnosticLevel.Fatal, exception, message, args);

    public static DiagnosticMessage Notice(string message, params object?[] args)
        => new(DiagnosticLevel.Notice, message, args);

    public static DiagnosticMessage Notice(Exception exception, string? message = null, params object?[] args)
        => new(DiagnosticLevel.Notice, exception, message, args);
}