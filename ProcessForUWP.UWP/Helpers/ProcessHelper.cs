using Newtonsoft.Json;
using ProcessForUWP.Core.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Windows.ApplicationModel.AppService;
using Windows.Foundation;

namespace ProcessForUWP.UWP.Helpers
{
    public static partial class ProcessHelper
    {
        public static Action<object> SendMessage;
        public static (bool IsReceived, Message Received) Received;
        public static event TypedEventHandler<AppServiceConnection, AppServiceRequestReceivedEventArgs> RequestReceived;

        public static void SendMessages(MessageType typeEnum)
        {
            SendMessages(typeEnum, 0);
        }

        public static void SendMessages(MessageType typeEnum, object message)
        {
            SendMessages(typeEnum, 0, message);
        }

        public static void SendMessages(MessageType typeEnum, int id, object message)
        {
            SendMessage(Message.MakeMessage(typeEnum, id, message));
        }


        public static (bool IsReceive, Message Received) GetMessages(MessageType sendEnum, MessageType receiveEnum)
        {
            return GetMessages(sendEnum, 0, receiveEnum);
        }

        public static (bool IsReceive, Message Received) GetMessages(MessageType sendEnum, object message, MessageType receiveEnum)
        {
            return GetMessages(sendEnum, 0, message, receiveEnum);
        }

        public static (bool IsReceive, Message Received) GetMessages(MessageType sendEnum, int id, object message, MessageType receiveEnum)
        {
            Received.IsReceived = false;
            SendMessages(sendEnum, id, message);
            return Receive(receiveEnum);
        }

        public static void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            RequestReceived?.Invoke(sender, args);
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

        public static (bool IsReceive, Message Received) Receive(MessageType? type = null)
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            try
            {
            wait:
                while (!Received.IsReceived)
                {
                    cancellationToken.Token.ThrowIfCancellationRequested();
                }
                if (Received.Received != null && (type == null || Received.Received.MessageType == type))
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
