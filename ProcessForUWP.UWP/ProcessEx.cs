using Newtonsoft.Json;
using ProcessForUWP.Core.Models;
using ProcessForUWP.UWP.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Windows.ApplicationModel.AppService;

namespace ProcessForUWP.UWP
{
    /// <summary>
    /// Provides data for the <see cref="ProcessEx.OutputDataReceived"/> and <see cref="ProcessEx.ErrorDataReceived"/> events.
    /// </summary>
    public class DataReceivedEventArgsEx : EventArgs
    {
        /// <summary>
        /// Gets the line of characters that was written to a redirected <see cref="ProcessEx"/> output stream.
        /// </summary>
        public string Data;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataReceivedEventArgsEx"/> class.
        /// </summary>
        /// <param name="data"></param>
        public DataReceivedEventArgsEx(string data)
        {
            Data = data;
        }
    }

    /// <summary>
    /// Represents the method that will handle the <see cref="ProcessEx.OutputDataReceived"/> event or <see cref="ProcessEx.ErrorDataReceived"/> event of a ProcessForUWP.UWP.ProcessEx.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="DataReceivedEventArgsEx"/> that contains the event data.</param>
    public delegate void DataReceivedEventHandlerEx(ProcessEx sender, DataReceivedEventArgsEx e);

    /// <summary>
    /// Provides access to local and remote processes and enables you to start and stop local system processes.
    /// </summary>
    public class ProcessEx : Process
    {
        private readonly int CommunicationID = Communication.GetID;

        /// <summary>
        /// Gets a stream used to read the error output of the application.
        /// </summary>
        public new StreamReader StandardError;

        /// <summary>
        /// Gets a stream used to write the input of the application.
        /// </summary>
        public new StreamWriter StandardInput;

        /// <summary>
        /// Gets a stream used to read the textual output of the application.
        /// </summary>
        public new StreamReader StandardOutput;

        private StreamWriter ErrorStreamWriter;
        private StreamWriter OutputStreamWriter;

        private readonly MemoryStream ErrorStream = new MemoryStream();
        private readonly MemoryStream OutputStream = new MemoryStream();

        /// <summary>
        /// Occurs when a process exits.
        /// </summary>
        public new event EventHandler Exited;

        /// <summary>
        /// Occurs when an application writes to its redirected System.Diagnostics.ProcessEx.StandardError stream.
        /// </summary>
        public new event DataReceivedEventHandlerEx ErrorDataReceived;

        /// <summary>
        /// Occurs each time an application writes a line to its redirected System.Diagnostics.ProcessEx.StandardOutput stream.
        /// </summary>
        public new event DataReceivedEventHandlerEx OutputDataReceived;

        /// <summary>
        /// Gets the unique identifier for the associated process.
        /// </summary>
        public new long Id => (long)PropertyGet("Id");

        /// <summary>
        /// Gets the Terminal Services session identifier for the associated process.
        /// </summary>
        public new long SessionId => (long)PropertyGet("SessionId");

        /// <summary>
        /// Gets a value indicating whether the associated process has been terminated.
        /// </summary>
        public new bool HasExited => (bool)PropertyGet("HasExited");

        /// <summary>
        /// Gets the amount of physical memory, in bytes, allocated for the associated process.
        /// </summary>
        public new int WorkingSet64 => (int)PropertyGet("WorkingSet64");

        /// <summary>
        /// Gets the name of the process.
        /// </summary>
        public new string ProcessName => (string)PropertyGet("ProcessName");

        /// <summary>
        /// Gets a value indicating whether the associated process has been terminated.
        /// </summary>
        public bool IsExited;

        private int basePriority;
        /// <summary>
        /// Gets the base priority of the associated process.
        /// </summary>
        public new int BasePriority
        {
            get => basePriority;
            set
            {
                if (basePriority != value)
                {
                    PropertySet("BasePriority", value);
                    basePriority = value;
                }
            }
        }

