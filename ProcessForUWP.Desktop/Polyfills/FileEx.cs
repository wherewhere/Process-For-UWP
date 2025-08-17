#if !WINDOWS_UWP && !COMP_NETSTANDARD2_1
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessForUWP.Desktop.Polyfills
{
    /// <inheritdoc cref="File"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class FileEx
    {
        /// <summary>
        /// The extension for the <see cref="File"/> class.
        /// </summary>
        extension(File)
        {
            /// <summary>
            /// Asynchronously opens a binary file, reads the contents of the file into a byte array, and then closes the file.
            /// </summary>
            /// <param name="path">The file to open for reading.</param>
            /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
            /// <returns>A task that represents the asynchronous read operation, which wraps the byte array containing the contents of the file.</returns>
            public static Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default) =>
                Task.Run(() => File.ReadAllBytes(path), cancellationToken);

            /// <summary>
            /// Asynchronously opens a text file, reads all lines of the file, and then closes the file.
            /// </summary>
            /// <param name="path">The file to open for reading.</param>
            /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
            /// <returns>A task that represents the asynchronous read operation, which wraps the string array containing all lines of the file.</returns>
            public static Task<string[]> ReadAllLinesAsync(string path, CancellationToken cancellationToken = default) =>
                Task.Run(() => File.ReadAllLines(path), cancellationToken);

            /// <summary>
            /// Asynchronously opens a text file, reads all the text in the file, and then closes the file.
            /// </summary>
            /// <param name="path">The file to open for reading.</param>
            /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
            /// <returns>A task that represents the asynchronous read operation, which wraps the string containing all text in the file.</returns>
            public static Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default) =>
                Task.Run(() => File.ReadAllText(path), cancellationToken);
        }
    }
}
#endif