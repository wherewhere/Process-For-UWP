using Newtonsoft.Json;
using ProcessForUWP.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace ProcessForUWP.Desktop
{
    public class RemoteProcess : IDisposable
    {
        private readonly Process Process = new Process();
        private bool disposedValue;

        public int Id => Process.Id;
        public bool HasExited => Process.HasExited;

        // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        public RemoteProcess()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
            Process.Exited += Process_Exited;
            Process.ErrorDataReceived += Process_ErrorDataReceived;
            Process.OutputDataReceived += Process_OutputDataReceived;
            Communication.Connection.RequestReceived += Connection_RequestReceived;
        }

        private void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            try
            {
                Message msg = JsonConvert.DeserializeObject<Message>(args.Request.Message["1"] as string);
                switch (msg.ControlType)
                {
                    case ControlType.Kill:
                        Process.Kill();
                        break;
                    case ControlType.Start:
                        ProcessStartInfo info = msg.GetPackage<StartInfo>().GetStartInfo();
                        Process.StartInfo = info;
                        Process.Start();
                        break;
                    case ControlType.Close:
                        Process.Close();
                        break;
                    case ControlType.Refresh:
                        Process.Refresh();
                        break;
                    case ControlType.Dispose:
                        Process.Dispose();
                        break;
                    case ControlType.BeginErrorReadLine:
                        Process.BeginErrorReadLine();
                        break;
                    case ControlType.BeginOutputReadLine:
                        Process.BeginOutputReadLine();
                        break;
                    case ControlType.PropertyGet:
                        break;
                }
            }
            catch
            {
                
            }
        }

        private void Process_ErrorDataReceived(object sender,DataReceivedEventArgs e)
        {
            Communication.SendMessage(e.Data);
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Communication.SendMessage(e.Data);
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Process.OutputDataReceived -= Process_OutputDataReceived;
            Process.ErrorDataReceived -= Process_ErrorDataReceived;
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
