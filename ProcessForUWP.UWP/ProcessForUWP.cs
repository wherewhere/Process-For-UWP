using Newtonsoft.Json;
using ProcessForUWP.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;

namespace ProcessForUWP.UWP
{
    public static partial class ProcessHelper
    {
        public static Action<object> SendObject;
        public static (bool IsReceived, Message Received) Received;

        public static void SendMessages(ControlType typeEnum)
        {
            SendObject(Message.MakeMessage(typeEnum, 0));
        }

        public static void SendMessages(ControlType typeEnum, object message)
        {
            SendObject(Message.MakeMessage(typeEnum, 0, message));
        }

        public static void SendMessages(ControlType typeEnum, int id, object message)
        {
            SendObject(Message.MakeMessage(typeEnum, id, message));
        }

        public static void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            try
            {
                if (!Received.IsReceived)
                {
                    Message msg = JsonConvert.DeserializeObject<Message>(args.Request.Message["1"] as string);
                    Received = (true, msg);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public static (bool IsReceive, Message Received) Receive()
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            try
            {
                while (!Received.IsReceived)
                {
                    cancellationToken.Token.ThrowIfCancellationRequested();
                }
                return (true, Received.Received);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return (false, null);
            }
        }
    }

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

        public new int Id
        {
            get => (int)PropertyGet("Id");
        }

        //public new int BasePriority
        //{
        //    get => BasePriority;
        //    set => PropertySet("BasePriority", value);
        //}

        public new bool HasExited
        {
            get => (bool)PropertyGet("HasExited");
        }

        private object PropertyGet(string Name)
        {
            ProcessHelper.Received.IsReceived = false;
            ProcessHelper.SendMessages(ControlType.PropertyGet, 0, Name);
            (bool IsReceive, Message Received) = ProcessHelper.Receive();
            if (IsReceive)
            {
                return Received.GetPackage<object>();
            }
            else
            {
                throw new Exception("Process Unresponsive.");
            }
        }

        //private void PropertySet(string Name, object value)
        //{
        //    ProcessHelper.SendMessage((byte)ControlType.PropertySet);
        //    ProcessHelper.SendMessage(Name);
        //    ProcessHelper.SendMessage(value);
        //}

        public Process()
        {
            ProcessHelper.Received.IsReceived = false;
            ProcessHelper.SendMessages(ControlType.NewProcess, 0);
            if (!ProcessHelper.Receive().IsReceive)
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
            //ProcessHelper.SendMessages(ControlType.Close);
        }

        public new void Dispose()
        {
            //ProcessHelper.SendMessages(ControlType.Dispose);
            base.Dispose();
        }

        public new void Kill()
        {
            //ProcessHelper.SendMessages(ControlType.Kill);
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
