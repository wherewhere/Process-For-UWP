using ProcessForUWP.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace ProcessForUWP.Desktop
{
    /// <inheritdoc cref="File"/>
    [ComVisible(true)]
    public sealed partial class RemoteFile : IFileStatic
    {
        private const uint SE_FILE_OBJECT = 1;
        private const uint DACL_SECURITY_INFORMATION = 0x00000004;

        /// <summary>
        /// Gets the singleton instance of the <see cref="RemoteFile"/> class.
        /// </summary>
        public static RemoteFile Instance { get; } = new();

        /// <inheritdoc cref="File.Copy(string, string)"/>
        public void Copy(string sourceFileName, string destFileName) => File.Copy(sourceFileName, destFileName);

        /// <inheritdoc cref="File.Copy(string, string, bool)"/>
        public void Copy(string sourceFileName, string destFileName, bool overwrite) => File.Copy(sourceFileName, destFileName, overwrite);

        /// <inheritdoc cref="File.Exists(string)"/>
        public bool Exists([NotNullWhen(true)] string? path) => File.Exists(path);

        /// <inheritdoc cref="File.GetAttributes(string)"/>
        public CoFileAttributes GetAttributes(string path) => (CoFileAttributes)File.GetAttributes(path);

        /// <inheritdoc cref="File.GetCreationTimeUtc(string)"/>
        public DateTimeOffset GetCreationTime(string path) => File.GetCreationTimeUtc(path);

        /// <inheritdoc cref="File.GetLastAccessTimeUtc(string)"/>
        public DateTimeOffset GetLastAccessTime(string path) => File.GetLastAccessTimeUtc(path);

        /// <inheritdoc cref="File.GetLastWriteTimeUtc(string)"/>
        public DateTimeOffset GetLastWriteTime(string path) => File.GetLastWriteTimeUtc(path);

        /// <inheritdoc cref="File.OpenRead(string)"/>
        public IOutputStream OpenRead(string path) => File.OpenRead(path).AsOutputStream();

        /// <inheritdoc cref="File.OpenText(string)"/>
        public IStreamReader OpenText(string path) => RemoteStreamReader.ConvertToWrapper(File.OpenText(path));

        /// <inheritdoc cref="File.ReadAllBytes(string)"/>
        public byte[] ReadAllBytes(string path) => File.ReadAllBytes(path);

#if COMP_NETSTANDARD2_1 || WINDOWS_UWP
        /// <inheritdoc cref="File.ReadAllBytesAsync(string, System.Threading.CancellationToken)"/>
#else
        /// <inheritdoc cref="FileEx.ReadAllBytesAsync(string, System.Threading.CancellationToken)"/>
#endif
        public IAsyncOperation<IBuffer> ReadAllBytesAsync(string path) => AsyncInfo.Run(x => File.ReadAllBytesAsync(path, x).ContinueWith(x => x.Result.AsBuffer()));

        /// <inheritdoc cref="File.ReadAllLines(string)"/>
        public string[] ReadAllLines(string path) => File.ReadAllLines(path);

#if COMP_NETSTANDARD2_1 || WINDOWS_UWP
        /// <inheritdoc cref="File.ReadAllLinesAsync(string, System.Threading.CancellationToken)"/>
#else
        /// <inheritdoc cref="FileEx.ReadAllLinesAsync(string, System.Threading.CancellationToken)"/>
#endif
        public IAsyncOperation<IReadOnlyList<string>> ReadAllLinesAsync(string path) => AsyncInfo.Run(x => File.ReadAllLinesAsync(path, x).ContinueWith(x => x.Result as IReadOnlyList<string>));

        /// <inheritdoc cref="File.ReadAllText(string)"/>
        public string ReadAllText(string path) => File.ReadAllText(path);

#if COMP_NETSTANDARD2_1 || WINDOWS_UWP
        /// <inheritdoc cref="File.ReadAllTextAsync(string, System.Threading.CancellationToken)"/>
#else
        /// <inheritdoc cref="FileEx.ReadAllTextAsync(string, System.Threading.CancellationToken)"/>
#endif
        public IAsyncOperation<string> ReadAllTextAsync(string path) => AsyncInfo.Run(x => File.ReadAllTextAsync(path, x));

        /// <summary>
        /// Creates a file hard link identified by <paramref name="path"/> that points to <paramref name="pathToTarget"/>.
        /// </summary>
        /// <param name="path">The path where the symbolic link should be created.</param>
        /// <param name="pathToTarget">The path of the target to which the symbolic link points.</param>
        /// <returns><see langword="true"/> if the hard link was successfully created; otherwise, <see langword="false"/>.</returns>
        public bool CreateHardLink(string path, string pathToTarget) => CreateHardLinkW(path, pathToTarget);

        /// <summary>
        /// Copies the discretionary access control list (DACL) from one file to another.
        /// </summary>
        /// <param name="sourceFileName">The name of the source file from which to copy the DACL.</param>
        /// <param name="destFileName">The name of the destination file to which the DACL will be copied.</param>
        /// <returns><see langword="true"/> if the DACL was successfully copied; otherwise, <see langword="false"/>.</returns>
        public bool CopyDiscretionaryAccessControlList(string sourceFileName, string destFileName) =>
            GetNamedSecurityInfoW(sourceFileName, SE_FILE_OBJECT, DACL_SECURITY_INFORMATION, out _, out _, out nint dacl, out _, out _) == 0
            && SetNamedSecurityInfoW(destFileName, SE_FILE_OBJECT, DACL_SECURITY_INFORMATION, 0, 0, dacl, 0) == 0;

        /// <summary>
        /// Establishes a hard link between an existing file and a new file. This function is only supported on the NTFS file system, and only for files, not directories.
        /// </summary>
        /// <param name="lpFileName">The name of the new file.</param>
        /// <param name="lpExistingFileName">The name of the existing file.</param>
        /// <param name="lpSecurityAttributes">Reserved; must be <see langword="null"/>.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
#if NET7_0_OR_GREATER
        [LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool CreateHardLinkW(string lpFileName, string lpExistingFileName, nint lpSecurityAttributes = 0);
#else
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, BestFitMapping = false, SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CreateHardLinkW(string lpFileName, string lpExistingFileName, nint lpSecurityAttributes = 0);
#endif

        /// <summary>
        /// The <see cref="GetNamedSecurityInfoW"/> function retrieves a copy of the security descriptor for an object specified by name.
        /// </summary>
        /// <param name="name">A pointer to a null-terminated string that specifies the name of the object from which to retrieve security information.</param>
        /// <param name="objectType">Specifies a value from the SE_OBJECT_TYPE enumeration that indicates the type of object named by the <paramref name="name"/> parameter.</param>
        /// <param name="securityInformation">A set of bit flags that indicate the type of security information to retrieve. This parameter can be a combination of the SECURITY_INFORMATION bit flags.</param>
        /// <param name="sidOwner">A pointer to a variable that receives a pointer to the owner SID in the security descriptor returned in <paramref name="securityDescriptor"/> or <see langword="null"/> if the security descriptor has no owner SID.</param>
        /// <param name="sidGroup">A pointer to a variable that receives a pointer to the primary group SID in the returned security descriptor or <see langword="null"/> if the security descriptor has no group SID.</param>
        /// <param name="dacl">A pointer to a variable that receives a pointer to the DACL in the returned security descriptor or <see langword="null"/> if the security descriptor has no DACL.</param>
        /// <param name="sacl">A pointer to a variable that receives a pointer to the SACL in the returned security descriptor or <see langword="null"/> if the security descriptor has no SACL.</param>
        /// <param name="securityDescriptor">A pointer to a variable that receives a pointer to the security descriptor of the object.</param>
        /// <returns>If the function succeeds, the return value is ERROR_SUCCESS.</returns>
#if NET7_0_OR_GREATER
        [LibraryImport("advapi32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        private static partial uint GetNamedSecurityInfoW(string name, uint objectType, uint securityInformation, out nint sidOwner, out nint sidGroup, out nint dacl, out nint sacl, out nint securityDescriptor);
#else
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        private static extern uint GetNamedSecurityInfoW(string name, uint objectType, uint securityInformation, out nint sidOwner, out nint sidGroup, out nint dacl, out nint sacl, out nint securityDescriptor);
#endif

        /// <summary>
        /// The <see cref="SetNamedSecurityInfoW"/> function sets specified security information in the security descriptor of a specified object. The caller identifies the object by name.
        /// </summary>
        /// <param name="name">A pointer to a null-terminated string that specifies the name of the object for which to set security information.</param>
        /// <param name="objectType">A value of the SE_OBJECT_TYPE enumeration that indicates the type of object named by the <paramref name="name"/> parameter.</param>
        /// <param name="securityInformation">A set of bit flags that indicate the type of security information to set. This parameter can be a combination of the SECURITY_INFORMATION bit flags.</param>
        /// <param name="owner">A pointer to a SID structure that identifies the owner of the object.</param>
        /// <param name="group">A pointer to a SID that identifies the primary group of the object.</param>
        /// <param name="dacl">A pointer to the new DACL for the object.</param>
        /// <param name="sacl">A pointer to the new SACL for the object.</param>
        /// <returns>If the function succeeds, the function returns ERROR_SUCCESS.</returns>
#if NET7_0_OR_GREATER
        [LibraryImport("advapi32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        private static partial uint SetNamedSecurityInfoW(string name, uint objectType, uint securityInformation, nint owner, nint group, nint dacl, nint sacl);
#else
        [DllImport("advapi32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern uint SetNamedSecurityInfoW(string name, uint objectType, uint securityInformation, nint owner, nint group, nint dacl, nint sacl);
#endif
    }
}
