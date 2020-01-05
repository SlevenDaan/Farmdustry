using System;

namespace Farmdustry.Network
{
    public class NetworkException : Exception
    {
        public NetworkException(string message) : base(message)
        {

        }
    }
}
