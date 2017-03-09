/**
 * Title:        DhcpV4Message
 * Description:  Object that represents a DHCPv4 message as defined in
 *               RFC 2131.
 *               
 * The following diagram illustrates the format of DHCP messages sent
 * between clients and servers:
 *
 * <pre>
 *   0                   1                   2                   3
 *   0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
 *   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 *   |     op (1)    |   htype (1)   |   hlen (1)    |   hops (1)    |
 *   +---------------+---------------+---------------+---------------+
 *   |                            xid (4)                            |
 *   +-------------------------------+-------------------------------+
 *   |           secs (2)            |           flags (2)           |
 *   +-------------------------------+-------------------------------+
 *   |                          ciaddr  (4)                          |
 *   +---------------------------------------------------------------+
 *   |                          yiaddr  (4)                          |
 *   +---------------------------------------------------------------+
 *   |                          siaddr  (4)                          |
 *   +---------------------------------------------------------------+
 *   |                          giaddr  (4)                          |
 *   +---------------------------------------------------------------+
 *   |                                                               |
 *   |                          chaddr  (16)                         |
 *   |                                                               |
 *   |                                                               |
 *   +---------------------------------------------------------------+
 *   |                                                               |
 *   |                          sname   (64)                         |
 *   +---------------------------------------------------------------+
 *   |                                                               |
 *   |                          file    (128)                        |
 *   +---------------------------------------------------------------+
 *   |                                                               |
 *   |                          options (variable)                   |
 *   +---------------------------------------------------------------+
 * </pre>
 * 
 * @author A. Gregory Rabil
 */
using PIXIS.DHCP.Message;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Option.V4;
using PIXIS.DHCP.Utility;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Linq;
using System.Net.NetworkInformation;

namespace PIXIS.DHCP.Message
{
    public class DhcpV4Message : DhcpMessage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// true if the message was received on a unicast socket, false otherwise 
        /// </summary>
        protected bool unicast;

        /// <summary>
        /// the IP and port on the local host on  
        /// which the message is sent or received 
        /// </summary>
        IPAddress _localAddress;

        /// <summary>
        /// the IP and port on the remote host from
        /// which the message is received or sent
        /// </summary>
        IPEndPoint _remoteAddress;
        private DhcpV4ServerIdOption _dhcpServerIdOption;

        /// <summary>
        /// need a short to hold unsigned byte 
        /// </summary>
        short _op = 0;
        short _htype = 0;
        short _hlen = 0;
        short _hops = 0;

        /// <summary>
        /// // need a long to hold unsigned int 
        /// </summary>
        long _transactionId = 0;
        int _secs = 0;
        int _flags = 0;
        IPAddress _ciAddr = DhcpConstants.ZEROADDR_V4;
        IPAddress _yiAddr = DhcpConstants.ZEROADDR_V4;
        IPAddress _siAddr = DhcpConstants.ZEROADDR_V4;
        IPAddress _giAddr = DhcpConstants.ZEROADDR_V4;

        byte[] _chAddr;
        string _sName = "";
        string _file = "";
        static byte[] _magicCookie = new byte[] { (byte)99, (byte)130, (byte)83, (byte)99 };
        Dictionary<int, DhcpOption> _dhcpOptions = new Dictionary<int, DhcpOption>();

        public DhcpV4Message(IPAddress localAddress, IPEndPoint remoteAddress)
        {
            _localAddress = localAddress;
            _remoteAddress = remoteAddress;
        }

        public void SetUnicast(bool unicast)
        {
            this.unicast = unicast;
        }

        public void SetDhcpV4ServerIdOption(DhcpV4ServerIdOption option)
        {
            if (option != null) _dhcpServerIdOption = option;
        }
        public bool IsUnicast()
        {
            return unicast;
        }

        ///
        ///
        /// <summary>
        /// Return the length of this DhcpMessage in bytes.
        /// </summary>
        /// <returns>an int containing a length of a least four(4)</returns>
        public int GetLength()
        {
            // portion of the DHCPv4 header plus the required
            // "magic cookie", and at least the message type option
            // and the end option: 244 = 236 + 4 + 3 + 1
            int len = 244;
            len += GetOptionsLength();
            return len;
        }

        /// <summary>
        /// Return the length of the options in this DhcpMessage in bytes.
        /// </summary>
        /// <returns>an int containing the total length of all options</returns>
        protected int GetOptionsLength()
        {
            int len = 0;
            if (_dhcpOptions != null)
            {
                foreach (DhcpOption option in _dhcpOptions.Values)
                {
                    len += 2;   // option code (1 byte) + length (1 byte) 
                    len += option.GetLength();
                }
            }
            return len;
        }

