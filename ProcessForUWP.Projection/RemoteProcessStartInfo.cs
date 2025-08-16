#if !NETSTANDARD || NETSTANDARD2_0_OR_GREATER
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ProcessForUWP.Core
{
#if NETFRAMEWORK || COMP_NETSTANDARD2_0
    using System.Diagnostics;
    using WinRTWrapper.CodeAnalysis;

    [ComVisible(true)]
    [WinRTWrapperMarshaller(typeof(ProcessStartInfo), typeof(IProcessStartInfo))]
    [GenerateWinRTWrapper(typeof(ProcessStartInfo), GenerateMember.Defined)]
    public sealed partial class RemoteProcessStartInfo : IProcessStartInfo
    {
        /// <inheritdoc cref="ProcessStartInfo()"/>
        public RemoteProcessStartInfo() : this(new ProcessStartInfo()) { }

        /// <inheritdoc cref="ProcessStartInfo(string)"/>
        public RemoteProcessStartInfo(string fileName) : this(new ProcessStartInfo(fileName)) { }

        /// <inheritdoc cref="ProcessStartInfo(string, string)"/>
        public RemoteProcessStartInfo(string fileName, string arguments) : this(new ProcessStartInfo(fileName, arguments)) { }

        public partial string[] Verbs { get; }
        public partial string Verb { get; set; }
        public partial bool UseShellExecute { get; set; }
        public partial string UserName { get; set; }
        public partial bool RedirectStandardOutput { get; set; }
        public partial bool RedirectStandardInput { get; set; }
        public partial bool RedirectStandardError { get; set; }

#if !NETFRAMEWORK || NET461_OR_GREATER
        public partial string? PasswordInClearText { get; set; }
#else
        /// <summary>
        /// Gets or sets the user password in clear text to use when starting the process.
        /// </summary>
        public string? PasswordInClearText { get; set; }
#endif

        /// <inheritdoc cref="ProcessStartInfo.WindowStyle"/>
        public CoProcessWindowStyle WindowStyle
        {
            get => (CoProcessWindowStyle)target.WindowStyle;
            set => target.WindowStyle = (ProcessWindowStyle)value;
        }

        public partial bool LoadUserProfile { get; set; }
        public partial string FileName { get; set; }
        public partial bool ErrorDialog { get; set; }

#if !NETFRAMEWORK || NET46_OR_GREATER
        public partial IDictionary<string, string?> Environment { get; }
#else
        private Dictionary<string, string?>? _environmentVariables;
        /// <summary>
        /// Gets the environment variables that apply to this process and its child processes.
        /// </summary>
        public IDictionary<string, string?> Environment
        {
            get
            {
                if (_environmentVariables == null)
                {
                    System.Collections.IDictionary envVars = System.Environment.GetEnvironmentVariables();

                    _environmentVariables = new Dictionary<string, string?>(
                        envVars.Count,
                        System.Environment.OSVersion.Platform is
                            System.PlatformID.Win32NT or System.PlatformID.Win32S or System.PlatformID.Xbox
                            or System.PlatformID.Win32Windows or System.PlatformID.WinCE
                            ? System.StringComparer.OrdinalIgnoreCase : System.StringComparer.Ordinal);

                    // Manual use of IDictionaryEnumerator instead of foreach to avoid DictionaryEntry box allocations.
                    System.Collections.IDictionaryEnumerator e = envVars.GetEnumerator();
                    while (e.MoveNext())
                    {
                        System.Collections.DictionaryEntry entry = e.Entry;
                        _environmentVariables.Add((string)entry.Key, (string?)entry.Value);
                    }
                }
                return _environmentVariables;
            }
        }
#endif

        public partial string Domain { get; set; }
        public partial bool CreateNoWindow { get; set; }
        public partial string Arguments { get; set; }

        public partial string WorkingDirectory { get; set; }

        /// <summary>
        /// Converts a wrapper type <see cref="IProcessStartInfo"/> to a managed type <see cref="ProcessStartInfo"/>.
        /// </summary>
        /// <param name="wrapper">The wrapper type to convert.</param>
        /// <returns>The converted managed type.</returns>
        public static ProcessStartInfo ConvertToManaged(IProcessStartInfo wrapper) =>
            wrapper is RemoteProcessStartInfo remoteStartInfo
                ? remoteStartInfo.target
                : new ProcessStartInfo
                {
                    Verb = wrapper.Verb,
                    UseShellExecute = wrapper.UseShellExecute,
                    UserName = wrapper.UserName,
                    RedirectStandardOutput = wrapper.RedirectStandardOutput,
                    RedirectStandardInput = wrapper.RedirectStandardInput,
                    RedirectStandardError = wrapper.RedirectStandardError,
#if !NETFRAMEWORK || NET461_OR_GREATER
                    PasswordInClearText = wrapper.PasswordInClearText,
#endif
                    WindowStyle = (ProcessWindowStyle)wrapper.WindowStyle,
                    LoadUserProfile = wrapper.LoadUserProfile,
                    FileName = wrapper.FileName,
                    ErrorDialog = wrapper.ErrorDialog,
                    Domain = wrapper.Domain,
                    CreateNoWindow = wrapper.CreateNoWindow,
                    Arguments = wrapper.Arguments,
                    WorkingDirectory = wrapper.WorkingDirectory
                };
    }
#else
    /// <summary>
    /// Specifies a set of values that are used when you start a process.
    /// </summary>
    [ComVisible(true)]
    public sealed partial class RemoteProcessStartInfo : IProcessStartInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteProcessStartInfo"/> class without specifying a file name with which to start the process.
        /// </summary>
        public RemoteProcessStartInfo() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteProcessStartInfo"/> class and specifies a file name such as an application or document with which to start the process.
        /// </summary>
        /// <param name="fileName">An application or document with which to start a process.</param>
        public RemoteProcessStartInfo(string fileName) : this() => FileName = fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteProcessStartInfo"/> class, specifies an application file name with which to start the process, and specifies a set of command-line arguments to pass to the application.
        /// </summary>
        /// <param name="fileName">An application with which to start a process.</param>
        /// <param name="arguments">Command-line arguments to pass to the application when the process starts.</param>
        public RemoteProcessStartInfo(string fileName, string arguments) : this(fileName) => Arguments = arguments;

        /// <summary>
        /// Gets the set of verbs associated with the type of file specified by the <see cref="FileName"/> property.
        /// </summary>
        [System.Obsolete("Not Implemented")]
        public string[] Verbs => [];

        /// <summary>
        /// Gets or sets the verb to use when opening the application or document specified by the <see cref="FileName"/> property.
        /// </summary>
        public string Verb { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to use the operating system shell to start the process.
        /// </summary>
        public bool UseShellExecute { get; set; }

        /// <summary>
        /// Gets or sets the user name to use when starting the process. If you use the UPN format, <c>user@DNS_domain_name</c>, the <see cref="Domain"/> property must be <see langword="null"/>.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value that indicates whether the textual output of an application is written to the <see cref="IProcess.StandardOutput"/> stream.
        /// </summary>
        public bool RedirectStandardOutput { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the input for an application is read from the <see cref="IProcess.StandardInput"/> stream.
        /// </summary>
        public bool RedirectStandardInput { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the error output of an application is written to the <see cref="IProcess.StandardError"/> stream.
        /// </summary>
        public bool RedirectStandardError { get; set; }

        /// <summary>
        /// Gets or sets the user password in clear text to use when starting the process.
        /// </summary>
        public string? PasswordInClearText { get; set; }

        /// <summary>
        /// Gets or sets the window state to use when the process is started.
        /// </summary>
        public CoProcessWindowStyle WindowStyle { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the Windows user profile is to be loaded from the registry.
        /// </summary>
        public bool LoadUserProfile { get; set; }

        /// <summary>
        /// Gets or sets the application or document to start.
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether an error dialog box is displayed to the user if the process cannot be started.
        /// </summary>
        public bool ErrorDialog { get; set; }

        /// <summary>
        /// Gets the environment variables that apply to this process and its child processes.
        /// </summary>
        [System.Obsolete("Not Implemented")]
        public IDictionary<string, string?> Environment => new Dictionary<string, string?>();

        /// <summary>
        /// Gets or sets a value that identifies the domain to use when starting the process. If this value is <see langword="null"/>, the <see cref="UserName"/> property must be specified in UPN format.
        /// </summary>
        public string Domain { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to start the process in a new window.
        /// </summary>
        public bool CreateNoWindow { get; set; }

        /// <summary>
        /// Gets or sets the set of command-line arguments to use when starting the application.
        /// </summary>
        public string Arguments { get; set; } = string.Empty;

        /// <summary>
        /// When the <see cref="UseShellExecute"/> property is <see langword="false"/>, gets or sets the working directory for the process to be started.
        /// When <see cref="UseShellExecute"/> is <see langword="true"/>, gets or sets the directory that contains the process to be started.
        /// </summary>
        public string WorkingDirectory { get; set; } = string.Empty;
    }
#endif
}
#endif