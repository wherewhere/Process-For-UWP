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
    public class DataReceivedEventArgs : EventArgs
    {
        public string Data;
        public DataReceivedEventArgs(string data)
        {
            Data = data;
        }
    }

    public delegate void DataReceivedEventHandler(Process sender, DataReceivedEventArgs e);

    public class Process : System.Diagnostics.Process
    {
        private readonly int CommunicationID = ProcessHelper.GetID;

        public new StreamReader StandardError;
        public new StreamWriter StandardInput;
        public new StreamReader StandardOutput;

        private StreamWriter ErrorStreamWriter;
        private StreamWriter OutputStreamWriter;

        private readonly MemoryStream ErrorStream = new MemoryStream();
        private readonly MemoryStream OutputStream = new MemoryStream();

        public new event EventHandler Exited;
        public new event DataReceivedEventHandler ErrorDataReceived;
        public new event DataReceivedEventHandler OutputDataReceived;

        public new long Id => (long)PropertyGet("Id");
        public new long SessionId => (long)PropertyGet("SessionId");
        public new bool HasExited => (bool)PropertyGet("HasExited");
        public new int WorkingSet64 => (int)PropertyGet("WorkingSet64");
        public new string ProcessName => (string)PropertyGet("ProcessName");

        public bool IsExited;

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

        public Process()
        {
            if (!ProcessHelper.IsInitialized) { throw new InvalidOperationException("Have not initialized process yet."); }
            ProcessHelper.RequestReceived += Connection_RequestReceived;
            (bool IsReceive, Message Received) = ProcessHelper.GetMessages(MessageType.NewProcess, CommunicationID, MessageType.Message);
            if (!IsReceive && Received.GetPackage<StatuesType>() == StatuesType.Success)
            {
                throw new InvalidOperationException("Cannot initializes process.");
            }
        }

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

        public new void Start(ProcessStartInfo info)
        {
            StartInfo = info;
            Start();
        }

        public new void BeginErrorReadLine()
        {
            ProcessHelper.SendMessages(MessageType.BeginErrorReadLine, CommunicationID);
        }

        public new void BeginOutputReadLine()
        {
            ProcessHelper.SendMessages(MessageType.BeginOutputReadLine, CommunicationID);
        }

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

        public void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
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

        public new void WaitForExit()
        {
            while (!IsExited)
            {
                ;
            }
        }

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
