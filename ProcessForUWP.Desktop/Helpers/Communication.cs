using Newtonsoft.Json;
using ProcessForUWP.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace ProcessForUWP.Desktop.Helpers
{
    /// <summary>
    /// Communication Helpers for RemoteProcess.
    /// </summary>
    public static class Communication
    {
        private static readonly object locker = new object();
        internal static List<RemoteProcess> Processes = new List<RemoteProcess>();
        internal static AppServiceConnection Connection;

        /// <summary>
        /// Initialize Communication.
        /// </summary>
        public static async void InitializeAppServiceConnection()
        {
            try
            {
                Connection = new AppServiceConnection
                {
                    AppServiceName = "ProcessForUWP.Delegate",
                    PackageFamilyName = Package.Current.Id.FamilyName
                };
                Connection.ServiceClosed += Connection_ServiceClosed;
                Connection.RequestReceived += Connection_RequestReceived;

                AppServiceConnectionStatus status = await Connection.OpenAsync();
                if (status != AppServiceConnectionStatus.Success)
                {
                    // something went wrong ...
                    Console.WriteLine(status.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        internal static async void SendMessage(object value)
        {
            string json = JsonConvert.SerializeObject(value);
            try
            {
                ValueSet message = new ValueSet() { { $"Desktop", json } };
                _ = await Connection.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Debug.WriteLine(json);
            }
        }

        internal static void SendMessages(MessageType typeEnum)
        {
            lock (locker)
            {
                SendMessage(Message.MakeMessage(typeEnum));
            }
        }

        internal static void SendMessages(MessageType typeEnum, int id)
        {
            lock (locker)
            {
                SendMessage(Message.MakeMessage(typeEnum, id));
            }
        }

        internal static void SendMessages(MessageType typeEnum, object message)
        {
            lock (locker)
            {
                SendMessage(Message.MakeMessage(typeEnum, 0, message));
            }
        }

        internal static void SendMessages(MessageType typeEnum, int id, object message)
        {
            lock (locker)
            {
                SendMessage(Message.MakeMessage(typeEnum, id, message));
            }
        }

        private static void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            try
            {
                Message msg = JsonConvert.DeserializeObject<Message>(args.Request.Message["UWP"] as string);
                switch (msg.MessageType)
                {
                    case MessageType.NewProcess:
                        Processes.Add(new RemoteProcess(msg.ID));
                        SendMessages(MessageType.Message, msg.ID, StatuesType.Success);
                        break;
                    case MessageType.CopyFile:
                        try
                        {
                            (string sourceFileName, string destFileName, bool overwrite) = msg.GetPackage<(string, string, bool)>();
                            File.Copy(sourceFileName, destFileName, overwrite);
                            SendMessages(MessageType.CopyFile, msg.ID, StatuesType.Success);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Kill all processes which have created.
        /// </summary>
        public static void KillAllProcesses()
        {
            foreach (RemoteProcess Process in Processes)
            {
                if (!Process.Process.HasExited)
                {
                    Process.Process.Kill();
                }
            }
        }

        private static void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args) => Environment.Exit(0);
    }
}