        private object PropertyGet(string Name)
        {
            (bool IsReceive, Message Received) = Communication.GetMessages("RemoteProcess", MessageType.PropertyGet, CommunicationID, Name, MessageType.PropertySet);
            if (IsReceive)
            {
                return Received.GetPackage<object>();
            }
            else
            {
                throw new ArithmeticException("ProcessEx Unresponsive.");
            }
        }

        private void PropertySet(string Name, object value)
        {
            Communication.Received.IsReceived = false;
            Communication.SendMessages("RemoteProcess", MessageType.PropertySet, CommunicationID, (Name, value));
        }

        /// <summary>
        /// Initializes a new instance of the ProcessForUWP.UWP.ProcessEx class.
        /// </summary>
        /// <param name="s">The amount of time, in seconds, to wait for instance process.</param>
        /// <exception cref="InvalidOperationException">Initializes process failed.</exception>
        public ProcessEx(double s = 10)
        {
            if (!Communication.IsInitialized(s)) { throw new InvalidOperationException("Have not initialized process yet."); }
            Communication.RequestReceived += Connection_RequestReceived;
            (bool IsReceive, Message Received) = Communication.GetMessages(nameof(Communication), MessageType.NewProcess, CommunicationID, MessageType.Message);
            if (!IsReceive || Received?.GetPackage<StatuesType>() != StatuesType.Success)
            {
                throw new InvalidOperationException("Cannot initializes process.");
            }
        }

        /// <summary>
        /// Starts (or reuses) the process resource that is specified by the ProcessForUWP.UWP.ProcessEx.StartInfo property of this ProcessForUWP.UWP.ProcessEx component and associates it with the component.
        /// </summary>
        public new void Start()
        {
            IsExited = false;
            if (StartInfo.RedirectStandardError)
            {
                StandardError = new StreamReader(ErrorStream);
                ErrorStreamWriter = new StreamWriter(ErrorStream);
            }
            if (StartInfo.RedirectStandardOutput)
            {
                StandardOutput = new StreamReader(OutputStream);
                OutputStreamWriter = new StreamWriter(OutputStream);
            }
            Communication.SendMessages("RemoteProcess", MessageType.ProcessStart, CommunicationID, new StartInfo(StartInfo));
        }

        /// <summary>
        /// Starts the process resource that is specified by the parameter containing process start information (for example, the file name of the process to start) and associates the resource with a new ProcessForUWP.UWP.ProcessEx component.
        /// </summary>
        /// <param name="info">The System.Diagnostics.ProcessStartInfo that contains the information that is used to start the process, including the file name and any command-line arguments.</param>
        /// <returns>A new ProcessForUWP.UWP.ProcessEx that is associated with the process resource, or null if no process resource is started. Note that a new process that’s started alongside already running instances of the same process will be independent from the others. In addition, ProcessStart may return a non-null ProcessEx with its ProcessForUWP.UWP.ProcessEx.HasExited property already set to true. In this case, the started process may have activated an existing instance of itself and then exited.</returns>
        public static new ProcessEx Start(ProcessStartInfo info)
        {
            ProcessEx process = new ProcessEx() { StartInfo = info };
            process.Start();
            return process;
        }

        /// <summary>
        /// Discards any information about the associated process that has been cached inside the process component.
        /// </summary>
        public new void Refresh()
        {
            Communication.SendMessages("RemoteProcess", MessageType.Method, CommunicationID, nameof(Refresh));
        }

        /// <summary>
        /// Begins asynchronous read operations on the redirected ProcessForUWP.UWP.ProcessEx.StandardError stream of the application.
        /// </summary>
        public new void BeginErrorReadLine()
        {
            Communication.SendMessages("RemoteProcess", MessageType.Method, CommunicationID, nameof(BeginErrorReadLine));
        }

        /// <summary>
        /// Begins asynchronous read operations on the redirected ProcessForUWP.UWP.ProcessEx.StandardOutput stream of the application.
        /// </summary>
        public new void BeginOutputReadLine()
        {
            Communication.SendMessages("RemoteProcess", MessageType.Method, CommunicationID, nameof(BeginOutputReadLine));
        }

