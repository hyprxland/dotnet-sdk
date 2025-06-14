using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Hyprx.Results;

namespace Hyprx.Extras;

public static partial class ExceptionMembers
{
    extension(ArgumentNullException)
    {
#if !NET8_0_OR_GREATER
        /// <summary>Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is null.</summary>
        /// <param name="argument">The reference type argument to validate as non-null.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
        public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            if (argument is null)
                throw new ArgumentNullException(paramName);
        }

        /// <summary>Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is null.</summary>
        /// <param name="argument">The pointer argument to validate as non-null.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
        [CLSCompliant(false)]
        public static unsafe void ThrowIfNull([NotNull] void* argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            if (argument is null)
                 throw new ArgumentNullException(paramName);
        }

        /// <summary>Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is null.</summary>
        /// <param name="argument">The pointer argument to validate as non-null.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
        public static void ThrowIfNull(IntPtr argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            if (argument == IntPtr.Zero)
                throw new ArgumentNullException(paramName);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if <paramref name="value"/> is null or empty.
        /// </summary>
        /// <param name="value">The string value to validate as non-null or empty.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        ///
        public static void ThrowIfNullOrEmpty([NotNullIfNotNull(nameof(value))] string? value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(paramName);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if <paramref name="value"/> is null, empty, or whitespace.
        /// </summary>
        /// <param name="value">The string value to validate as non-null, empty, or whitespace.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        /// ///
        public static void ThrowIfNullOrWhiteSpace([NotNullIfNotNull(nameof(value))] string? value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(paramName);
        }
#endif
    }
}