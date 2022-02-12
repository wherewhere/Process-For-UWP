using Newtonsoft.Json;
using ProcessForUWP.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace ProcessForUWP.Desktop.Helpers
{
    public static class Communication
    {
        public static List<RemoteProcess> Processes = new List<RemoteProcess>();
        public static AppServiceConnection Connection;

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

        public static async void SendMessage(object value)
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

        public static void SendMessages(MessageType typeEnum)
        {
            SendMessage(Message.MakeMessage(typeEnum));
        }

        public static void SendMessages(MessageType typeEnum, object message = null)
        {
            SendMessage(Message.MakeMessage(typeEnum, 0, message));
        }

        public static void SendMessages(MessageType typeEnum, int id, object message = null)
        {
            SendMessage(Message.MakeMessage(typeEnum, id, message));
        }

        private static void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            try
            {
                Message msg = JsonConvert.DeserializeObject<Message>(args.Request.Message["UWP"] as string);
                switch (msg.MessageType)
                {
                    case MessageType.NewProcess:
                        Processes.Add(new RemoteProcess());
                        SendMessages(MessageType.Message, 0, StatuesType.Success);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private static void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args) => Environment.Exit(0);
    }
}
