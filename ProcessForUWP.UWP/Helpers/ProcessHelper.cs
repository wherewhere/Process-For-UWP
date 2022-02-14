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
        private static int ID = 0;
        public static int GetID => ID++;
        private static readonly object locker = new object();

        public static Action<object> SendMessage;
        public static (bool IsReceived, Message Received) Received;
        public static event TypedEventHandler<AppServiceConnection, AppServiceRequestReceivedEventArgs> RequestReceived;

        public static void SendMessages(MessageType typeEnum)
        {
            lock (locker)
            {
                SendMessage(Message.MakeMessage(typeEnum));
            }
        }

        public static void SendMessages(MessageType typeEnum, int id)
        {
            lock (locker)
            {
                SendMessage(Message.MakeMessage(typeEnum, id));
            }
        }

        public static void SendMessages(MessageType typeEnum, object message)
        {
            lock (locker)
            {
                SendMessage(Message.MakeMessage(typeEnum, 0, message));
            }
        }

        public static void SendMessages(MessageType typeEnum, int id, object message)
        {
            lock (locker)
            {
                SendMessage(Message.MakeMessage(typeEnum, id, message));
            }
        }


        public static (bool IsReceive, Message Received) GetMessages(MessageType sendEnum, MessageType receiveEnum)
        {
            Received.IsReceived = false;
            SendMessages(sendEnum);
            return Receive(0, receiveEnum);
        }

        public static (bool IsReceive, Message Received) GetMessages(MessageType sendEnum, int id, MessageType receiveEnum)
        {
            Received.IsReceived = false;
            SendMessages(sendEnum, id);
            return Receive(id, receiveEnum);
        }

        public static (bool IsReceive, Message Received) GetMessages(MessageType sendEnum, object message, MessageType receiveEnum)
        {
            Received.IsReceived = false;
            SendMessages(sendEnum, message);
            return Receive(0, receiveEnum);
        }

        public static (bool IsReceive, Message Received) GetMessages(MessageType sendEnum, int id, object message, MessageType receiveEnum)
        {
            Received.IsReceived = false;
            SendMessages(sendEnum, id, message);
            return Receive(id, receiveEnum);
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

        public static bool IsInitialized
        {
            get
            {
                if (SendMessage != null)
                {
                    return true;
                }
                else
                {
                    CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                    try
                    {
                        Debug.WriteLine("Warning!\nYou may initialize a process when the app is not Initialized.\nDo not do this or app will crash after 60s if app still not Initialized.");
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
        }

        public static (bool IsReceive, Message Received) Receive(int id, MessageType? type = null)
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            try
            {
            wait:
                while (!Received.IsReceived)
                {
                    cancellationToken.Token.ThrowIfCancellationRequested();
                }
                if (Received.Received != null && Received.Received.ID == id && (type == null || Received.Received.MessageType == type))
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

        public static void CopyFile(string sourceFileName, string destFileName, bool overwrite)
        {
            SendMessages(MessageType.CopyFile, (sourceFileName, destFileName, overwrite));
            try
            {
                (bool iscopyed, Message message) = Receive(0, MessageType.CopyFile);
                if (!(iscopyed && message.GetPackage<StatuesType>() == StatuesType.Success))
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                throw new FieldAccessException("Cannot copy this file.");
            }
        }
    }
}
