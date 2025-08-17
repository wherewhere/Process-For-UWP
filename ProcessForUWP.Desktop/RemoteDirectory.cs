using ProcessForUWP.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace ProcessForUWP.Desktop
{
    /// <inheritdoc cref="Directory"/>
    [ComVisible(true)]
    public sealed partial class RemoteDirectory : IDirectoryStatic
    {
        /// <summary>
        /// Gets the singleton instance of the <see cref="RemoteDirectory"/> class.
        /// </summary>
        public static RemoteDirectory Instance { get; } = new();

        /// <inheritdoc cref="Directory.EnumerateDirectories(string)"/>
        public IEnumerable<string> EnumerateDirectories(string path) => Directory.EnumerateDirectories(path);

        /// <inheritdoc cref="Directory.EnumerateDirectories(string, string)"/>
        public IEnumerable<string> EnumerateDirectories(string path, string searchPattern) => Directory.EnumerateDirectories(path, searchPattern);

        /// <inheritdoc cref="Directory.EnumerateDirectories(string, string, SearchOption)"/>
        public IEnumerable<string> EnumerateDirectories(string path, string searchPattern, CoSearchOption searchOption) => Directory.EnumerateDirectories(path, searchPattern, (SearchOption)searchOption);

        /// <inheritdoc cref="Directory.EnumerateFiles(string)"/>
        public IEnumerable<string> EnumerateFiles(string path)=> Directory.EnumerateFiles(path);

        /// <inheritdoc cref="Directory.EnumerateFiles(string, string)"/>
        public IEnumerable<string> EnumerateFiles(string path, string searchPattern) => Directory.EnumerateFiles(path, searchPattern);

        /// <inheritdoc cref="Directory.EnumerateFiles(string, string, SearchOption)"/>
        public IEnumerable<string> EnumerateFiles(string path, string searchPattern, CoSearchOption searchOption) => Directory.EnumerateFiles(path, searchPattern, (SearchOption)searchOption);

        /// <inheritdoc cref="Directory.EnumerateFileSystemEntries(string)"/>
        public IEnumerable<string> EnumerateFileSystemEntries(string path) => Directory.EnumerateFileSystemEntries(path);

        /// <inheritdoc cref="Directory.EnumerateFileSystemEntries(string, string)"/>
        public IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern) => Directory.EnumerateFileSystemEntries(path, searchPattern);

        /// <inheritdoc cref="Directory.EnumerateFileSystemEntries(string, string, SearchOption)"/>
        public IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, CoSearchOption searchOption) => Directory.EnumerateFileSystemEntries(path, searchPattern, (SearchOption)searchOption);

        /// <inheritdoc cref="Directory.Exists(string)"/>
        public bool Exists(string path) => Directory.Exists(path);

        /// <inheritdoc cref="Directory.GetCreationTimeUtc(string)"/>
        public DateTimeOffset GetCreationTime(string path) => Directory.GetCreationTimeUtc(path);

        /// <inheritdoc cref="Directory.GetLastAccessTimeUtc(string)"/>
        public DateTimeOffset GetLastAccessTime(string path) => Directory.GetLastAccessTimeUtc(path);

        /// <inheritdoc cref="Directory.GetLastWriteTimeUtc(string)"/>
        public DateTimeOffset GetLastWriteTime(string path) => Directory.GetLastWriteTimeUtc(path);

        /// <inheritdoc cref="Directory.GetCurrentDirectory"/>
        public string GetCurrentDirectory() => Directory.GetCurrentDirectory();

        /// <inheritdoc cref="Directory.GetDirectoryRoot(string)"/>
        public string GetDirectoryRoot(string path) => Directory.GetDirectoryRoot(path);

        /// <inheritdoc cref="Directory.GetDirectories(string)"/>
        public string[] GetDirectories(string path) => Directory.GetDirectories(path);

        /// <inheritdoc cref="Directory.GetDirectories(string, string)"/>
        public string[] GetDirectories(string path, string searchPattern) => Directory.GetDirectories(path, searchPattern);

        /// <inheritdoc cref="Directory.GetDirectories(string, string, SearchOption)"/>
        public string[] GetDirectories(string path, string searchPattern, CoSearchOption searchOption) => Directory.GetDirectories(path, searchPattern, (SearchOption)searchOption);

        /// <inheritdoc cref="Directory.GetFiles(string)"/>
        public string[] GetFiles(string path) => Directory.GetFiles(path);

        /// <inheritdoc cref="Directory.GetFiles(string, string)"/>
        public string[] GetFiles(string path, string searchPattern) => Directory.GetFiles(path, searchPattern);

        /// <inheritdoc cref="Directory.GetFiles(string, string, SearchOption)"/>
        public string[] GetFiles(string path, string searchPattern, CoSearchOption searchOption) => Directory.GetFiles(path, searchPattern, (SearchOption)searchOption);

        /// <inheritdoc cref="Directory.GetFileSystemEntries(string)"/>
        public string[] GetFileSystemEntries(string path) => Directory.GetFileSystemEntries(path);

        /// <inheritdoc cref="Directory.GetFileSystemEntries(string, string)"/>
        public string[] GetFileSystemEntries(string path, string searchPattern) => Directory.GetFileSystemEntries(path, searchPattern);

        /// <inheritdoc cref="Directory.GetFileSystemEntries(string, string, SearchOption)"/>
        public string[] GetFileSystemEntries(string path, string searchPattern, CoSearchOption searchOption) => Directory.GetFileSystemEntries(path, searchPattern, (SearchOption)searchOption);

        /// <inheritdoc cref="Directory.GetLogicalDrives"/>
        public string[] GetLogicalDrives() => Directory.GetLogicalDrives();
    }
}
