using Newtonsoft.Json;
using ProcessForUWP.Core.Models;
using ProcessForUWP.UWP.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
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
        public new StreamReader StandardError;
        public new StreamWriter StandardInput;
        public new StreamReader StandardOutput;

        private StreamWriter ErrorStreamWriter;
        private StreamWriter OutputStreamWriter;

        private MemoryStream ErrorStream = new MemoryStream();
        private MemoryStream OutputStream = new MemoryStream();

        public new event EventHandler Exited;
        public new event DataReceivedEventHandler ErrorDataReceived;
        public new event DataReceivedEventHandler OutputDataReceived;

        public new long Id => (long)PropertyGet("Id");
        public new long SessionId => (long)PropertyGet("SessionId");
        public new bool HasExited => (bool)PropertyGet("HasExited");
        public new int WorkingSet64 => (int)PropertyGet("WorkingSet64");
        public new string ProcessName => (string)PropertyGet("ProcessName");

        public new int BasePriority
        {
            get => BasePriority;
            set => PropertySet("BasePriority", value);
        }

        private object PropertyGet(string Name)
        {
            (bool IsReceive, Message Received) = ProcessHelper.GetMessages(MessageType.PropertyGet, 0, Name, MessageType.PropertySet);
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
            ProcessHelper.SendMessages(MessageType.PropertySet, 0, (Name, value));
        }

        public Process()
        {
            ProcessHelper.RequestReceived += Connection_RequestReceived;
            (bool IsReceive, Message Received) = ProcessHelper.GetMessages(MessageType.NewProcess, 0, MessageType.Message);
            if (!IsReceive && Received.GetPackage<StatuesType>() == StatuesType.Success)
            {
                throw new InvalidOperationException("Cannot initializes process.");
            }
        }

        public new void Start()
        {
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
            ProcessHelper.SendMessages(MessageType.Start, 0, new StartInfo(StartInfo));
        }

        public new void Start(ProcessStartInfo info)
        {
            StartInfo = info;
            Start();
        }

        public new void BeginErrorReadLine()
        {
            ProcessHelper.SendMessages(MessageType.BeginErrorReadLine);
        }

        public new void BeginOutputReadLine()
        {
            ProcessHelper.SendMessages(MessageType.BeginOutputReadLine);
        }

        public new void Close()
        {
            ProcessHelper.SendMessages(MessageType.Close);
        }

        public new void Dispose()
        {
            ProcessHelper.RequestReceived -= Connection_RequestReceived;
            ProcessHelper.SendMessages(MessageType.Dispose);
            OutputStreamWriter?.Dispose();
            ErrorStreamWriter?.Dispose();
            StandardOutput?.Dispose();
            StandardInput?.Dispose();
            StandardError?.Dispose();
            OutputStream?.Dispose();
            ErrorStream?.Dispose();
            base.Dispose();
        }

        public new void Kill()
        {
            ProcessHelper.SendMessages(MessageType.Kill);
        }

        public void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            try
            {
                Message msg = JsonConvert.DeserializeObject<Message>(args.Request.Message["Desktop"] as string);
                if (msg.MessageType == MessageType.Exited)
                {
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        //private void DataClient_MessageReceived(TCPClient sender,string message)
        //{
        //    OutputDataReceived(this, new DataReceivedEventArgs(message));
        //}

        //private void ErrorClint_MessageReceived(TCPClient sender, string message)
        //{
        //    ErrorDataReceived(this, new DataReceivedEventArgs(message));
        //}

        //private void ControlClient_ByteReceived(TCPClient sender, Byte message)
        //{
        //    switch(message)
        //    {
        //        case (byte)ControlType.Exited:
        //            Exited(this, new EventArgs());
        //            break;
        //    }
        //}
    }
}
