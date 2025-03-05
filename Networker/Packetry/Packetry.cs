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

            [Serializable]
            public class SendFailure : Exception
            {
                public SendFailure() { }
                public SendFailure(string message) : base(message) { }
                public SendFailure(string message, Exception inner) : base(message, inner) { }
                protected SendFailure(
                  System.Runtime.Serialization.SerializationInfo info,
                  System.Runtime.Serialization.StreamingContext context) { }
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


            [Serializable]
            public class ConnectionFailure : Exception
            {
                public ConnectionFailure() { }
                public ConnectionFailure(string message) : base(message) { }
                public ConnectionFailure(string message, Exception inner) : base(message, inner) { }
                protected ConnectionFailure(
                  System.Runtime.Serialization.SerializationInfo info,
                  System.Runtime.Serialization.StreamingContext context) { }
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
            public long timeRecieved { get; internal set; }
            /// <summary>
            /// Boolean flag indicating whether the packet was sent by the client or the server. Used for deserialisation.
            /// </summary>
            public bool isClient { get; }
        }
    }
}
