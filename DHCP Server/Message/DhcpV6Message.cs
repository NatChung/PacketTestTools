/**
 * Title:        DhcpV6Message
 * Description:  Object that represents a DHCPv6 message as defined in
 *               RFC 3315.
 *               
 * The following diagram illustrates the format of DHCP messages sent
 * between clients and servers:
 *
 * <pre>
 *     0                   1                   2                   3
 *     0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 *    |    msg-type   |               transaction-id                  |
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 *    |                                                               |
 *    .                            options                            .
 *    .                           (variable)                          .
 *    |                                                               |
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 *
 *    msg-type             Identifies the DHCP message type; the
 *                         available message types are listed in
 *                         section 5.3.
 *
 *    transaction-id       The transaction ID for this message exchange.
 *
 *    options              Options carried in this message; options are
 *                         described in section 22.
 *
 *   The format of DHCP options is:

 *     0                   1                   2                   3
 *     0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 *    |          option-code          |           option-len          |
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 *    |                       option-opaqueData                       |
 *    |                      (option-len octets)                      |
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 * 
 *    option-code   An unsigned integer identifying the specific option
 *                  type carried in this option.
 * 
 *    option-len    An unsigned integer giving the length of the
 *                  option-opaqueData field in this option in octets.
 * 
 *    option-opaqueData   The opaqueData for the option; the format of this opaqueData
 *                  depends on the definition of the option.
 * </pre>
 * 
 * @author A. Gregory Rabil
 */
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Option.V6;
using PIXIS.DHCP.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Message
{
    public class DhcpV6Message : DhcpMessage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //  true if the message was received on a unicast socket, false otherwise
        //  note that a DhcpRelayMessage (subclass) may be unicast, but the "inner"
        //  DhcpMessage will _not_ be unicast, which is the desired behavior
        protected bool unicast;

        //  the IP and port on the local host on 
        //  which the message is sent or received
        protected IPAddress localAddress;

        //  the IP and port on the remote host from
        //  which the message is received or sent
        protected IPEndPoint remoteAddress;

        public short MessageType = 0;

        //  need a short to hold unsigned byte
        protected int transactionId = 0;

        //  we only use low order three bytes
        protected Dictionary<int, DhcpOption> dhcpOptions = new Dictionary<int, DhcpOption>();

        protected List<DhcpV6IaNaOption> iaNaOptions = new List<DhcpV6IaNaOption>();

        protected List<DhcpV6IaTaOption> iaTaOptions = new List<DhcpV6IaTaOption>();

        protected List<DhcpV6IaPdOption> iaPdOptions = new List<DhcpV6IaPdOption>();


        /// <summary>
        /// 租期結束時間
        /// </summary>
        public DateTime GetValidEndTime()
        {
            long validLifetime = 0;
            DhcpV6IaNaOption iaOption = (DhcpV6IaNaOption)GetIaNaOptions().FirstOrDefault();
            if (iaOption != null && iaOption.GetIaAddrOptions() != null)
            { 
                var iaAddress = iaOption.GetIaAddrOptions().FirstOrDefault();
                validLifetime = iaAddress.GetValidLifetime();
            }

            return DateTime.Now.AddSeconds(validLifetime);
        }



        public DhcpV6Message(IPAddress localAddress, IPEndPoint remoteAddress)
        {
            this.localAddress = localAddress;
            this.remoteAddress = remoteAddress;
        }

        public void SetUnicast(bool unicast)
        {
            this.unicast = unicast;
        }

        public bool IsUnicast()
        {
            return this.unicast;
        }

        /// <summary>
        ///  Return the length of this DhcpMessage in bytes.
        /// </summary>
        /// <returns>an int containing a length of a least four(4)</returns>
        public virtual int GetLength()
        {
            int len = 4;    // msg type (1) + transaction id (3)
            len += GetOptionsLength();
            len += GetIaNaOptionsLength();
            len += GetIaTaOptionsLength();
            len += GetIaPdOptionsLength();
            return len;
        }

        /// <summary>
        /// Get the length of the options in this DhcpMessage in bytes.
        /// </summary>
        /// <returns>an int containing the total length of all options</returns>
        protected int GetOptionsLength()
        {
            int len = 0;
            if ((this.dhcpOptions != null))
            {
                foreach (DhcpOption option in this.dhcpOptions.Values)
                {
                    len += 4;   // option code (2 bytes) + length (2 bytes) 
                    len += option.GetLength();
                }

            }

            return len;
        }

        /// <summary>
        /// Get the length of the IA_NA options in this DhcpMessage in bytes.
        /// </summary>
        /// <returns>an int containing the total length of all IA_NA options</returns>
        protected int GetIaNaOptionsLength()
        {
            int len = 0;
            if ((this.iaNaOptions != null))
            {
                foreach (DhcpV6IaNaOption option in this.iaNaOptions)
                {
                    len += 4;
                    //  option code (2 bytes) + length (2 bytes) 
                    len = (len + option.GetLength());
                }

            }

            return len;
        }

        /// <summary>
        /// Get the length of the IA_TA options in this DhcpMessage in bytes.
        /// </summary>
        /// <returns>an int containing the total length of all IA_TA options</returns>
        protected int GetIaTaOptionsLength()
        {
            int len = 0;
            if ((this.iaTaOptions != null))
            {
                foreach (DhcpV6IaTaOption option in this.iaTaOptions)
                {
                    len += 4;
                    //  option code (2 bytes) + length (2 bytes) 
                    len = (len + option.GetLength());
                }

            }

            return len;
        }

        /// <summary>
        /// Return the length of the IA_PD options in this DhcpMessage in bytes.
        /// </summary>
        /// <returns>an int containing the total length of all IA_PD options</returns>
        protected int GetIaPdOptionsLength()
        {
            int len = 0;
            if ((this.iaPdOptions != null))
            {
                foreach (DhcpV6IaPdOption option in this.iaPdOptions)
                {
                    len += 4;
                    //  option code (2 bytes) + length (2 bytes) 
                    len = (len + option.GetLength());
                }

            }

            return len;
        }

        public IPAddress GetLocalAddress()
        {
            return this.localAddress;
        }

        public void SetLocalAddress(IPAddress localAddress)
        {
            this.localAddress = localAddress;
        }

        public IPEndPoint GetRemoteAddress()
        {
            return this.remoteAddress;
        }

        public void SetRemoteAddress(IPAddress remoteAddress)
        {
            this.remoteAddress.Address = remoteAddress;
        }

        public short GetMessageType()
        {
            return this.MessageType;
        }

        public void SetMessageType(short messageType)
        {
            this.MessageType = messageType;
        }

        public int GetTransactionId()
        {
            return this.transactionId;
        }

        public void SetTransactionId(int transactionId)
        {
            this.transactionId = transactionId;
        }

        public bool HasOption(int optionCode)
        {
            if (this.dhcpOptions.ContainsKey(optionCode))
            {
                return true;
            }

            return false;
        }

        public DhcpOption GetDhcpOption(int optionCode)
        {
            return this.dhcpOptions.Get(optionCode);
        }

        public void PutDhcpOption(DhcpOption dhcpOption)
        {
            if ((dhcpOption != null))
            {
                this.dhcpOptions[dhcpOption.GetCode()] = dhcpOption;
            }

        }

        public void PutAllDhcpOptions(Dictionary<int, DhcpOption> dhcpOptions)
        {
            this.dhcpOptions.PutAll(dhcpOptions);
        }

        public Dictionary<int, DhcpOption> GetDhcpOptionMap()
        {
            return this.dhcpOptions;
        }

        public void SetDhcpOptionMap(Dictionary<int, DhcpOption> dhcpOptions)
        {
            this.dhcpOptions = dhcpOptions;
        }

        public List<DhcpOption> GetDhcpOptions()
        {
            return this.dhcpOptions.Values.ToList();
        }

        /// <summary>
        /// 取得派出的 IP
        /// </summary>
        /// <returns></returns>
        public IPAddress GetIaAddress()
        {
            if (GetIaNaOptions() == null || GetIaNaOptions().Count == 0) return IPAddress.IPv6Any;
            DhcpV6IaNaOption iaOption = (DhcpV6IaNaOption)GetIaNaOptions().FirstOrDefault();
            if (iaOption.GetIaAddrOptions() == null || iaOption.GetIaAddrOptions().Count == 0) return IPAddress.IPv6Any;
            var iaAddress = iaOption.GetIaAddrOptions().FirstOrDefault();
            return iaAddress.GetInetAddress();
        }

        public List<DhcpV6IaNaOption> GetIaNaOptions()
        {
            return this.iaNaOptions;
        }

        public void SetIaNaOptions(List<DhcpV6IaNaOption> iaNaOptions)
        {
            this.iaNaOptions = iaNaOptions;
        }

        public List<DhcpV6IaTaOption> GetIaTaOptions()
        {
            return this.iaTaOptions;
        }

        public void SetIaTaOptions(List<DhcpV6IaTaOption> iaTaOptions)
        {
            this.iaTaOptions = iaTaOptions;
        }

        public void AddIaTaOption(DhcpV6IaTaOption iaTaOption)
        {
            if ((this.iaTaOptions == null))
            {
                this.iaTaOptions = new List<DhcpV6IaTaOption>();
            }

            this.iaTaOptions.Add(iaTaOption);
        }

        public List<DhcpV6IaPdOption> GetIaPdOptions()
        {
            return this.iaPdOptions;
        }

        public void SetIaPdOptions(List<DhcpV6IaPdOption> iaPdOptions)
        {
            this.iaPdOptions = iaPdOptions;
        }

        public void AddIaPdOption(DhcpV6IaPdOption iaPdOption)
        {
            if ((this.iaPdOptions == null))
            {
                this.iaPdOptions = new List<DhcpV6IaPdOption>();
            }

            this.iaPdOptions.Add(iaPdOption);
        }

        private DhcpV6ClientIdOption dhcpClientIdOption;

        public DhcpV6ClientIdOption GetDhcpClientIdOption()
        {
            if (this.dhcpClientIdOption == null)
            {
                if (this.dhcpOptions != null && this.dhcpOptions.Count > 0)
                {
                    this.dhcpClientIdOption = (DhcpV6ClientIdOption)this.dhcpOptions[DhcpConstants.V6OPTION_CLIENTID];
                }
            }
            return this.dhcpClientIdOption;
        }

        private DhcpV6ServerIdOption dhcpServerIdOption;

        public DhcpV6ServerIdOption GetDhcpServerIdOption()
        {
            if (this.dhcpServerIdOption == null)
            {
                if (this.dhcpOptions != null)
                {
                    this.dhcpServerIdOption = (DhcpV6ServerIdOption)dhcpOptions.Get(DhcpConstants.V6OPTION_SERVERID);
                }

            }

            return this.dhcpServerIdOption;
        }

        private List<int> requestedOptionCodes;

        public List<int> GetRequestedOptionCodes()
        {
            if ((this.requestedOptionCodes == null))
            {
                if ((this.dhcpOptions != null))
                {
                    DhcpV6OptionRequestOption oro = ((DhcpV6OptionRequestOption)(this.dhcpOptions[DhcpConstants.V6OPTION_ORO]));
                    if ((oro != null))
                    {
                        this.requestedOptionCodes = oro.getUnsignedShortList();
                    }

                }

            }

            return this.requestedOptionCodes;
        }

        /// <summary>
        ///  Decode a datagram packet into this DhcpMessage object.
        /// </summary>
        /// <param name="buf">ByteBuffer containing the packet to be decoded</param>
        public void Decode(ByteBuffer buf)
        {

            log.Debug("Decoding DhcpMessage from: " + remoteAddress);

            if ((buf != null) && buf.hasRemaining())
            {
                DecodeMessageType(buf);
                if (buf.hasRemaining())
                {
                    SetTransactionId(DhcpTransactionId.Decode(buf));
                    if (log.IsDebugEnabled)
                        log.Debug("TransactionId=" + transactionId);
                    if (buf.hasRemaining())
                    {
                        DecodeOptions(buf);
                    }
                    else
                    {
                        string errmsg = "Failed to decode options: buffer is empty";
                        log.Error(errmsg);
                        throw new IOException(errmsg);
                    }
                }
                else
                {
                    string errmsg = "Failed to decode transaction id: buffer is empty";
                    log.Error(errmsg);
                    throw new IOException(errmsg);
                }
            }
            else
            {
                string errmsg = "Failed to decode message: buffer is empty";
                log.Error(errmsg);
                throw new IOException(errmsg);
            }

            if (log.IsDebugEnabled)
            {
                log.Debug("DhcpMessage decoded.");
            }
        }
        public static DhcpV6Message Decode(ByteBuffer buf, IPAddress localAddr, IPEndPoint remoteAddr)
        {
            DhcpV6Message dhcpMessage = null;
            if ((buf != null) && buf.hasRemaining())
            {

                if (log.IsDebugEnabled)
                {
                    log.Debug("Decoding packet:" +
                            " size=" + buf.limit() +
                            " localAddr=" + localAddr +
                            " remoteAddr=" + remoteAddr.Address);
                }

                // we'll "peek" at the message type to use for this mini-factory
                byte msgtype = buf.getByte();
                if (log.IsDebugEnabled)
                    log.Debug("Message type byte=" + msgtype);

                if ((msgtype >= DhcpConstants.V6MESSAGE_TYPE_SOLICIT) &&
                        (msgtype <= DhcpConstants.V6MESSAGE_TYPE_INFO_REQUEST))
                {
                    dhcpMessage = new DhcpV6Message(localAddr, remoteAddr);
                }
                else if ((msgtype >= DhcpConstants.V6MESSAGE_TYPE_RELAY_FORW) &&
                        (msgtype <= DhcpConstants.V6MESSAGE_TYPE_RELAY_REPL))
                {
                    // note that it doesn't make much sense to be decoding
                    // a relay-reply message unless we implement a relay
                    dhcpMessage = new DhcpV6RelayMessage(localAddr, remoteAddr);
                }
                else
                {
                    log.Error("Unknown message type: " + msgtype);
                }

                if (dhcpMessage != null)
                {
                    // reset the buffer to point at the message type byte
                    // because the message decoder will expect it
                    buf.rewind();
                    dhcpMessage.Decode(buf);
                }
            }
            else
            {
                String errmsg = "Buffer is null or empty";
                log.Error(errmsg);
                throw new IOException(errmsg);
            }
            return dhcpMessage;
        }


        /// <summary>
        /// Decode the options.
        /// </summary>
        /// <param name="buf">ByteBuffer positioned at the start of the options in the packet</param>
        /// <returns>a Map of DhcpOptions keyed by the option code</returns>
        protected Dictionary<int, DhcpOption> DecodeOptions(ByteBuffer buf)
        {
            while (buf.hasRemaining())
            {
                int code = Util.GetUnsignedShort(buf);
                if (log.IsDebugEnabled)
                    log.Debug("Option code=" + code);
                DhcpOption option = DhcpV6OptionFactory.GetDhcpOption(code);
                if (option != null)
                {
                    if ((option is DhcpV6RelayOption) &&
                            (this is DhcpV6RelayMessage))
                    {
                        DhcpV6RelayOption relayOption = (DhcpV6RelayOption)option;
                        relayOption.SetRelayMessage((DhcpV6RelayMessage)this);
                    }
                    option.Decode(buf);
                    if (option is DhcpV6IaNaOption)
                    {
                        iaNaOptions.Add((DhcpV6IaNaOption)option);
                    }
                    else if (option is DhcpV6IaTaOption)
                    {
                        iaTaOptions.Add((DhcpV6IaTaOption)option);
                    }
                    else if (option is DhcpV6IaPdOption)
                    {
                        iaPdOptions.Add((DhcpV6IaPdOption)option);
                    }
                    else
                    {
                        dhcpOptions[option.GetCode()] = option;
                    }
                }
                else
                {
                    break;  // no more options, or one is malformed, so we're done
                }
            }
            return dhcpOptions;
        }



        /// <summary>
        /// Decode the message type.
        /// </summary>
        protected void DecodeMessageType(ByteBuffer buf)
        {
            if ((buf != null) && buf.hasRemaining())
            {
                SetMessageType(Util.GetUnsignedByte(buf));
            }
            else
            {
                string errmsg = "Failed to decode message type: buffer is empty";
                log.Error(errmsg);
                throw new IOException(errmsg);
            }
        }

        /// <summary>
        /// Encode this DhcpMessage to wire format for sending.
        /// </summary>
        /// <returns>a ByteBuffer containing the encoded DhcpMessage</returns>
        public ByteBuffer Encode()
        {
            if (log.IsDebugEnabled)
                log.Debug("Encoding DhcpMessage for: " + remoteAddress);

            ByteBuffer buf = ByteBuffer.allocate(1024);
            buf.put((byte)MessageType);
            buf.put(DhcpTransactionId.Encode(transactionId));
            buf.put(EncodeOptions());
            buf.flip();

            if (log.IsDebugEnabled)
                log.Debug("DhcpMessage encoded.");

            return buf;
        }

        /// <summary>
        ///  Encode the options of this DhcpMessage to wire format for sending.
        /// </summary>
        /// <returns>a ByteBuffer containing the encoded options</returns>
        protected ByteBuffer EncodeOptions()
        {
            ByteBuffer buf = ByteBuffer.allocate(1020); // 1024 - 1(msgType) - 3(transId) = 1020 (options)
            if (dhcpOptions != null)
            {
                foreach (DhcpOption option in dhcpOptions.Values.OrderBy(p => p.GetCode()))
                {
                    buf.put(option.Encode());
                }
            }
            if (iaNaOptions != null)
            {
                foreach (DhcpV6IaNaOption iaNaOption in iaNaOptions)
                {
                    buf.put(iaNaOption.Encode());
                }
            }
            if (iaTaOptions != null)
            {
                foreach (DhcpV6IaTaOption iaTaOption in iaTaOptions)
                {
                    buf.put(iaTaOption.Encode());
                }
            }
            if (iaPdOptions != null)
            {
                foreach (DhcpV6IaPdOption iaPdOption in iaPdOptions)
                {
                    buf.put(iaPdOption.Encode());
                }
            }
            return (ByteBuffer)buf.flip();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Util.LINE_SEPARATOR);
            sb.Append(DhcpConstants.getV6MessageString(this.GetMessageType()));
            sb.Append(" (xactId=");
            sb.Append(this.GetTransactionId());
            sb.Append(')');
            if (((this.MessageType == DhcpConstants.V6MESSAGE_TYPE_ADVERTISE)
                        || ((this.MessageType == DhcpConstants.V6MESSAGE_TYPE_REPLY)
                        || (this.MessageType == DhcpConstants.V6MESSAGE_TYPE_RECONFIGURE))))
            {
                sb.Append(" to ");
            }
            else
            {
                sb.Append(" from ");
            }

            sb.Append(Util.SocketAddressAsString(this.remoteAddress));
            return sb.ToString();
        }

        public String ToStringWithOptions()
        {
            StringBuilder sb = new StringBuilder(this.ToString());
            if (((this.dhcpOptions != null)
                        && this.dhcpOptions.Count > 0))
            {
                sb.Append(Util.LINE_SEPARATOR);
                sb.Append("MSG_DHCPOPTIONS");
                foreach (DhcpOption dhcpOption in this.dhcpOptions.Values)
                {
                    sb.Append(dhcpOption.ToString());
                }

            }

            if (((this.iaNaOptions != null)
                        && this.iaNaOptions.Count > 0))
            {
                sb.Append(Util.LINE_SEPARATOR);
                sb.Append("IA_NA_OPTIONS");
                foreach (DhcpV6IaNaOption iaNaOption in this.iaNaOptions)
                {
                    sb.Append(iaNaOption.ToString());
                }

            }

            if (((this.iaTaOptions != null)
                        && this.iaTaOptions.Count > 0))
            {
                sb.Append(Util.LINE_SEPARATOR);
                sb.Append("IA_TA_OPTIONS");
                foreach (DhcpV6IaTaOption iaTaOption in this.iaTaOptions)
                {
                    sb.Append(iaTaOption.ToString());
                }

            }

            if (((this.iaPdOptions != null)
                        && this.iaPdOptions.Count > 0))
            {
                sb.Append(Util.LINE_SEPARATOR);
                sb.Append("IA_PD_OPTIONS");
                foreach (DhcpV6IaPdOption iaPdOption in this.iaPdOptions)
                {
                    sb.Append(iaPdOption.ToString());
                }

            }

            return sb.ToString();
        }

        public void AddIaNaOption(DhcpV6IaNaOption iaNaOption)
        {
            if (iaNaOptions == null)
            {
                iaNaOptions = new List<DhcpV6IaNaOption>();
            }
            iaNaOptions.Add(iaNaOption);
        }

    }
}
