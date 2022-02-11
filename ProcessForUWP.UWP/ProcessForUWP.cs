using ProcessForUWP.Core.Models;
using ProcessForUWP.UWP.Helpers;
using System;
using System.Diagnostics;

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
        //public new EventHandler Exited;
        //public new DataReceivedEventHandler ErrorDataReceived;
        //public new DataReceivedEventHandler OutputDataReceived;

        //public new int WorkingSet64 => (int)PropertyGet("WorkingSet64");
        //public new string ProcessName => (string)PropertyGet("ProcessName");

        public new long Id
        {
            get => (long)PropertyGet("Id");
        }

        public new long SessionId
        {
            get => (long)PropertyGet("SessionId");
        }

        public new int BasePriority
        {
            get => BasePriority;
            set => PropertySet("BasePriority", value);
        }

        public new bool HasExited
        {
            get => (bool)PropertyGet("HasExited");
        }

        private object PropertyGet(string Name)
        {
            ProcessHelper.Received.IsReceived = false;
            ProcessHelper.SendMessages(ControlType.PropertyGet, 0, Name);
            (bool IsReceive, Message Received) = ProcessHelper.Receive(ControlType.PropertySet);
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
            ProcessHelper.SendMessages(ControlType.PropertySet, 0, (Name, value));
        }

        public Process()
        {
            ProcessHelper.Received.IsReceived = false;
            ProcessHelper.SendMessages(ControlType.NewProcess, 0);
            (bool IsReceive, Message Received) = ProcessHelper.Receive(ControlType.Message);
            if (!IsReceive && Received.GetPackage<StatuesType>() == StatuesType.Success)
            {
                throw new InvalidOperationException("Cannot initializes process.");
            }
        }

        public new void Start()
        {
            ProcessHelper.SendMessages(ControlType.Start, 0, new StartInfo(StartInfo));
            //ControlClient.ListenByte();
        }

        public new void Start(ProcessStartInfo info)
        {
            StartInfo = info;
            ProcessHelper.SendMessages(ControlType.Start, 0, new StartInfo(StartInfo));
        }

        public new void BeginErrorReadLine()
        {
            ProcessHelper.SendMessages(ControlType.BeginErrorReadLine);
        }

        public new void BeginOutputReadLine()
        {
            ProcessHelper.SendMessages(ControlType.BeginOutputReadLine);
        }

        public new void Close()
        {
            ProcessHelper.SendMessages(ControlType.Close);
        }

        public new void Dispose()
        {
            ProcessHelper.SendMessages(ControlType.Dispose);
            base.Dispose();
        }

        public new void Kill()
        {
            ProcessHelper.SendMessages(ControlType.Kill);
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
