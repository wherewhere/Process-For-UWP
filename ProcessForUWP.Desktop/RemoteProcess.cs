using Newtonsoft.Json;
using ProcessForUWP.Core.Helpers;
using ProcessForUWP.Core.Models;
using ProcessForUWP.Desktop.Helpers;
using System;
using System.Diagnostics;
using Windows.ApplicationModel.AppService;

namespace ProcessForUWP.Desktop
{
    internal class RemoteProcess : IDisposable
    {
        internal readonly Process Process = new();
        private readonly int CommunicationID = 0;
        private bool disposedValue;

        public int Id => Process.Id;
        public bool HasExited => Process.HasExited;

        // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        public RemoteProcess(int ID)
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
            CommunicationID = ID;
            Process.Exited += Process_Exited;
            Process.ErrorDataReceived += Process_ErrorDataReceived;
            Process.OutputDataReceived += Process_OutputDataReceived;
            Communication.Connection.RequestReceived += Connection_RequestReceived;
        }

        private void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            try
            {
                if (args.Request.Message.ContainsKey(nameof(RemoteProcess)))
                {
                    Message message = JsonConvert.DeserializeObject<Message>(args.Request.Message[nameof(RemoteProcess)] as string);
                    if (message.ID == CommunicationID)
                    {
                        switch (message.MessageType)
                        {
                            case MessageType.Method:
                                Process.InvokeMethod(message.GetPackage<string>());
                                break;
                            case MessageType.PropertyGet:
                                try
                                {
                                    object value = Process.GetProperty(message.GetPackage<string>());
                                    Communication.SendMessages(nameof(RemoteProcess), MessageType.PropertySet, message.ID, value);
                                }
                                catch (Exception ex)
                                {
                                    Debug.Write(ex);
                                }
                                break;
                            case MessageType.PropertySet:
                                try
                                {
                                    (string name, object value) = message.GetPackage<(string, object)>();
                                    Process.SetProperty(name, value);
                                }
                                catch (Exception ex)
                                {
                                    Debug.Write(ex);
                                }
                                break;
                            case MessageType.ProcessStart:
                                ProcessStartInfo info = message.GetPackage<StartInfo>().GetStartInfo();
                                Process.StartInfo = info;
                                Process.Start();
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Communication.SendMessages("ProcessEx", MessageType.ProcessErrorData, CommunicationID, e.Data);
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Communication.SendMessages("ProcessEx", MessageType.ProcessOutputData, CommunicationID, e.Data);
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Process.OutputDataReceived -= Process_OutputDataReceived;
            Process.ErrorDataReceived -= Process_ErrorDataReceived;
            Communication.SendMessages("ProcessEx", MessageType.ProcessExited);
            Process.Exited -= Process_Exited;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Process.OutputDataReceived -= Process_OutputDataReceived;
                    Process.ErrorDataReceived -= Process_ErrorDataReceived;
                    Process.Exited -= Process_Exited;
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
