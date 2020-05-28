using System;
using System.Net.Sockets;
using EasyTcp3.Protocols;

namespace EasyTcp3
{
    /// <summary>
    /// Class that holds all the information and some functions of an EasyTcpClient
    /// See ClientUtils for more functions
    /// </summary>
    public class EasyTcpClient : IDisposable
    {
        /// <summary>
        /// BaseSocket of client,
        /// Gets disposed when calling Dispose()
        /// Null if disconnected
        /// </summary>
        public Socket BaseSocket { get; set; }

        /// <summary>
        /// Protocol for this client,
        /// determines actions when receiving/sending data etc..
        /// </summary>
        public IEasyTcpProtocol Protocol { get; set; }

        /// <summary>
        /// Buffer used for receiving incoming data. See Internal/OnConnectUtil.cs for usage
        /// </summary>
        public byte[] Buffer;

        /// <summary>
        /// Fired when the client connects to a server
        /// </summary>
        public event EventHandler<EasyTcpClient> OnConnect;

        /// <summary>
        /// Fired when a client disconnects from a server to the server
        /// </summary>
        public event EventHandler<EasyTcpClient> OnDisconnect;

        /// <summary>
        /// Fired when a client receives new information,
        /// Doesn't fire when one of these functions are called:
        /// SendAndGetReply
        /// SendAndReplyAsync
        /// </summary>
        public event EventHandler<Message> OnDataReceive;

        /// <summary>
        /// Fired when an error occurs,
        /// if not set errors will be thrown
        /// </summary>
        public event EventHandler<Exception> OnError;

        /// <summary>
        /// Function used to fire the OnConnect event
        /// </summary>
        protected internal void FireOnConnect() => OnConnect?.Invoke(this, this);

        /// <summary>
        /// Function used to fire the OnDisconnect event
        /// </summary>
        protected internal void FireOnDisconnect() => OnDisconnect?.Invoke(this, this);

        /// <summary>
        /// Function used to fire the OnError event,
        /// or if event is null, throw an exception
        /// </summary>
        /// <param name="exception"></param>
        protected internal void FireOnError(Exception exception)
        {
            if (OnError != null) OnError.Invoke(this, exception);
#if DEBUG
            else throw exception;
#endif
        }

        /// <summary>
        /// Function used to fire the OnDataReceive event
        /// </summary>
        /// <param name="message">received message</param>
        protected internal void FireOnDataReceiveEvent(Message message) => OnDataReceive?.Invoke(this, message);

        /// <summary>
        /// Action that is called when new data is received
        /// </summary>
        public Action<Message> DataReceiveHandler;

        /// <summary>
        /// Reset DataReceiveHandler to its default behavior (Calling OnDataReceive)
        /// </summary>
        public void ResetDataReceiveHandler() => DataReceiveHandler = FireOnDataReceiveEvent;

        /// <summary>
        /// </summary>
        /// <param name="protocol">determines actions when sending/receiving data etc.. PrefixLengthProtocol is used when null</param>
        public EasyTcpClient(IEasyTcpProtocol protocol = null)
        {
            Protocol = protocol ?? new PrefixLengthProtocol();
            ResetDataReceiveHandler();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="protocol">determines actions when sending/receiving data etc.. DefaultProtocol is used when null</param>
        public EasyTcpClient(Socket socket, IEasyTcpProtocol protocol = null) : this(protocol) => BaseSocket = socket;

        /// <summary>
        /// Dispose current instance of the baseSocket if not null,
        /// client will disconnect when function is called
        /// </summary>
        public void Dispose()
        {
            BaseSocket?.Dispose();
            BaseSocket = null;
        }
    }
}