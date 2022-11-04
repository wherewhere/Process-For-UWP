using Newtonsoft.Json;
using ProcessForUWP.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace ProcessForUWP.Desktop.Helpers
{
    /// <summary>
    /// Communication Helpers for ProcessEx.
    /// </summary>
    public static class Communication
    {
        private static readonly object locker = new();
        internal static List<ProcessEx> Processes = new();
        internal static AppServiceConnection Connection;
        internal static event TypedEventHandler<AppServiceConnection, AppServiceRequestReceivedEventArgs> RequestReceived;

        static Communication()
        {
            FileEx.Initialize();
        }

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
                    Debug.WriteLine(status.ToString());
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        internal static async void SendMessage(string key, object value)
        {
            string json = JsonConvert.SerializeObject(value);
            try
            {
                if (IsInitialized())
                {
                    ValueSet message = new() { { key, json } };
                    _ = await Connection?.SendMessageAsync(message);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Debug.WriteLine(json);
            }
        }

        internal static void SendMessages(string key, MessageType typeEnum)
        {
            lock (locker)
            {
                SendMessage(key, Message.MakeMessage(typeEnum));
            }
        }

        internal static void SendMessages(string key, MessageType typeEnum, int id)
        {
            lock (locker)
            {
                SendMessage(key, Message.MakeMessage(typeEnum, id));
            }
        }

        internal static void SendMessages(string key, MessageType typeEnum, object message)
        {
            lock (locker)
            {
                SendMessage(key, Message.MakeMessage(typeEnum, 0, message));
            }
        }

        internal static void SendMessages(string key, MessageType typeEnum, int id, object message)
        {
            lock (locker)
            {
                SendMessage(key, Message.MakeMessage(typeEnum, id, message));
            }
        }

        private static void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            RequestReceived?.Invoke(sender, args);
            try
            {
                if (args.Request.Message.ContainsKey(nameof(Communication)))
                {
                    Message message = JsonConvert.DeserializeObject<Message>(args.Request.Message[nameof(Communication)] as string);
                    switch (message.MessageType)
                    {
                        case MessageType.NewProcess:
                            Processes.Add(new ProcessEx(message.ID));
                            SendMessages(nameof(Communication), MessageType.Message, message.ID, StatuesType.Success);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        internal static bool IsInitialized(double s = 10)
        {
            if (Connection != null)
            {
                return true;
            }
            else
            {
                CancellationTokenSource cancellationToken = new(TimeSpan.FromSeconds(s));
                try
                {
                    Debug.WriteLine($"Warning!\nYou may initialize a process when the app is not Initialized.\nDo not do this or app will crash after {s}s if app still not Initialized.");
                    while (SendMessage == null)
                    {
                        cancellationToken.Token.ThrowIfCancellationRequested();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// Kill all processes which have created.
        /// </summary>
        public static void KillAllProcesses()
        {
            foreach (ProcessEx Process in Processes)
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
