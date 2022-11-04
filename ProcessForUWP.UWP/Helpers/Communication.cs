using Newtonsoft.Json;
using ProcessForUWP.Core.Models;
using System;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;

namespace ProcessForUWP.UWP.Helpers
{
    /// <summary>
    /// Communication Helpers for ProcessEx.
    /// </summary>
    public static partial class Communication
    {
        internal static Application Application => Application.Current;

        private static int ID = 0;
        internal static int GetID => ID++;
        private static readonly object locker = new();

        internal static AppServiceConnection Connection;
        internal static BackgroundTaskDeferral AppServiceDeferral;
        internal static event EventHandler AppServiceDisconnected;
        internal static event EventHandler<AppServiceTriggerDetails> AppServiceConnected;
        internal static event TypedEventHandler<AppServiceConnection, AppServiceRequestReceivedEventArgs> RequestReceived;

        internal static (bool IsReceived, ValueSet Message) Received;

        static Communication()
        {
            AppServiceConnected += (sender, e) => Connection.RequestReceived += Connection_RequestReceived;
        }

        /// <summary>
        /// Initialize Communication.
        /// </summary>
        public static async void InitializeAppServiceConnection()
        {
            if (Connection == null)
            {
                if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0))
                {
                    try
                    {
                        await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Handles connection requests to the app service
        /// </summary>
        public static void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            if (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails details)
            {
                // only accept connections from callers in the same package
                if (details.CallerPackageFamilyName == Package.Current.Id.FamilyName)
                {
                    // connection established from the fulltrust process
                    AppServiceDeferral = args.TaskInstance.GetDeferral();
                    args.TaskInstance.Canceled += OnTaskCanceled;

                    Connection = details.AppServiceConnection;
                    AppServiceConnected?.Invoke(Application, args.TaskInstance.TriggerDetails as AppServiceTriggerDetails);
                }
            }
        }

        /// <summary>
        /// Task canceled here means the app service client is gone
        /// </summary>
        private static void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            AppServiceDeferral?.Complete();
            AppServiceDeferral = null;
            Connection = null;
            AppServiceDisconnected?.Invoke(Application, null);
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

        internal static (bool IsReceive, Message Received) GetMessages(string key, MessageType sendEnum, MessageType receiveEnum)
        {
            Received.IsReceived = false;
            SendMessages(key, sendEnum);
            return Receive(key, 0, receiveEnum);
        }

        internal static (bool IsReceive, Message Received) GetMessages(string key, MessageType sendEnum, int id, MessageType receiveEnum)
        {
            Received.IsReceived = false;
            SendMessages(key, sendEnum, id);
            return Receive(key, id, receiveEnum);
        }

        internal static (bool IsReceive, Message Received) GetMessages(string key, MessageType sendEnum, object message, MessageType receiveEnum)
        {
            Received.IsReceived = false;
            SendMessages(key, sendEnum, message);
            return Receive(key, 0, receiveEnum);
        }

        internal static (bool IsReceive, Message Received) GetMessages(string key, MessageType sendEnum, int id, object message, MessageType receiveEnum)
        {
            Received.IsReceived = false;
            SendMessages(key, sendEnum, id, message);
            return Receive(key, id, receiveEnum);
        }

        /// <summary>
        /// Receive Message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            try
            {
                if (!Received.IsReceived)
                {
                    ValueSet msg = args.Request.Message;
                    Received = (true, msg);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            RequestReceived?.Invoke(sender, args);
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

        internal static (bool IsReceive, Message Received) Receive(string key, int id, MessageType? type = null)
        {
            CancellationTokenSource cancellationToken = new(TimeSpan.FromSeconds(10));
            try
            {
                wait:
                while (!Received.IsReceived)
                {
                    cancellationToken.Token.ThrowIfCancellationRequested();
                }
                if (Received.Message != null && Received.Message.ContainsKey(key))
                {
                    if (Received.Message.TryGetValue(key, out object value))
                    {
                        Message message = JsonConvert.DeserializeObject<Message>(value.ToString());
                        if (message.ID == id && (type == null || message.MessageType == type))
                        {
                            return (true, message);
                        }
                    }
                }
                Received.IsReceived = false;
                goto wait;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return (false, null);
            }
            finally
            {
                Received.IsReceived = false;
            }
        }
    }
}
