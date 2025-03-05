using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace networker
{
    namespace Packetry
    {
        namespace Exceptions
        {

            [Serializable]
            public class FailedtoReadSCRStrmException : Exception // failed to read server-client read stream
            {
                public FailedtoReadSCRStrmException() { }
                public FailedtoReadSCRStrmException(string message) : base(message) { }
                public FailedtoReadSCRStrmException(string message, Exception inner) : base(message, inner) { }
                protected FailedtoReadSCRStrmException(
                  System.Runtime.Serialization.SerializationInfo info,
                  System.Runtime.Serialization.StreamingContext context)
                { }
            }
            [Serializable]
            public class FailedToUnformatPacketException : Exception
            {
                public FailedToUnformatPacketException() { }
                public FailedToUnformatPacketException(string message) : base(message) { }
                public FailedToUnformatPacketException(string message, Exception inner) : base(message, inner) { }
                protected FailedToUnformatPacketException(
                  System.Runtime.Serialization.SerializationInfo info,
                  System.Runtime.Serialization.StreamingContext context)
                { }
            }

            [Serializable]
            public class FailedToFormatPacketException : Exception
            {
                public FailedToFormatPacketException() { }
                public FailedToFormatPacketException(string message) : base(message) { }
                public FailedToFormatPacketException(string message, Exception inner) : base(message, inner) { }
                protected FailedToFormatPacketException(
                  System.Runtime.Serialization.SerializationInfo info,
                  System.Runtime.Serialization.StreamingContext context)
                { }
            }
            [Serializable]
            public class IncorrectTransmissionSideException : Exception
            {
                public IncorrectTransmissionSideException() { }
                public IncorrectTransmissionSideException(string message) : base(message) { }
                public IncorrectTransmissionSideException(string message, Exception inner) : base(message, inner) { }
                protected IncorrectTransmissionSideException(
                  System.Runtime.Serialization.SerializationInfo info,
                  System.Runtime.Serialization.StreamingContext context)
                { }
            }

            [Serializable]
            public class InvalidPacketIDException : Exception
            {
                public InvalidPacketIDException() { }
                public InvalidPacketIDException(string message) : base(message) { }
                public InvalidPacketIDException(string message, Exception inner) : base(message, inner) { }
                protected InvalidPacketIDException(
                  System.Runtime.Serialization.SerializationInfo info,
                  System.Runtime.Serialization.StreamingContext context)
                { }
            }
        }
        public interface IPacket
        {
            /// <summary>
            /// The parent 'packet' interface. 
            /// Basic inheritance structure is Packet>Server/ClientPacket>PacketType(data)
            /// </summary>
            /// <value> The ID of the packet. NOT ITS TYPE. Used to identify packets in the log, keeps synchronicity across client-server. Replicated.</value>
            public int packetID { get; internal set; }
            /// <summary>
            /// The UTC time that the packet was sent, defined as a 'long'. keeps track of ping. Replicated.
            /// </summary>
            public long timeSent { get; internal set; }
            /// <summary>
            /// If the packet is sent by the client or not
            /// </summary>
            public bool isClient { get; }
        }

    }
}