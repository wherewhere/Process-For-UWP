using Newtonsoft.Json;
using ProcessForUWP.Core.Models;
using System;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.AppService;
using Windows.Foundation;

namespace ProcessForUWP.UWP.Helpers
{
    /// <summary>
    /// Communication Helpers for Process.
    /// </summary>
    public static partial class ProcessHelper
    {
        private static int ID = 0;
        internal static int GetID => ID++;
        private static readonly object locker = new object();

        /// <summary>
        /// SendMessage Action.
        /// </summary>
        public static Action<object> SendMessage;
        internal static (bool IsReceived, Message Received) Received;
        internal static event TypedEventHandler<AppServiceConnection, AppServiceRequestReceivedEventArgs> RequestReceived;

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


        internal static (bool IsReceive, Message Received) GetMessages(MessageType sendEnum, MessageType receiveEnum)
        {
            Received.IsReceived = false;
            SendMessages(sendEnum);
            return Receive(0, receiveEnum);
        }

        internal static (bool IsReceive, Message Received) GetMessages(MessageType sendEnum, int id, MessageType receiveEnum)
        {
            Received.IsReceived = false;
            SendMessages(sendEnum, id);
            return Receive(id, receiveEnum);
        }

        internal static (bool IsReceive, Message Received) GetMessages(MessageType sendEnum, object message, MessageType receiveEnum)
        {
            Received.IsReceived = false;
            SendMessages(sendEnum, message);
            return Receive(0, receiveEnum);
        }

        internal static (bool IsReceive, Message Received) GetMessages(MessageType sendEnum, int id, object message, MessageType receiveEnum)
        {
            Received.IsReceived = false;
            SendMessages(sendEnum, id, message);
            return Receive(id, receiveEnum);
        }

        /// <summary>
        /// Receive Message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
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

        internal static bool IsInitialized(double s = 10)
        {
            if (SendMessage != null)
            {
                return true;
            }
            else
            {
                CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(s));
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

        internal static (bool IsReceive, Message Received) Receive(int id, MessageType? type = null)
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

        /// <summary>
        /// Copy File through Delegate.
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="destFileName"></param>
        /// <param name="overwrite"></param>
        /// <exception cref="FieldAccessException">Cannot copy this file.</exception>
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
