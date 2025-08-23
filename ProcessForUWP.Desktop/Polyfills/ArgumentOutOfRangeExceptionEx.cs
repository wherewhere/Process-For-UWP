#if !NET8_0_OR_GREATER
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ProcessForUWP.Desktop.Polyfills
{
    /// <inheritdoc cref="ArgumentOutOfRangeException"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class ArgumentOutOfRangeExceptionEx
    {
        /// <summary>
        /// The extension for <see cref="ArgumentOutOfRangeException"/> class.
        /// </summary>
        extension(ArgumentOutOfRangeException)
        {
            [DoesNotReturn]
            private static void ThrowGreater<T>(T value, T other, string? paramName) =>
                throw new ArgumentOutOfRangeException(paramName, value, string.Format("{0} ('{1}') must be less than or equal to '{2}'.", paramName, value, other));

            [DoesNotReturn]
            private static void ThrowLess<T>(T value, T other, string? paramName) =>
                throw new ArgumentOutOfRangeException(paramName, value, string.Format("{0} ('{1}') must be greater than or equal to '{2}'.", paramName, value, other));

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than <paramref name="other"/>.
            /// </summary>
            /// <param name="value">The argument to validate as less or equal than <paramref name="other"/>.</param>
            /// <param name="other">The value to compare with <paramref name="value"/>.</param>
            /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
            public static void ThrowIfGreaterThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
                where T : IComparable<T>
            {
                if (value.CompareTo(other) > 0)
                { ArgumentOutOfRangeException.ThrowGreater(value, other, paramName); }
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than <paramref name="other"/>.
            /// </summary>
            /// <param name="value">The argument to validate as greater than or equal than <paramref name="other"/>.</param>
            /// <param name="other">The value to compare with <paramref name="value"/>.</param>
            /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
            public static void ThrowIfLessThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
                where T : IComparable<T>
            {
                if (value.CompareTo(other) < 0)
                { ArgumentOutOfRangeException.ThrowLess(value, other, paramName); }
            }
        }
    }
}
#endif