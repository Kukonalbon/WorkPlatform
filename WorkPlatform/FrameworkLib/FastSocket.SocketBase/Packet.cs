using System;
using System.Text;
using System.Threading.Tasks;
using Sodao.FastSocket.SocketBase.Utils;

namespace Sodao.FastSocket.SocketBase
{
    /// <summary>
    /// packet
    /// </summary>
    [Serializable]
    public class Packet
    {
        #region Members
        /// <summary>
        /// get or set sent size.
        /// </summary>
        internal int SentSize = 0;
        /// <summary>
        /// get begin time
        /// </summary>
        public readonly DateTime BeginTime = DateTime.UtcNow;
        /// <summary>
        /// get payload
        /// </summary>
        public readonly byte[] Payload;

        public byte[] Body { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        /// <param name="payload"></param>
        /// <exception cref="ArgumentNullException">payload is null.</exception>
        public Packet(byte[] payload)
        {
            if (payload == null) throw new ArgumentNullException("payload");
            this.Payload = payload;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// get or set tag object
        /// </summary>
        public object Tag { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// 获取一个值，该值指示当前packet是否已发送完毕.
        /// </summary>
        /// <returns>true表示已发送完毕</returns>
        public bool IsSent()
        {
            return this.SentSize >= this.Payload.Length;
        }

        public void SetBody(byte[] body)
        {
            this.Body = body;
        }

        #endregion

        public static Packet Creater(int seqID, byte[] payload)
        {
            var messageLength = (payload == null ? 0 : payload.Length) + 6;
            var sendBuffer = new byte[messageLength + 4];

            //write message length
            Buffer.BlockCopy(NetworkBitConverter.GetBytes(messageLength), 0, sendBuffer, 0, 4);
            //write seqID.
            Buffer.BlockCopy(NetworkBitConverter.GetBytes(seqID), 0, sendBuffer, 4, 4);

            //write body buffer
            if (payload != null && payload.Length > 0)
                Buffer.BlockCopy(payload, 0, sendBuffer, 10, payload.Length);

            var packet = new Packet(sendBuffer);
            packet.SetBody(payload);
            return packet;
        }

    }
}