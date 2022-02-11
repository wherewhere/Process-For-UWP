using Newtonsoft.Json;
using ProcessForUWP.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace ProcessForUWP.Desktop
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

        public async static void SendMessage(object value)
        {
            string json = JsonConvert.SerializeObject(value);
            try
            {
                ValueSet message = new ValueSet() { { $"1", json } };
                _ = await Connection.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Debug.WriteLine(json);
            }
        }

        public static void SendMessages(ControlType typeEnum)
        {
            SendMessage(Message.MakeMessage(typeEnum));
        }

        public static void SendMessages(ControlType typeEnum, object message = null)
        {
            SendMessage(Message.MakeMessage(typeEnum, 0, message));
        }

        public static void SendMessages(ControlType typeEnum, int id, object message = null)
        {
            SendMessage(Message.MakeMessage(typeEnum, id, message));
        }

        private static void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            try
            {
                Message msg = JsonConvert.DeserializeObject<Message>(args.Request.Message["1"] as string);
                switch (msg.ControlType)
                {
                    case ControlType.NewProcess:
                        Processes.Add(new RemoteProcess());
                        SendMessages(ControlType.Message, 0, StatuesType.Success);
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
