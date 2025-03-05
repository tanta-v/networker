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
            /// <summary>
            /// Exception thrown when the server-client read stream fails.
            /// </summary>
            [Serializable]
            public class FailedtoReadSCRStrmException : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="FailedtoReadSCRStrmException"/> class.
                /// </summary>
                public FailedtoReadSCRStrmException() { }

                /// <summary>
                /// Initializes a new instance of the <see cref="FailedtoReadSCRStrmException"/> class with a specified error message.
                /// </summary>
                /// <param name="message">The error message that explains the reason for the exception.</param>
                public FailedtoReadSCRStrmException(string message) : base(message) { }

                /// <summary>
                /// Initializes a new instance of the <see cref="FailedtoReadSCRStrmException"/> class with a specified error message and a reference to the inner exception.
                /// </summary>
                /// <param name="message">The error message that explains the reason for the exception.</param>
                /// <param name="inner">The exception that is the cause of the current exception.</param>
                public FailedtoReadSCRStrmException(string message, Exception inner) : base(message, inner) { }

                /// <summary>
                /// Initializes a new instance of the <see cref="FailedtoReadSCRStrmException"/> class with serialized data.
                /// </summary>
                /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data.</param>
                /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that provides contextual information about the source or destination.</param>
                protected FailedtoReadSCRStrmException(
                  System.Runtime.Serialization.SerializationInfo info,
                  System.Runtime.Serialization.StreamingContext context)
                { }
            }

            /// <summary>
            /// Exception thrown when the packet fails to unformat correctly.
            /// </summary>
            [Serializable]
            public class FailedToUnformatPacketException : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="FailedToUnformatPacketException"/> class.
                /// </summary>
                public FailedToUnformatPacketException() { }

                /// <summary>
                /// Initializes a new instance of the <see cref="FailedToUnformatPacketException"/> class with a specified error message.
                /// </summary>
                /// <param name="message">The error message that explains the reason for the exception.</param>
                public FailedToUnformatPacketException(string message) : base(message) { }

                /// <summary>
                /// Initializes a new instance of the <see cref="FailedToUnformatPacketException"/> class with a specified error message and a reference to the inner exception.
                /// </summary>
                /// <param name="message">The error message that explains the reason for the exception.</param>
                /// <param name="inner">The exception that is the cause of the current exception.</param>
                public FailedToUnformatPacketException(string message, Exception inner) : base(message, inner) { }

                /// <summary>
                /// Initializes a new instance of the <see cref="FailedToUnformatPacketException"/> class with serialized data.
                /// </summary>
                /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data.</param>
                /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that provides contextual information about the source or destination.</param>
                protected FailedToUnformatPacketException(
                  System.Runtime.Serialization.SerializationInfo info,
                  System.Runtime.Serialization.StreamingContext context)
                { }
            }

            /// <summary>
            /// Exception thrown when the packet fails to format correctly.
            /// </summary>
            [Serializable]
            public class FailedToFormatPacketException : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="FailedToFormatPacketException"/> class.
                /// </summary>
                public FailedToFormatPacketException() { }

                /// <summary>
                /// Initializes a new instance of the <see cref="FailedToFormatPacketException"/> class with a specified error message.
                /// </summary>
                /// <param name="message">The error message that explains the reason for the exception.</param>
                public FailedToFormatPacketException(string message) : base(message) { }

                /// <summary>
                /// Initializes a new instance of the <see cref="FailedToFormatPacketException"/> class with a specified error message and a reference to the inner exception.
                /// </summary>
                /// <param name="message">The error message that explains the reason for the exception.</param>
                /// <param name="inner">The exception that is the cause of the current exception.</param>
                public FailedToFormatPacketException(string message, Exception inner) : base(message, inner) { }

                /// <summary>
                /// Initializes a new instance of the <see cref="FailedToFormatPacketException"/> class with serialized data.
                /// </summary>
                /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data.</param>
                /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that provides contextual information about the source or destination.</param>
                protected FailedToFormatPacketException(
                  System.Runtime.Serialization.SerializationInfo info,
                  System.Runtime.Serialization.StreamingContext context)
                { }
            }

            /// <summary>
            /// Exception thrown when the transmission side of the packet is incorrect.
            /// </summary>
            [Serializable]
            public class IncorrectTransmissionSideException : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="IncorrectTransmissionSideException"/> class.
                /// </summary>
                public IncorrectTransmissionSideException() { }

                /// <summary>
                /// Initializes a new instance of the <see cref="IncorrectTransmissionSideException"/> class with a specified error message.
                /// </summary>
                /// <param name="message">The error message that explains the reason for the exception.</param>
                public IncorrectTransmissionSideException(string message) : base(message) { }

                /// <summary>
                /// Initializes a new instance of the <see cref="IncorrectTransmissionSideException"/> class with a specified error message and a reference to the inner exception.
                /// </summary>
                /// <param name="message">The error message that explains the reason for the exception.</param>
                /// <param name="inner">The exception that is the cause of the current exception.</param>
                public IncorrectTransmissionSideException(string message, Exception inner) : base(message, inner) { }

                /// <summary>
                /// Initializes a new instance of the <see cref="IncorrectTransmissionSideException"/> class with serialized data.
                /// </summary>
                /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data.</param>
                /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that provides contextual information about the source or destination.</param>
                protected IncorrectTransmissionSideException(
                  System.Runtime.Serialization.SerializationInfo info,
                  System.Runtime.Serialization.StreamingContext context)
                { }
            }

            /// <summary>
            /// Exception thrown when the packet ID is invalid.
            /// </summary>
            [Serializable]
            public class InvalidPacketIDException : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="InvalidPacketIDException"/> class.
                /// </summary>
                public InvalidPacketIDException() { }

                /// <summary>
                /// Initializes a new instance of the <see cref="InvalidPacketIDException"/> class with a specified error message.
                /// </summary>
                /// <param name="message">The error message that explains the reason for the exception.</param>
                public InvalidPacketIDException(string message) : base(message) { }

                /// <summary>
                /// Initializes a new instance of the <see cref="InvalidPacketIDException"/> class with a specified error message and a reference to the inner exception.
                /// </summary>
                /// <param name="message">The error message that explains the reason for the exception.</param>
                /// <param name="inner">The exception that is the cause of the current exception.</param>
                public InvalidPacketIDException(string message, Exception inner) : base(message, inner) { }

                /// <summary>
                /// Initializes a new instance of the <see cref="InvalidPacketIDException"/> class with serialized data.
                /// </summary>
                /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data.</param>
                /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that provides contextual information about the source or destination.</param>
                protected InvalidPacketIDException(
                  System.Runtime.Serialization.SerializationInfo info,
                  System.Runtime.Serialization.StreamingContext context)
                { }
            }
        }

        /// <summary>
        /// Interface representing a generic packet structure.
        /// </summary>
        public interface IPacket
        {
            /// <summary>
            /// The ID of the packet's type. Used to regulate deserialization.
            /// The basic inheritance structure is Packet > Server/ClientPacket > PacketType (data).
            /// </summary>
            /// <value> The packet's type. Default is -1, to detect incorrectly formatted packets. Set this. </value>
            public int packetType { get { return -1; } }

            /// <summary>
            /// The unique ID of the packet. Used for primarily logging purposes.
            /// </summary>
            /// <value> The packet's unique logging ID. </value>
            public int packetID { get { return -1; } }

            /// <summary>
            /// The UTC time that the packet was sent, defined as a long.
            /// Used to track ping and replicate the packet.
            /// </summary>
            public long timeSent { get; internal set; }

            /// <summary>
            /// Boolean flag indicating whether the packet was sent by the client or the server. Used for deserialisation.
            /// </summary>
            public bool isClient { get; }
        }
    }
}
