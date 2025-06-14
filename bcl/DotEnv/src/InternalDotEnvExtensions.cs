using System.Text;

namespace Hyprx.DotEnv;

public static class InternalDotEnvExtensions
{
    public static char[] ToArray(this StringBuilder builder)
    {
        var copy = new char[builder.Length];
        builder.CopyTo(0, copy, 0, builder.Length);
        return copy;
    }

    public static string AsString(this ReadOnlySpan<char> span)
    {
#if NET5_0_OR_GREATER
        return span.ToString();
#else
        return new string(span.ToArray());
#endif
    }
}