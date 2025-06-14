using System.Diagnostics;

namespace Hyprx.Exec;

public partial class Command
{
    internal static Action<ProcessStartInfo>? GlobalWriteCommand { get; set; }

    public static void SetGlobalWriteCommand(Action<ProcessStartInfo>? writeCommand)
    {
        GlobalWriteCommand = writeCommand;
    }
}