#if !NET8_0_OR_GREATER
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ProcessForUWP.Desktop.Polyfills
{
    /// <inheritdoc cref="ArgumentNullException"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class ArgumentNullExceptionEx
    {
        /// <summary>
        /// The extension for <see cref="ArgumentNullException"/> class.
        /// </summary>
        extension(ArgumentNullException)
        {
            /// <summary>
            /// Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is null.
            /// </summary>
            /// <param name="argument">The reference type argument to validate as non-null.</param>
            /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
            public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
            {
                if (argument is null)
                {
                    Throw(paramName);
                }
            }

            /// <summary>
            /// Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is null.
            /// </summary>
            /// <param name="argument">The pointer argument to validate as non-null.</param>
            /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
            public static unsafe void ThrowIfNull([NotNull] void* argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
            {
                if (argument is null)
                {
                    Throw(paramName);
                }
            }

            /// <summary>
            /// Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is null.
            /// </summary>
            /// <param name="argument">The pointer argument to validate as non-null.</param>
            /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
            internal static unsafe void ThrowIfNull(nint argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
            {
                if (argument == 0)
                {
                    Throw(paramName);
                }
            }

            [DoesNotReturn]
            internal static void Throw(string? paramName) => throw new ArgumentNullException(paramName);
        }
    }
}
#endif