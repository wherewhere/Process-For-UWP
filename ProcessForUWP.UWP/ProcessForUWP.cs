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
    /// Provides data for the ProcessForUWP.UWP.Process.OutputDataReceived and ProcessForUWP.UWP.Process.ErrorDataReceived events.
    /// </summary>
    public class DataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the line of characters that was written to a redirected ProcessForUWP.UWP.Process output stream.
        /// </summary>
        public string Data;

        /// <summary>
        /// Initializes a new instance of the ProcessForUWP.UWP.DataReceivedEventArgs class.
        /// </summary>
        /// <param name="data"></param>
        public DataReceivedEventArgs(string data)
        {
            Data = data;
        }
    }

    /// <summary>
    /// Represents the method that will handle the ProcessForUWP.UWP.Process.OutputDataReceived event or ProcessForUWP.UWP.Process.ErrorDataReceived event of a ProcessForUWP.UWP.Process.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A ProcessForUWP.UWP.DataReceivedEventArgs that contains the event data.</param>
    public delegate void DataReceivedEventHandler(Process sender, DataReceivedEventArgs e);

    /// <summary>
    /// Provides access to local and remote processes and enables you to start and stop local system processes.
    /// </summary>
    public class Process : System.Diagnostics.Process
    {
        private readonly int CommunicationID = ProcessHelper.GetID;

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
        /// Occurs when an application writes to its redirected System.Diagnostics.Process.StandardError stream.
        /// </summary>
        public new event DataReceivedEventHandler ErrorDataReceived;

        /// <summary>
        /// Occurs each time an application writes a line to its redirected System.Diagnostics.Process.StandardOutput stream.
        /// </summary>
        public new event DataReceivedEventHandler OutputDataReceived;

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

        /// <summary>
        /// Gets the base priority of the associated process.
        /// </summary>
        public new int BasePriority
        {
            get => BasePriority;
            set => PropertySet("BasePriority", value);
        }

        private object PropertyGet(string Name)
        {
            (bool IsReceive, Message Received) = ProcessHelper.GetMessages(MessageType.PropertyGet, CommunicationID, Name, MessageType.PropertySet);
            if (IsReceive)
            {
                return Received.GetPackage<object>();
            }
            else
            {
                throw new ArithmeticException("Process Unresponsive.");
            }
        }

        private void PropertySet(string Name, object value)
        {
            ProcessHelper.Received.IsReceived = false;
            ProcessHelper.SendMessages(MessageType.PropertySet, CommunicationID, (Name, value));
        }

        /// <summary>
        /// Initializes a new instance of the ProcessForUWP.UWP.Process class.
        /// </summary>
        /// <exception cref="InvalidOperationException">Initializes process failed.</exception>
        public Process()
        {
            if (!ProcessHelper.IsInitialized) { throw new InvalidOperationException("Have not initialized process yet."); }
            ProcessHelper.RequestReceived += Connection_RequestReceived;
            (bool IsReceive, Message Received) = ProcessHelper.GetMessages(MessageType.NewProcess, CommunicationID, MessageType.Message);
            if (!IsReceive || Received?.GetPackage<StatuesType>() != StatuesType.Success)
            {
                throw new InvalidOperationException("Cannot initializes process.");
            }
        }

        /// <summary>
        /// Starts (or reuses) the process resource that is specified by the ProcessForUWP.UWP.Process.StartInfo property of this ProcessForUWP.UWP.Process component and associates it with the component.
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
            ProcessHelper.SendMessages(MessageType.Start, CommunicationID, new StartInfo(StartInfo));
        }

        /// <summary>
        /// Starts the process resource that is specified by the parameter containing process start information (for example, the file name of the process to start) and associates the resource with a new ProcessForUWP.UWP.Process component.
        /// </summary>
        /// <param name="info">The System.Diagnostics.ProcessStartInfo that contains the information that is used to start the process, including the file name and any command-line arguments.</param>
        /// <returns>A new ProcessForUWP.UWP.Process that is associated with the process resource, or null if no process resource is started. Note that a new process that’s started alongside already running instances of the same process will be independent from the others. In addition, Start may return a non-null Process with its ProcessForUWP.UWP.Process.HasExited property already set to true. In this case, the started process may have activated an existing instance of itself and then exited.</returns>
        public static new Process Start(ProcessStartInfo info)
        {
            Process process = new Process() { StartInfo = info };
            process.Start();
            return process;
        }

        /// <summary>
        /// Discards any information about the associated process that has been cached inside the process component.
        /// </summary>
        public new void Refresh()
        {
            ProcessHelper.SendMessages(MessageType.Refresh, CommunicationID);
        }

        /// <summary>
        /// Begins asynchronous read operations on the redirected ProcessForUWP.UWP.Process.StandardError stream of the application.
        /// </summary>
        public new void BeginErrorReadLine()
        {
            ProcessHelper.SendMessages(MessageType.BeginErrorReadLine, CommunicationID);
        }

        /// <summary>
        /// Begins asynchronous read operations on the redirected ProcessForUWP.UWP.Process.StandardOutput stream of the application.
        /// </summary>
        public new void BeginOutputReadLine()
        {
            ProcessHelper.SendMessages(MessageType.BeginOutputReadLine, CommunicationID);
        }

        /// <summary>
        /// Frees all the resources that are associated with this component.
        /// </summary>
        public new void Close()
        {
            ProcessHelper.SendMessages(MessageType.Close, CommunicationID);
            ProcessHelper.RequestReceived -= Connection_RequestReceived;
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
            ProcessHelper.SendMessages(MessageType.Dispose, CommunicationID);
            ProcessHelper.RequestReceived -= Connection_RequestReceived;
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
            ProcessHelper.SendMessages(MessageType.Kill, CommunicationID);
            ProcessHelper.RequestReceived -= Connection_RequestReceived;
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
                Message msg = JsonConvert.DeserializeObject<Message>(args.Request.Message["Desktop"] as string);
                if (msg.ID == CommunicationID)
                {
                    if (msg.MessageType == MessageType.Exited)
                    {
                        IsExited = true;
                        Exited?.Invoke(this, new EventArgs());
                    }
                    else if (msg.MessageType == MessageType.ErrorData)
                    {
                        string line = msg.GetPackage<string>();
                        ErrorDataReceived?.Invoke(this, new DataReceivedEventArgs(line));
                        if (StartInfo.RedirectStandardError)
                        {
                            ErrorStreamWriter.WriteLine(line);
                            ErrorStreamWriter.Flush();
                        }
                    }
                    else if (msg.MessageType == MessageType.OutputData)
                    {
                        string line = msg.GetPackage<string>();
                        OutputDataReceived?.Invoke(this, new DataReceivedEventArgs(line));
                        if (StartInfo.RedirectStandardOutput)
                        {
                            OutputStreamWriter.WriteLine(line);
                            OutputStreamWriter.Flush();
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
        /// Instructs the System.Diagnostics.Process component to wait indefinitely for the associated process to exit.
        /// </summary>
        public new void WaitForExit()
        {
            while (!IsExited)
            {
                ;
            }
        }

        /// <summary>
        /// Instructs the System.Diagnostics.Process component to wait the specified number of milliseconds for the associated process to exit.
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