        public IPAddress GetLocalAddress()
        {
            return _localAddress;
        }

        public void SetLocalAddress(IPAddress localAddress)
        {
            this._localAddress = localAddress;
        }

        public IPEndPoint GetRemoteAddress()
        {
            return _remoteAddress;
        }

        public void SetRemoteAddress(IPEndPoint remoteAddress)
        {
            this._remoteAddress = remoteAddress;
        }

        public DhcpOption GetDhcpOption(int optionCode)
        {
            return _dhcpOptions.Get(optionCode);
        }

        public void PutDhcpOption(DhcpOption dhcpOption)
        {
            if (dhcpOption != null)
            {
                _dhcpOptions[dhcpOption.GetCode()] = dhcpOption;
            }
        }
        public void PutAllDhcpOptions(Dictionary<int, DhcpOption> dhcpOptions)
        {
            this._dhcpOptions.PutAll(dhcpOptions);
        }

        public Dictionary<int, DhcpOption> GetDhcpOptionMap()
        {
            return _dhcpOptions;
        }
        public void SetDhcpOptionMap(Dictionary<int, DhcpOption> dhcpOptions)
        {
            this._dhcpOptions = dhcpOptions;
        }

        public List<DhcpOption> GetDhcpOptions()
        {
            return _dhcpOptions.Values.ToList();
        }

        public DhcpV4ServerIdOption GetDhcpV4ServerIdOption()
        {
            if (_dhcpServerIdOption == null)
            {
                if (_dhcpOptions != null)
                {
                    _dhcpServerIdOption = (DhcpV4ServerIdOption)_dhcpOptions.Get(DhcpConstants.V4OPTION_SERVERID);
                }
            }
            return _dhcpServerIdOption;
        }

        /// <summary>
        /// Encode this DhcpMessage to wire format for sending.
        /// </summary>
        /// <returns>ByteBuffer containing the encoded DhcpMessage</returns>
        public ByteBuffer Encode()
        {
            if (log.IsDebugEnabled)
                log.Debug("Encoding DhcpMessage for: " + _remoteAddress);

            ByteBuffer buf = ByteBuffer.allocate(1024);
            try
            {
                buf.put((byte)_op);
                buf.put((byte)_htype);
                buf.put((byte)_hlen);
                buf.put((byte)_hops);
                buf.put(BitConverter.GetBytes((int)_transactionId));
                buf.putShort((short)_secs);
                byte[] flagsArray = BitConverter.GetBytes((short)_flags).ToArray();
                buf.put(flagsArray);
                if (_ciAddr != null)
                {
                    buf.put(_ciAddr.GetAddressBytes());
                }
                else
                {
                    buf.put(DhcpConstants.ZEROADDR_V4.GetAddressBytes());
                }
                if (_yiAddr != null)
                {
                    buf.put(_yiAddr.GetAddressBytes());
                }
                else
                {
                    buf.put(DhcpConstants.ZEROADDR_V4.GetAddressBytes());
                }
                if (_siAddr != null)
                {
                    buf.put(_siAddr.GetAddressBytes());
                }
                else
                {
                    buf.put(DhcpConstants.ZEROADDR_V4.GetAddressBytes());
                }
                if (_giAddr != null)
                {
                    buf.put(_giAddr.GetAddressBytes());
                }
                else
                {
                    buf.put(DhcpConstants.ZEROADDR_V4.GetAddressBytes());
                }
                byte[] newchAddr = new byte[16];
                Array.Copy(_chAddr, newchAddr, _chAddr.Length);
                buf.put(newchAddr);     // pad to 16 bytes for encoded packet

                byte[] newsName = new byte[64];
                var sNameBytes = Encoding.ASCII.GetBytes(_sName);
                Array.Copy(sNameBytes, newsName, sNameBytes.Length);
                buf.put(newsName);

                byte[] newsfileBuf = new byte[128];
                var fileBuf = Encoding.ASCII.GetBytes(_file);
                Array.Copy(fileBuf, newsfileBuf, fileBuf.Length);
                buf.put(newsfileBuf);

                buf.put(EncodeOptions());
                long msglen = buf.position();
                if (log.IsDebugEnabled)
                    log.Debug("DHCPv4 Message is " + msglen + " bytes");
                if (msglen < 300)
                {
                    long pad = 300 - msglen;
                    if (log.IsDebugEnabled)
                        log.Debug("Padding with " + pad + " bytes to 300 byte (Bootp) minimum");
                    buf.put(new byte[pad]);
                }
                buf.flip();


                if (log.IsDebugEnabled)
                    log.Debug("DhcpMessage encoded.");


            }
            catch (Exception ex)
            {
                log.Error("DhcpMessageV4 Encoded Error : " + ex.Message);
            }
            return buf;
        }

