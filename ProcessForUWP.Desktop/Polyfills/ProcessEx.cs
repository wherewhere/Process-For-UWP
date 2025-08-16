#if !NET7_0_OR_GREATER
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace ProcessForUWP.Desktop.Polyfills
{
    /// <inheritdoc cref="Process"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class ProcessEx
    {
        /// <summary>
        /// The extension for <see cref="Process"/> class.
        /// </summary>
        extension(Process process)
        {
            /// <summary>
            /// Instructs the Process component to wait the specified amount of time for the associated process to exit.
            /// </summary>
            /// <param name="timeout">The amount of time to wait for the associated process to exit.</param>
            /// <returns><see langword="true"/> if the associated process has exited; otherwise, <see langword="false"/>.</returns>
            public bool WaitForExit(TimeSpan timeout) => process.WaitForExit(Process.ToTimeoutMilliseconds(timeout));

            /// <summary>
            /// Causes the <see cref="Process"/> component to wait the specified <paramref name="timeout"/> for the associated process to enter an idle state.
            /// This overload applies only to processes with a user interface and, therefore, a message loop.
            /// </summary>
            /// <param name="timeout">The amount of time, in milliseconds, to wait for the associated process to become idle.</param>
            /// <returns><see langword="true"/> if the associated process has reached an idle state; otherwise, <see langword="false"/>.</returns>
            public bool WaitForInputIdle(TimeSpan timeout) => process.WaitForInputIdle(Process.ToTimeoutMilliseconds(timeout));

            /// <summary>
            /// Converts a <see cref="TimeSpan"/> to milliseconds for use in process timeout operations.
            /// </summary>
            /// <param name="timeout">The <see cref="TimeSpan"/> to convert.</param>
            /// <returns>The timeout in milliseconds.</returns>
            private static int ToTimeoutMilliseconds(TimeSpan timeout)
            {
                long totalMilliseconds = (long)timeout.TotalMilliseconds;

                ArgumentOutOfRangeException.ThrowIfLessThan(totalMilliseconds, -1, nameof(timeout));
                ArgumentOutOfRangeException.ThrowIfGreaterThan(totalMilliseconds, int.MaxValue, nameof(timeout));

                return (int)totalMilliseconds;
            }
        }
    }
}
#endif