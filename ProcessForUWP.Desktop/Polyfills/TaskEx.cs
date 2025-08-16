#if NETFRAMEWORK && !NET46_OR_GREATER
using System.ComponentModel;
using System.Threading.Tasks;

namespace ProcessForUWP.Desktop.Polyfills
{
    /// <inheritdoc cref="Task"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class TaskEx
    {
        /// <summary>
        /// Singleton cached task that's been completed successfully.
        /// </summary>
        private static readonly Task s_cachedCompleted = Task.FromResult<object?>(null);

        /// <summary>
        /// The extension for the <see cref="Task"/> class.
        /// </summary>
        extension(Task)
        {
            /// <summary>
            /// Gets a task that's already been completed successfully.
            /// </summary>
            public static Task CompletedTask => s_cachedCompleted;
        }
    }
}
#endif