using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ProcessForUWP.Desktop
{
    public class RemoteProcess
    {
        private TCPClient DataClient;
        private TCPClient ErrorClint;
        private TCPClient ControlClient;
        private readonly Process Process = new Process();

        public void StartProcess(string Command)
        {
            Process.Exited += Process_Exited;
            Process.ErrorDataReceived += Process_ErrorDataReceived;
            Process.OutputDataReceived += Process_OutputDataReceived;
            string[] a = Command.Split(' ');
            TCPListener e = new TCPListener(ushort.Parse(a[3]));
            Process.Start(new ProcessStartInfo("CheckNetIsolation.exe", $"LoopbackExempt -a -n=\"{a[2]}") { Verb= "runas" });
            ControlType b;
            System.Reflection.PropertyInfo d;
            e.Start();
            ControlClient = e.AcceptTcpClient();
            DataClient = e.AcceptTcpClient();
            ErrorClint = e.AcceptTcpClient();
            e.Stop();
            bool exit = false;
            do
            {
                b = (ControlType)ControlClient.ReceiveByte();
                switch (b)
                {
                    case ControlType.Start:
                        bool[] c = ControlClient.ReceiveBooleans();
                        Process.StartInfo.Arguments = ControlClient.ReceiveString();
                        Process.StartInfo.CreateNoWindow = c[0];
                        Process.StartInfo.RedirectStandardError = c[1];
                        Process.StartInfo.RedirectStandardInput = c[2];
                        Process.StartInfo.RedirectStandardOutput = c[3];
                        Process.StartInfo.UseShellExecute = c[4];
                        Process.StartInfo.FileName = ControlClient.ReceiveString();
                        break;
                    case ControlType.BeginErrorReadLine:
                        Process.BeginErrorReadLine();
                        break;
                    case ControlType.BeginOutputReadLine:
                        Process.BeginOutputReadLine();
                        break;
                    case ControlType.PropertyGet:
                        d = Process.GetType().GetProperty(ControlClient.ReceiveString());
                        ControlClient.Send((object)d.GetValue(Process));
                        break;
                    case ControlType.PropertySet:
                        d = Process.GetType().GetProperty(ControlClient.ReceiveString());
                        d.SetValue(Process, ControlClient.Receive(d.GetType()));
                        break;
                    case ControlType.Close:
                        Process.Close();
                        break;
                    case ControlType.Dispose:
                        exit = true;
                        break;
                    case ControlType.Kill:
                        Process.Kill();
                        break;
                }
            }
            while (exit);
            Process.OutputDataReceived -= Process_OutputDataReceived;
            Process.ErrorDataReceived -= Process_ErrorDataReceived;
            Process.Exited -= Process_Exited;
        }

        private void Process_ErrorDataReceived(object sender,DataReceivedEventArgs e)
        {
            ErrorClint.Send(e.Data);
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            DataClient.Send(e.Data);
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            ControlClient.Send(ControlType.Exited);
        }
    }
}
