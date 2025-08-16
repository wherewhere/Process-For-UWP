using ProcessForUWP.Core;
using System.IO;
using System.Runtime.InteropServices;

namespace ProcessForUWP.Desktop
{
    /// <inheritdoc cref="File"/>
    [ComVisible(true)]
    public partial class RemoteFile : IFileStatic
    {
        /// <summary>
        /// Gets the singleton instance of the <see cref="RemoteFile"/> class.
        /// </summary>
        public static RemoteFile Instance { get; } = new();

        /// <inheritdoc cref="File.Copy(string, string)"/>
        public void Copy(string sourceFileName, string destFileName) => File.Copy(sourceFileName, destFileName);

        /// <inheritdoc cref="File.Copy(string, string, bool)"/>
        public void Copy(string sourceFileName, string destFileName, bool overwrite) => File.Copy(sourceFileName, destFileName, overwrite);

        /// <inheritdoc cref="File.Exists(string)"/>
        public bool Exists(string path) => File.Exists(path);
    }
}
