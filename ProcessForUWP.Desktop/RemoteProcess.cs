using ProcessForUWP.Core;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using WinRTWrapper.CodeAnalysis;

namespace ProcessForUWP.Desktop
{
    [ComVisible(true)]
    [WinRTWrapperMarshaller(typeof(Process), typeof(IProcess))]
    [GenerateWinRTWrapper(typeof(Process), GenerateMember.Defined)]
    public partial class RemoteProcess : RemoteComponent, IProcess
    {
        public partial bool PriorityBoostEnabled { get; set; }
        public partial long PeakWorkingSet64 { get; }
        public partial long PeakVirtualMemorySize64 { get; }
        public partial long PeakPagedMemorySize64 { get; }
        public partial long PagedSystemMemorySize64 { get; }
        public partial long PagedMemorySize64 { get; }
        public partial long NonpagedSystemMemorySize64 { get; }
        public partial string MainWindowTitle { get; }
        public partial long PrivateMemorySize64 { get; }
        public partial TimeSpan PrivilegedProcessorTime { get; }
        public partial long WorkingSet64 { get; }
        public partial long VirtualMemorySize64 { get; }
        public partial TimeSpan UserProcessorTime { get; }
        public partial TimeSpan TotalProcessorTime { get; }

        /// <inheritdoc cref="Process.StartTime"/>
        public DateTimeOffset StartTime => target.StartTime;
        [WinRTWrapperMarshalUsing(typeof(RemoteStreamReader))]
        public partial IStreamReader StandardOutput { get; }
        [WinRTWrapperMarshalUsing(typeof(RemoteStreamWriter))]
        public partial IStreamWriter StandardInput { get; }
        [WinRTWrapperMarshalUsing(typeof(RemoteStreamReader))]
        public partial IStreamReader StandardError { get; }
        public partial int SessionId { get; }
        public partial bool Responding { get; }
        public partial string ProcessName { get; }
        [WinRTWrapperMarshalUsing(typeof(RemoteProcessStartInfo))]
        public partial IProcessStartInfo StartInfo { get; set; }
        public partial string MachineName { get; }
        public partial bool HasExited { get; }
        public partial int HandleCount { get; }

        /// <inheritdoc cref="Process.ExitTime"/>
        public DateTimeOffset ExitTime => target.ExitTime;

        public partial int ExitCode { get; }
        public partial bool EnableRaisingEvents { get; set; }
        public partial int BasePriority { get; }
        public partial int Id { get; }

#if NET
        /// <summary>
        /// The event weak table for the <see cref="Process.Exited"/> event.
        /// </summary>
        private readonly System.Runtime.CompilerServices.ConditionalWeakTable<EventHandler<IEventArgs>, EventHandler> _Exited_EventWeakTable = [];

        /// <inheritdoc cref="Process.Exited"/>
        public event EventHandler<IEventArgs> Exited
        {
            add
            {
                void wrapper(object? sender, EventArgs e) => value(this, RemoteEventArgs.ConvertToWrapper(e));
                EventHandler handler = wrapper;
                target.Exited += handler;
                _Exited_EventWeakTable.Add(value, handler);
            }
            remove
            {
                if (_Exited_EventWeakTable.TryGetValue(value, out EventHandler? handler))
                {
                    target.Exited -= handler;
                    _Exited_EventWeakTable.Remove(value);
                }
            }
        }

        /// <summary>
        /// The event weak table for the <see cref="Process.ErrorDataReceived"/> event.
        /// </summary>
        private readonly System.Runtime.CompilerServices.ConditionalWeakTable<CoDataReceivedEventHandler, DataReceivedEventHandler> _ErrorDataReceived_EventWeakTable = [];

        /// <inheritdoc cref="Process.ErrorDataReceived"/>
        public event CoDataReceivedEventHandler ErrorDataReceived
        {
            add
            {
                void wrapper(object sender, DataReceivedEventArgs e) => value(this, new CoDataReceivedEventArgs { Data = e.Data });
                DataReceivedEventHandler handler = wrapper;
                target.ErrorDataReceived += handler;
                _ErrorDataReceived_EventWeakTable.Add(value, handler);
            }
            remove
            {
                if (_ErrorDataReceived_EventWeakTable.TryGetValue(value, out DataReceivedEventHandler? handler))
                {
                    target.ErrorDataReceived -= handler;
                    _ErrorDataReceived_EventWeakTable.Remove(value);
                }
            }
        }

        /// <summary>
        /// The event weak table for the <see cref="Process.OutputDataReceived"/> event.
        /// </summary>
        private readonly System.Runtime.CompilerServices.ConditionalWeakTable<CoDataReceivedEventHandler, DataReceivedEventHandler> _OutputDataReceived_EventWeakTable = [];

        /// <inheritdoc cref="Process.OutputDataReceived"/>
        public event CoDataReceivedEventHandler OutputDataReceived
        {
            add
            {
                void wrapper(object sender, DataReceivedEventArgs e) => value(this, new CoDataReceivedEventArgs(e.Data));
                DataReceivedEventHandler handler = wrapper;
                target.OutputDataReceived += handler;
                _OutputDataReceived_EventWeakTable.Add(value, handler);
            }
            remove
            {
                if (_OutputDataReceived_EventWeakTable.TryGetValue(value, out DataReceivedEventHandler? handler))
                {
                    target.OutputDataReceived -= handler;
                    _OutputDataReceived_EventWeakTable.Remove(value);
                }
            }
        }
#else
        /// <summary>
        /// The singleton flag for the <see cref="Process.Exited"/> event registration.
        /// </summary>
        private bool _is_Exited_EventRegistered = false;

