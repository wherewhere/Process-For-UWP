using Newtonsoft.Json;
using ProcessForUWP.Core.Helpers;
using ProcessForUWP.Core.Models;
using ProcessForUWP.Desktop.Helpers;
using System;
using System.Diagnostics;
using Windows.ApplicationModel.AppService;

namespace ProcessForUWP.Desktop
{
    public class RemoteProcess : IDisposable
    {
        private readonly Process Process = new Process();
        private int CommunicationID = 0;
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
                Message msg = JsonConvert.DeserializeObject<Message>(args.Request.Message["UWP"] as string);
                if (msg.ID == CommunicationID)
                {
                    switch (msg.MessageType)
                    {
                        case MessageType.Kill:
                            Process.Kill();
                            break;
                        case MessageType.Start:
                            ProcessStartInfo info = msg.GetPackage<StartInfo>().GetStartInfo();
                            Process.StartInfo = info;
                            Process.Start();
                            break;
                        case MessageType.Close:
                            Process.Close();
                            break;
                        case MessageType.Refresh:
                            Process.Refresh();
                            break;
                        case MessageType.Dispose:
                            Process.Dispose();
                            break;
                        case MessageType.PropertyGet:
                            try
                            {
                                object value = Process.GetProperty(msg.GetPackage<string>());
                                Communication.SendMessages(MessageType.PropertySet, msg.ID, value);
                            }
                            catch (Exception ex)
                            {
                                Debug.Write(ex);
                            }
                            break;
                        case MessageType.PropertySet:
                            try
                            {
                                (string name, object value) = msg.GetPackage<(string, object)>();
                                Process.SetProperty(name, value);
                            }
                            catch (Exception ex)
                            {
                                Debug.Write(ex);
                            }
                            break;
                        case MessageType.BeginErrorReadLine:
                            Process.BeginErrorReadLine();
                            break;
                        case MessageType.BeginOutputReadLine:
                            Process.BeginOutputReadLine();
                            break;
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
            Communication.SendMessages(MessageType.ErrorData, CommunicationID, e.Data);
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Communication.SendMessages(MessageType.OutputData, CommunicationID, e.Data);
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Process.OutputDataReceived -= Process_OutputDataReceived;
            Process.ErrorDataReceived -= Process_ErrorDataReceived;
            Communication.SendMessages(MessageType.Exited);
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