        /// <summary>
        /// 租期結束時間
        /// </summary>
        public DateTime GetValidEndTime()
        {
            long leaseTime = 0;
            if (!_dhcpOptions.ContainsKey(DhcpConstants.V4OPTION_LEASE_TIME)) leaseTime = 0;
            else leaseTime = ((DhcpV4LeaseTimeOption)_dhcpOptions[DhcpConstants.V4OPTION_LEASE_TIME]).GetUnsignedInt();
            return DateTime.Now.AddSeconds(leaseTime);
        }


        /// <summary>
        /// Encode the options of this DhcpMessage to wire format for sending.
        /// </summary>
        /// <returns>a ByteBuffer containing the encoded options</returns>
        protected ByteBuffer EncodeOptions()
        {
            ByteBuffer buf = ByteBuffer.allocate(788); // 788 - 236 = 1020 (options)
            if (_dhcpOptions != null)
            {
                // magic cookie as per rfc1497
                buf.put(_magicCookie);
                foreach (DhcpOption option in _dhcpOptions.Values)
                {
                    buf.put(option.Encode());
                }
                buf.put((byte)DhcpConstants.V4OPTION_EOF);  // end option
            }
            return (ByteBuffer)buf.flip();
        }

        /// <summary>
        ///  Decode a packet received on the wire into a DhcpMessage object.
        /// </summary>
        /// <param name="buf">ByteBuffer containing the packet to be decoded</param>
        /// <param name="localAddr">InetSocketAddress on the local host on which packet was received </param>
        /// <param name="remoteAddr">InetSocketAddress on the remote host from which the packet was received</param>
        /// <returns>a decoded DhcpMessage object, or null if the packet could not be decoded</returns>
        public static DhcpV4Message Decode(ByteBuffer buf, IPAddress localAddr, IPEndPoint remoteAddr)
        {
            DhcpV4Message dhcpMessage = null;
            try
            {
                if ((buf != null) && buf.hasRemaining())
                {

                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Decoding packet:" + " size=" + buf.limit() + " localAddr=" + localAddr +
                                " remoteAddr=" + remoteAddr);
                    }

                    // we'll "peek" at the message type to use for this mini-factory
                    byte _op = buf.getByte();
                    if (log.IsDebugEnabled)
                        log.Debug("op byte=" + _op);

                    // allow for reply(op=2) messages for use by client
                    // TODO: see if this is a problem for the server
                    if ((_op == 1) || (_op == 2))
                    {
                        dhcpMessage = new DhcpV4Message(localAddr, remoteAddr);
                    }
                    else
                    {
                        log.Error("Unsupported op code: " + _op);
                    }

                    if (dhcpMessage != null)
                    {
                        // reset the buffer to point at the message type byte
                        // because the message decoder will expect it
                        buf.rewind();
                        dhcpMessage.Decode(buf, localAddr);
                    }
                }
                else
                {
                    log.Error("Buffer is null or empty");
                }
            }
            catch (Exception ex)
            {
                log.Error("DhcpMessageV4 Decode Error :" + ex.Message);
            }
            return dhcpMessage;
        }

        /// <summary>
        /// Decode a datagram packet into this DhcpMessage object.
        /// </summary>
        /// <param name="buf">ByteBuffer containing the packet to be decoded</param>
        public void Decode(ByteBuffer buf, IPAddress managementIp)
        {
            if (log.IsDebugEnabled)
                log.Debug("Decoding DhcpV4Message from: " + _remoteAddress);

            if ((buf != null) && buf.hasRemaining())
            {
                // buffer must be at least the size of the fixed
                // portion of the DHCPv4 header plus the required
                // "magic cookie", and at least the message type option
                // and the end option: 244 = 236 + 4 + 3 + 1
                if (buf.limit() >= 244)
                {
                    _op = buf.getByte();
                    log.Debug("op=" + _op);
                    if (_op != 1)
                    {
                        // TODO
                    }
                    _htype = buf.getByte();
                    if (log.IsDebugEnabled)
                        log.Debug("htype=" + _htype);
                    _hlen = buf.getByte();
                    log.Debug("hlen=" + _hlen);
                    _hops = buf.getByte();
                    if (log.IsDebugEnabled)
                        log.Debug("hops=" + _hops);
                    _transactionId = buf.getInt();
                    if (log.IsDebugEnabled)
                        log.Debug("xid=" + _transactionId);
                    _secs = buf.getShort();
                    if (log.IsDebugEnabled)
                        log.Debug("secs=" + _secs);
                    _flags = buf.getShort();
                    if (log.IsDebugEnabled)
                        log.Debug("flags=" + _flags);
                    byte[] ipbuf = buf.getBytes(4);
                    _ciAddr = new IPAddress(ipbuf);
                    if (log.IsDebugEnabled)
                        log.Debug("ciaddr=" + _ciAddr);
                    ipbuf = buf.getBytes(4);
                    _yiAddr = new IPAddress(ipbuf);
                    if (log.IsDebugEnabled)
                        log.Debug("yiaddr=" + _yiAddr);
                    ipbuf = buf.getBytes(4);
                    _siAddr = new IPAddress(ipbuf);
                    if (log.IsDebugEnabled)
                        log.Debug("siaddr=" + _siAddr);
                    ipbuf = buf.getBytes(4);
                    _giAddr = new IPAddress(ipbuf);
                    if (log.IsDebugEnabled)
                        log.Debug("giaddr=" + _giAddr);
                    byte[] chbuf = buf.getBytes(16);
                    _chAddr = chbuf.Take(_hlen).ToArray();
                    if (log.IsDebugEnabled)
                        log.Debug("chaddr=" + Util.ToHexString(_chAddr));
                    byte[] sbuf = buf.getBytes(64);
                    _sName = Encoding.ASCII.GetString(sbuf);
                    if (log.IsDebugEnabled)
                        log.Debug("sname=" + _sName);
                    byte[] fbuf = buf.getBytes(128);
                    _file = Encoding.ASCII.GetString(fbuf);
                    if (log.IsDebugEnabled)
                        log.Debug("file=" + _file);
                    byte[] cookieBuf = buf.getBytes(4);
                    if (!cookieBuf.SequenceEqual(_magicCookie))
                    {
                        String errmsg = "Failed to decode DHCPv4 message: invalid magic cookie";
                        log.Error(errmsg);
                    }
                    DecodeOptions(buf);
                }
                else
                {
                    String errmsg = "Failed to decode DHCPv4 message: packet too short";
                    log.Error(errmsg);
                }
            }
            else
            {
                String errmsg = "Failed to decode message: buffer is empty";
                log.Error(errmsg);
            }

            if (log.IsDebugEnabled)
            {
                log.Debug("DhcpMessage decoded.");
            }
        }

        /**
         * Decode the options.
         * @param buf	ByteBuffer positioned at the start of the options in the packet
         * @return	a Map of DhcpOptions keyed by the option code
         * @throws IOException
         */
        protected Dictionary<int, DhcpOption> DecodeOptions(ByteBuffer buf)
        {
            while (buf.hasRemaining())
            {
                short code = Util.GetUnsignedByte(buf);
                if (log.IsDebugEnabled)
                    log.Debug("Option code=" + code);
                DhcpOption option = DhcpV4OptionFactory.GetDhcpOption(code);
                if (option != null)
                {
                    option.Decode(buf);
                    _dhcpOptions[option.GetCode()] = option;
                }
                else
                {
                    break;  // no more options, or one is malformed, so we're done
                }
            }
            return _dhcpOptions;
        }

        private List<int> requestedOptionCodes;

        public List<int> GetRequestedOptionCodes()
        {
            if (requestedOptionCodes == null)
            {
                if (_dhcpOptions != null)
                {
                    DhcpV4ParamRequestOption pro =
                        (DhcpV4ParamRequestOption)_dhcpOptions.Get(DhcpConstants.V4OPTION_PARAM_REQUEST_LIST);
                    if (pro != null)
                    {
                        requestedOptionCodes = new List<int>();
                        foreach (short ubyte in pro.GetUnsignedByteList())
                        {
                            requestedOptionCodes.Add((int)ubyte);
                        }
                    }
                }
            }
            return requestedOptionCodes;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(DhcpConstants.GetV4MessageString(GetMessageType()));
            if (this.GetOp() == DhcpConstants.V4_OP_REPLY)
                sb.Append(" to ");
            else
                sb.Append(" from ");
            sb.Append(_remoteAddress.ToString());

            sb.Append(" (htype=");
            sb.Append(this.GetHtype());
            sb.Append(", hlen=");
            sb.Append(this.GetHlen());
            sb.Append(", hops=");
            sb.Append(this.GetHops());
            sb.Append(", xid=");
            sb.Append(this.GetTransactionId());
            sb.Append(", secs=");
            sb.Append(this.GetSecs());
            sb.Append(", flags=");
            sb.Append(this.GetFlags());
            sb.Append(", ciaddr=");
            sb.Append(this.GetCiAddr().ToString());
            sb.Append(", yiaddr=");
            sb.Append(this.GetYiAddr().ToString());
            sb.Append(", siaddr=");
            sb.Append(this.GetSiAddr().ToString());
            sb.Append(", giaddr=");
            sb.Append(this.GetGiAddr().ToString());
            sb.Append(", chaddr=");
            sb.Append(Util.ToHexString(this.GetChAddr()));
            sb.Append(')');
            return sb.ToString();
        }

        public string toStringWithOptions()
        {
            StringBuilder sb = new StringBuilder(this.ToString());
            if ((_dhcpOptions != null) && _dhcpOptions.Count > 0)
            {
                sb.Append(Util.LINE_SEPARATOR);
                sb.Append("dhcpOptions");
                foreach (DhcpOption dhcpOption in _dhcpOptions.Values)
                {
                    sb.Append(dhcpOption.ToString());
                }
            }
            return sb.ToString();
        }

        public short GetOp()
        {
            return _op;
        }

        public void SetOp(short op)
        {
            this._op = op;
        }

        public short GetHtype()
        {
            return _htype;
        }

        public void SetHtype(short htype)
        {
            this._htype = htype;
        }

        public short GetHlen()
        {
            return _hlen;
        }

        public void SetHlen(short hlen)
        {
            this._hlen = hlen;
        }

        public short GetHops()
        {
            return _hops;
        }

        public void DetHops(short hops)
        {
            this._hops = hops;
        }

        public long GetTransactionId()
        {
            return _transactionId;
        }

        public void SetTransactionId(long transactionId)
        {
            this._transactionId = transactionId;
        }

        public int GetSecs()
        {
            return _secs;
        }

        public void SetSecs(int secs)
        {
            this._secs = secs;
        }

        public int GetFlags()
        {
            return _flags;
        }

        public void SetFlags(int flags)
        {
            this._flags = flags;
        }

        public IPAddress GetCiAddr()
        {
            return _ciAddr;
        }

        public void SetCiAddr(IPAddress ciAddr)
        {
            this._ciAddr = ciAddr;
        }

        public IPAddress GetYiAddr()
        {
            return _yiAddr;
        }

        public void SetYiAddr(IPAddress yiAddr)
        {
            this._yiAddr = yiAddr;
        }

        public IPAddress GetSiAddr()
        {
            return _siAddr;
        }

        public void SetSiAddr(IPAddress siAddr)
        {
            this._siAddr = siAddr;
        }

        public IPAddress GetGiAddr()
        {
            return _giAddr;
        }

        public void SetGiAddr(IPAddress giAddr)
        {
            this._giAddr = giAddr;
        }

        /// <summary>
        /// 取得 MAC
        /// </summary>
        /// <returns></returns>
        public byte[] GetChAddr()
        {
            return _chAddr;
        }

        /// <summary>
        /// 設定 MAC
        /// </summary>
        /// <returns></returns>
        public void SetChAddr(byte[] chAddr)
        {
            this._chAddr = chAddr;
        }

        public string GetsName()
        {
            return _sName;
        }

        public void SetsName(string sName)
        {
            this._sName = sName;
        }

        public string GetFile()
        {
            return _file;
        }

        public void SetFile(String file)
        {
            this._file = file;
        }

        public void SetMessageType(short msgType)
        {
            DhcpV4MsgTypeOption msgTypeOption = new DhcpV4MsgTypeOption();
            msgTypeOption.SetUnsignedByte(msgType);
            PutDhcpOption(msgTypeOption);
        }

        public short GetMessageType()
        {
            DhcpV4MsgTypeOption msgType = (DhcpV4MsgTypeOption)
                        _dhcpOptions.Get(DhcpConstants.V4OPTION_MESSAGE_TYPE);
            if (msgType != null)
            {
                return msgType.GetUnsignedByte();
            }
            return 0;
        }

        public bool IsBroadcastFlagSet()
        {
            return (this._flags & 0x80) > 0;
        }

        public bool HasOption(int optionCode)
        {
            if (_dhcpOptions.ContainsKey(optionCode))
            {
                return true;
            }
            return false;
        }
    }
}