        /// <summary>
        /// The event registration token table for the <see cref="Process.Exited"/> event.
        /// </summary>
        private readonly System.Runtime.InteropServices.WindowsRuntime.EventRegistrationTokenTable<EventHandler> _Exited_EventTable = new();

        /// <inheritdoc cref="Process.Exited"/>
        public event EventHandler<IEventArgs> Exited
        {
            add
            {
                if (!_is_Exited_EventRegistered)
                {
                    target.Exited += (sender, e) => _Exited_EventTable.InvocationList?.Invoke(sender, e);
                    _is_Exited_EventRegistered = true;
                }
                void wrapper(object? sender, EventArgs e) => value(this, RemoteEventArgs.ConvertToWrapper(e));
                return _Exited_EventTable.AddEventHandler(wrapper);
            }
            remove
            {
                _Exited_EventTable.RemoveEventHandler(value);
            }
        }

        /// <summary>
        /// The singleton flag for the <see cref="Process.OutputDataReceived"/> event registration.
        /// </summary>
        private bool _is_OutputDataReceived_EventRegistered = false;
        
        /// <summary>
        /// The event registration token table for the <see cref="Process.OutputDataReceived"/> event.
        /// </summary>
        private readonly System.Runtime.InteropServices.WindowsRuntime.EventRegistrationTokenTable<DataReceivedEventHandler> _OutputDataReceived_EventTable = new();
        
        /// <inheritdoc cref="Process.OutputDataReceived"/>
        public event CoDataReceivedEventHandler OutputDataReceived
        {
            add
            {
                if (!_is_OutputDataReceived_EventRegistered)
                {
                    target.OutputDataReceived += (sender, e) => _OutputDataReceived_EventTable.InvocationList?.Invoke(sender, e);
                    _is_OutputDataReceived_EventRegistered = true;
                }
                void wrapper(object sender, DataReceivedEventArgs e) => value(this, new CoDataReceivedEventArgs { Data = e.Data });
                return _OutputDataReceived_EventTable.AddEventHandler(wrapper);
            }
            remove
            {
                _OutputDataReceived_EventTable.RemoveEventHandler(value);
            }
        }

        /// <summary>
        /// The singleton flag for the <see cref="Process.ErrorDataReceived"/> event registration.
        /// </summary>
        private bool _is_ErrorDataReceived_EventRegistered = false;

        /// <summary>
        /// The event registration token table for the <see cref="Process.ErrorDataReceived"/> event.
        /// </summary>
        private readonly System.Runtime.InteropServices.WindowsRuntime.EventRegistrationTokenTable<DataReceivedEventHandler> _ErrorDataReceived_EventTable = new();

        /// <inheritdoc cref="Process.ErrorDataReceived"/>
        public event CoDataReceivedEventHandler ErrorDataReceived
        {
            add
            {
                if (!_is_ErrorDataReceived_EventRegistered)
                {
                    target.ErrorDataReceived += (sender, e) => _ErrorDataReceived_EventTable.InvocationList?.Invoke(sender, e);
                    _is_ErrorDataReceived_EventRegistered = true;
                }
                void wrapper(object sender, DataReceivedEventArgs e) => value(this, new CoDataReceivedEventArgs { Data = e.Data });
                return _ErrorDataReceived_EventTable.AddEventHandler(wrapper);
            }
            remove
            {
                _ErrorDataReceived_EventTable.RemoveEventHandler(value);
            }
        }
#endif

        public partial void BeginErrorReadLine();
        public partial void BeginOutputReadLine();
        public partial void CancelErrorRead();
        public partial void CancelOutputRead();
        public partial void Close();
        public partial bool CloseMainWindow();
        public partial void Kill();
        public partial void Refresh();
        public partial bool Start();
        public partial void WaitForExit();
        public partial bool WaitForExit(int milliseconds);
        public partial bool WaitForInputIdle();
        public partial bool WaitForInputIdle(int milliseconds);

#if NET7_0_OR_GREATER
        public partial bool WaitForExit(TimeSpan timeout);
        public partial bool WaitForInputIdle(TimeSpan timeout);
#else
        /// <inheritdoc cref="ProcessEx.WaitForExit(Process, TimeSpan)"/>
        public bool WaitForExit(TimeSpan timeout) => target.WaitForExit(timeout);
        /// <inheritdoc cref="ProcessEx.WaitForInputIdle(Process, TimeSpan)"/>
        public bool WaitForInputIdle(TimeSpan timeout) => target.WaitForInputIdle(timeout);
#endif
    }

    /// <inheritdoc cref="Process"/>
    [ComVisible(true)]
    public sealed partial class ProcessStatic : IProcessStatic
    {
        /// <summary>
        /// Gets the singleton instance of the <see cref="ProcessStatic"/> class.
        /// </summary>
        public static ProcessStatic Instance { get; } = new();

        /// <inheritdoc cref="Process.GetProcesses()"/>
        public IProcess[] GetProcesses() => [.. Process.GetProcesses().Select(x => new RemoteProcess(x))];

        /// <inheritdoc cref="Process.Start(ProcessStartInfo)"/>
        public IProcess? Start(IProcessStartInfo startInfo) =>
            Process.Start(RemoteProcessStartInfo.ConvertToManaged(startInfo)) is Process process
                ? new RemoteProcess(process) : null;
    }
}
