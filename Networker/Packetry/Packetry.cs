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
            public class RecieveFailure : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="RecieveFailure"/> class.
                /// </summary>
                public RecieveFailure() { }

                /// <summary>
                /// Initializes a new instance of the <see cref="RecieveFailure"/> class with a specified error message.
                /// </summary>
                /// <param name="message">The error message that explains the reason for the exception.</param>
                public RecieveFailure(string message) : base(message) { }

                /// <summary>
                /// Initializes a new instance of the <see cref="RecieveFailure"/> class with a specified error message and a reference to the inner exception.
                /// </summary>
                /// <param name="message">The error message that explains the reason for the exception.</param>
                /// <param name="inner">The exception that is the cause of the current exception.</param>
                public RecieveFailure(string message, Exception inner) : base(message, inner) { }

                /// <summary>
                /// Initializes a new instance of the <see cref="RecieveFailure"/> class with serialized data.
                /// </summary>
                /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data.</param>
                /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that provides contextual information about the source or destination.</param>
                protected RecieveFailure(
                  System.Runtime.Serialization.SerializationInfo info,
                  System.Runtime.Serialization.StreamingContext context)
                { }
            }

            /// <summary>
            /// Exception thrown when there is a failure to send a packet.
            /// </summary>
            [Serializable]
            public class SendFailure : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="SendFailure"/> class.
                /// </summary>
                public SendFailure() { }

                /// <summary>
                /// Initializes a new instance of the <see cref="SendFailure"/> class with a specified error message.
                /// </summary>
                /// <param name="message">The error message that explains the reason for the exception.</param>
                public SendFailure(string message) : base(message) { }

                /// <summary>
                /// Initializes a new instance of the <see cref="SendFailure"/> class with a specified error message and a reference to the inner exception.
                /// </summary>
                /// <param name="message">The error message that explains the reason for the exception.</param>
                /// <param name="inner">The exception that is the cause of the current exception.</param>
                public SendFailure(string message, Exception inner) : base(message, inner) { }

                /// <summary>
                /// Initializes a new instance of the <see cref="SendFailure"/> class with serialized data.
                /// </summary>
                /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data.</param>
                /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that provides contextual information about the source or destination.</param>
                protected SendFailure(
                  System.Runtime.Serialization.SerializationInfo info,
                  System.Runtime.Serialization.StreamingContext context)
                { }
            }

            /// <summary>
            /// Exception thrown when the packet fails to unformat correctly.
            /// </summary>
            [Serializable]
            public class UnformatFailure : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="UnformatFailure"/> class.
                /// </summary>
                public UnformatFailure() { }

                /// <summary>
                /// Initializes a new instance of the <see cref="UnformatFailure"/> class with a specified error message.
                /// </summary>
                /// <param name="message">The error message that explains the reason for the exception.</param>
                public UnformatFailure(string message) : base(message) { }

                /// <summary>
                /// Initializes a new instance of the <see cref="UnformatFailure"/> class with a specified error message and a reference to the inner exception.
                /// </summary>
                /// <param name="message">The error message that explains the reason for the exception.</param>
                /// <param name="inner">The exception that is the cause of the current exception.</param>
                public UnformatFailure(string message, Exception inner) : base(message, inner) { }

                /// <summary>
                /// Initializes a new instance of the <see cref="UnformatFailure"/> class with serialized data.
                /// </summary>
                /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data.</param>
                /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that provides contextual information about the source or destination.</param>
                protected UnformatFailure(
                  System.Runtime.Serialization.SerializationInfo info,
                  System.Runtime.Serialization.StreamingContext context)
                { }
            }

            /// <summary>
            /// Exception thrown when the server-client connection fails.
            /// </summary>
            [Serializable]
            public class ConnectionFailure : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="ConnectionFailure"/> class.
                /// </summary>
                public ConnectionFailure() { }

                /// <summary>
                /// Initializes a new instance of the <see cref="ConnectionFailure"/> class with a specified error message.
                /// </summary>
                /// <param name="message">The error message that explains the reason for the exception.</param>
                public ConnectionFailure(string message) : base(message) { }

                /// <summary>
                /// Initializes a new instance of the <see cref="ConnectionFailure"/> class with a specified error message and a reference to the inner exception.
                /// </summary>
                /// <param name="message">The error message that explains the reason for the exception.</param>
                /// <param name="inner">The exception that is the cause of the current exception.</param>
                public ConnectionFailure(string message, Exception inner) : base(message, inner) { }

                /// <summary>
                /// Initializes a new instance of the <see cref="ConnectionFailure"/> class with serialized data.
                /// </summary>
                /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data.</param>
                /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that provides contextual information about the source or destination.</param>
                protected ConnectionFailure(
                  System.Runtime.Serialization.SerializationInfo info,
                  System.Runtime.Serialization.StreamingContext context)
                { }
            }

            /// <summary>
            /// Exception thrown when the packet fails to format correctly.
            /// </summary>
            [Serializable]
            public class FormatFailure : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="FormatFailure"/> class.
                /// </summary>
                public FormatFailure() { }

                /// <summary>
                /// Initializes a new instance of the <see cref="FormatFailure"/> class with a specified error message.
                /// </summary>
                /// <param name="message">The error message that explains the reason for the exception.</param>
                public FormatFailure(string message) : base(message) { }

                /// <summary>
                /// Initializes a new instance of the <see cref="FormatFailure"/> class with a specified error message and a reference to the inner exception.
                /// </summary>
                /// <param name="message">The error message that explains the reason for the exception.</param>
                /// <param name="inner">The exception that is the cause of the current exception.</param>
                public FormatFailure(string message, Exception inner) : base(message, inner) { }

                /// <summary>
                /// Initializes a new instance of the <see cref="FormatFailure"/> class with serialized data.
                /// </summary>
                /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data.</param>
                /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that provides contextual information about the source or destination.</param>
                protected FormatFailure(
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
            /// </summary>
            /// <value> The packet's type.  </value>
            public int packetType { get; set; }

            /// <summary>
            /// The unique ID of the packet. Used for primarily logging purposes.
            /// </summary>
            /// <value> The packet's unique logging ID. </value>
            public int packetID { get; set; }

            /// <summary>
            /// The UTC time that the packet was sent, defined as a long.
            /// Used to track ping and replicate the packet.
            /// </summary>
            public long timeSent { get; internal set; }

            /// <summary>
            /// The UTC time that the packet was received, defined as a long.
            /// </summary>
            public long timeRecieved { get; internal set; }

            /// <summary>
            /// Boolean flag indicating whether the packet was sent by the client or the server. Used for deserialization.
            /// </summary>
            public bool isClient { get; set; }
        }
    }
}
