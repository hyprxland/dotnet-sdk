using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace Hyprx.Exec;

public partial class Command : Command<Command, CommandOptions>, ICommandOptionsOwner
{
    public Command()
    {
    }

    public Command(CommandArgs args)
        : this()
    {
        ArgumentOutOfRangeException.ThrowIfNullOrEmpty(args);
        var exe = args[0];
        args.RemoveAt(0);
        this.Options.File = exe;
        this.Options.Args = args;
    }

    public Command(CommandOptions options)
    {
        this.Options = options;
    }
}