        /// <summary>
        /// Frees all the resources that are associated with this component.
        /// </summary>
        public new void Close()
        {
            Communication.SendMessages("RemoteProcess", MessageType.Method, CommunicationID, nameof(Close));
            Communication.RequestReceived -= Connection_RequestReceived;
            OutputStreamWriter?.Dispose();
            ErrorStreamWriter?.Dispose();
            StandardOutput?.Dispose();
            StandardInput?.Dispose();
            StandardError?.Dispose();
            OutputStream?.Dispose();
            ErrorStream?.Dispose();
            IsExited = true;
        }

        /// <summary>
        /// Release all resources used by this process.
        /// </summary>
        public new void Dispose()
        {
            Communication.SendMessages("RemoteProcess", MessageType.Method, CommunicationID, nameof(Dispose));
            Communication.RequestReceived -= Connection_RequestReceived;
            OutputStreamWriter?.Dispose();
            ErrorStreamWriter?.Dispose();
            StandardOutput?.Dispose();
            StandardInput?.Dispose();
            StandardError?.Dispose();
            OutputStream?.Dispose();
            ErrorStream?.Dispose();
            IsExited = true;
            base.Dispose();
        }

        /// <summary>
        /// Immediately stops the associated process.
        /// </summary>
        public new void Kill()
        {
            Communication.SendMessages("RemoteProcess", MessageType.Method, CommunicationID, nameof(Kill));
            Communication.RequestReceived -= Connection_RequestReceived;
            OutputStreamWriter?.Dispose();
            ErrorStreamWriter?.Dispose();
            StandardOutput?.Dispose();
            StandardInput?.Dispose();
            StandardError?.Dispose();
            OutputStream?.Dispose();
            ErrorStream?.Dispose();
            IsExited = true;
        }

        private void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            try
            {
                if (args.Request.Message.ContainsKey(nameof(ProcessEx)))
                {
                    Message message = JsonConvert.DeserializeObject<Message>(args.Request.Message[nameof(ProcessEx)] as string);
                    if (message.ID == CommunicationID)
                    {
                        if (message.MessageType == MessageType.ProcessExited)
                        {
                            IsExited = true;
                            Exited?.Invoke(this, new EventArgs());
                        }
                        else if (message.MessageType == MessageType.ProcessErrorData)
                        {
                            string line = message.GetPackage<string>();
                            ErrorDataReceived?.Invoke(this, new DataReceivedEventArgsEx(line));
                            if (StartInfo.RedirectStandardError)
                            {
                                ErrorStreamWriter.WriteLine(line);
                                ErrorStreamWriter.Flush();
                            }
                        }
                        else if (message.MessageType == MessageType.ProcessOutputData)
                        {
                            string line = message.GetPackage<string>();
                            OutputDataReceived?.Invoke(this, new DataReceivedEventArgsEx(line));
                            if (StartInfo.RedirectStandardOutput)
                            {
                                OutputStreamWriter.WriteLine(line);
                                OutputStreamWriter.Flush();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Instructs the System.Diagnostics.ProcessEx component to wait indefinitely for the associated process to exit.
        /// </summary>
        public new void WaitForExit()
        {
            while (!IsExited)
            {
                ;
            }
        }

        /// <summary>
        /// Instructs the System.Diagnostics.ProcessEx component to wait the specified number of milliseconds for the associated process to exit.
        /// </summary>
        /// <param name="milliseconds">The amount of time, in milliseconds, to wait for the associated process to exit.
        /// The maximum is the largest possible value of a 32-bit integer, which represents infinity to the operating system.</param>
        /// <returns>true if the associated process has exited; otherwise, false.</returns>
        public new bool WaitForExit(int milliseconds)
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(milliseconds));
            try
            {
                while (!IsExited)
                {
                    cancellationToken.Token.ThrowIfCancellationRequested();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
