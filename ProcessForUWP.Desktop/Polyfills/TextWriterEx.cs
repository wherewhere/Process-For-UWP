using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;

namespace ProcessForUWP.Desktop.Polyfills
{
    /// <inheritdoc cref="TextWriter"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class TextWriterEx
    {
        /// <summary>
        /// The extension for the <see cref="TextWriter"/> class.
        /// </summary>
        extension(TextWriter writer)
        {
            /// <summary>
            /// Asynchronously releases all resources used by the <see cref="TextWriter"/> object.
            /// </summary>
            /// <returns>A task that represents the asynchronous dispose operation.</returns>
            public IAsyncAction DisposeAsync() => AsyncInfo.Run(_ =>
            {
                writer.Dispose();
                return Task.CompletedTask;
            });
        }
    }
}
