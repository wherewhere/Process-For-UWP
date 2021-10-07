using ProcessForUWP.Desktop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

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
        private TCPClient DataClient;
        private TCPClient ErrorClint;
        private TCPClient ControlClient;

        public new EventHandler Exited;
        public new DataReceivedEventHandler ErrorDataReceived;
        public new DataReceivedEventHandler OutputDataReceived;

        public new BinaryReader StandardError;
        public new BinaryWriter StandardInput;
        public new BinaryReader StandardOutput;

        public new int WorkingSet64 => (int)PropertyGet("WorkingSet64");
        public new string ProcessName => (string)PropertyGet("ProcessName");

        public new int BasePriority
        {
            get => BasePriority;
            set => PropertySet("BasePriority", value);
        }

        public new bool HasExited
        {
            get => (bool)PropertyGet("HasExited");
            set => PropertySet("HasExited", value);
        }

        private object PropertyGet(string Name)
        {
            ControlClient.SendByte((byte)ControlType.PropertyGet);
            ControlClient.Send(Name);
            return ControlClient.Receive(GetType().GetProperty(Name).GetType());
        }

        private void PropertySet(string Name, object value)
        {
            ControlClient.SendByte((byte)ControlType.PropertySet);
            ControlClient.Send(Name);
            ControlClient.Send(value);
        }

        public Process(ushort port = 32768)
        {
            _ = FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync("环回配置");
            DataClient = new TCPClient(port);
            ErrorClint = new TCPClient(port);
            ControlClient = new TCPClient(port);
            StandardInput = DataClient.BinaryWriter;
            StandardOutput = DataClient.BinaryReader;
            StandardError = ErrorClint.BinaryReader;
            DataClient.StringReceived += DataClient_MessageReceived;
            ErrorClint.StringReceived += ErrorClint_MessageReceived;
            ControlClient.ByteReceived += ControlClient_ByteReceived;
        }

        public new void Start()
        {
            ControlClient.SendByte((byte)ControlType.Start);
            ControlClient.Send(new bool[] { StartInfo.CreateNoWindow, StartInfo.RedirectStandardError, StartInfo.RedirectStandardInput, StartInfo.RedirectStandardOutput, StartInfo.UseShellExecute });
            ControlClient.Send(StartInfo.Arguments);
            ControlClient.Send(StartInfo.FileName);
            ControlClient.ListenByte();
        }

        public new void BeginErrorReadLine()
        {
            ControlClient.SendByte((byte)ControlType.BeginErrorReadLine);
            ControlClient.ListenString();
        }

        public new void BeginOutputReadLine()
        {
            ControlClient.SendByte((byte)ControlType.BeginOutputReadLine);
            ControlClient.ListenString();
        }

        public new void Close()
        {
            ControlClient.SendByte((byte)ControlType.Close);
        }

        public new void Dispose()
        {
            ControlClient.SendByte((byte)ControlType.Dispose);
            base.Dispose();
        }

        public new void Kill()
        {
            ControlClient.SendByte((byte)ControlType.Kill);
        }

        private void DataClient_MessageReceived(TCPClient sender,string message)
        {
            OutputDataReceived(this, new DataReceivedEventArgs(message));
        }

        private void ErrorClint_MessageReceived(TCPClient sender, string message)
        {
            ErrorDataReceived(this, new DataReceivedEventArgs(message));
        }

        private void ControlClient_ByteReceived(TCPClient sender, Byte message)
        {
            switch(message)
            {
                case (byte)ControlType.Exited:
                    Exited(this, new EventArgs());
                    break;
            }
        }
    }
}
