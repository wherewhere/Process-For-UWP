using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessForUWP.Desktop
{
    public class TCPListener : TcpListener
    {
        public TCPListener(int port) : base(new IPEndPoint(IPAddress.Loopback.Address, port))
        {

        }

        public new TCPClient AcceptTcpClient()
        {
            return new TCPClient(base.AcceptTcpClient());
        }

        public new Task<TCPClient> AcceptTcpClientAsync()
        {
            return new Task<TCPClient>(() => AcceptTcpClient());
        }
    }

    public delegate void StringReceivedEventHandler(TCPClient sender, string str);
    public delegate void ByteReceivedEventHandler(TCPClient sender, byte b);

    public class TCPClient
    {
        public StringReceivedEventHandler StringReceived;
        public ByteReceivedEventHandler ByteReceived;
        public BinaryReader BinaryReader;
        public BinaryWriter BinaryWriter;
        private TcpClient BasicClient;

        public TCPClient(ushort port)
        {
            BasicClient = new TcpClient();
            do
            {
                try
                {
                    BasicClient.Connect(IPAddress.Loopback, port);
                    break;
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                }
            }
            while (true);
            NetworkStream a = BasicClient.GetStream();
            BinaryReader = new BinaryReader(a);
            BinaryWriter = new BinaryWriter(a);
        }

        public TCPClient(TcpClient basic)
        {
            BasicClient = basic;
            NetworkStream a = BasicClient.GetStream();
            BinaryReader = new BinaryReader(a);
            BinaryWriter = new BinaryWriter(a);
        }

        public void Send(string str)
        {
            BinaryWriter.Write(str);
        }

        public void Send(bool[] bools)
        {
            byte b = (byte)(bools[0] ? 1 : 0);
            foreach (bool a in bools)
            {
                b = (byte)(b * 2 + (a ? 1 : 0));
            }
            BinaryWriter.Write((byte)bools.Length);
            BinaryWriter.Write(b);
        }

        public void Send(int num)
        {
            BinaryWriter.Write(num);
        }

        public void SendByte(byte b)
        {
            BinaryWriter.Write(b);
        }

        public void Send(object message)
        {
            if (message is int num)
            {
                BinaryWriter.Write(num);
            }
            else if (message is string str)
            {
                BinaryWriter.Write(str);
            }
            else if(message is bool boolean)
            {
                BinaryWriter.Write(boolean);
            }
        }

        public byte ReceiveByte()
        {
            return BinaryReader.ReadByte();
        }

        public string ReceiveString()
        {
            return BinaryReader.ReadString();
        }

        public bool[] ReceiveBooleans()
        {
            byte a = BinaryReader.ReadByte();
            byte c = BinaryReader.ReadByte();
            bool[] d = new bool[a-1];
            for (sbyte b = (sbyte)(a - 1); b >= 0; b--)
            {
                d[b] = c % 2 == 1;
                c = (byte)Math.Floor((double)(c / 2));
            }
            return d;
        }

        public int ReceiveInt()
        {
            return BinaryReader.ReadInt32();
        }

        public object Receive(Type type)
        {
            if (type == typeof(int))
            {
                return BinaryReader.ReadInt32();
            }
            else if (type == typeof(bool))
            {
                return BinaryReader.ReadBoolean();
            }
            else if (type == typeof(string))
            {
                return BinaryReader.ReadString();
            }
            else
            {
                return null;
            }
        }

        public void ListenString()
        {
            Task.Run(() =>
            {
                do
                {
                    StringReceived(this, BinaryReader.ReadString());
                }
                while (true);
            });
        }

        public void ListenByte()
        {
            Task.Run(() =>
            {
                do
                {
                    ByteReceived(this, BinaryReader.ReadByte());
                }
                while (true);
            });
        }

        public void Close()
        {
            BasicClient.Close();
        }
    }
}
