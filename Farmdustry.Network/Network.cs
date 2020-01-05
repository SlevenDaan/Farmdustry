using System;
using System.Net;
using System.Net.Sockets;

namespace Farmdustry.Network
{
    public static class Network
    {
        public static IPAddress GetLocalIp(AddressFamily type = AddressFamily.InterNetwork)
        {
            foreach (IPAddress address in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (address.AddressFamily == type)
                {
                    return address;
                }
            }
            throw new Exception($"No address of type {type.ToString()} was found.");
        }

        private const AddressFamily addressFamily = AddressFamily.InterNetwork;
        private const ProtocolType protocolType = ProtocolType.Tcp;
        internal static Socket CreateSocket()
        {
            Socket newSocket = new Socket(addressFamily, SocketType.Stream, protocolType);
            newSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
            return newSocket;
        }
    }
}
