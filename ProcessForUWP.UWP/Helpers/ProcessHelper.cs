using Newtonsoft.Json;
using ProcessForUWP.Core.Models;
using System;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.AppService;

namespace ProcessForUWP.UWP.Helpers
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
                    Message msg = JsonConvert.DeserializeObject<Message>(args.Request.Message["Desktop"] as string);
                    Received = (true, msg);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public static (bool IsReceive, Message Received) Receive(ControlType? type = null)
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            try
            {
            wait:
                while (!Received.IsReceived)
                {
                    cancellationToken.Token.ThrowIfCancellationRequested();
                }
                if (Received.Received != null && (type == null || Received.Received.ControlType == type))
                {
                    return (true, Received.Received);
                }
                else
                {
                    Received.IsReceived = false;
                    goto wait;
                }
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
