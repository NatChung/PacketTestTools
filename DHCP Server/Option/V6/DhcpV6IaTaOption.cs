using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIXIS.DHCP.Option.Base;

using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6IaTaOption : BaseDhcpOption
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        protected long iaId;

        /** The dhcp options inside this ia ta option, _NOT_ including any ia addr options. */
        protected Dictionary<int, DhcpOption> dhcpOptions = new Dictionary<int, DhcpOption>();

        /** The ia addr options. */
        private List<DhcpV6IaAddrOption> iaAddrOptions = new List<DhcpV6IaAddrOption>();

        /**
         * Instantiates a new dhcp ia ta option.
         */
        public DhcpV6IaTaOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp ia ta option.
         * 
         * @param iaTaOption the ia ta option
         */
        public DhcpV6IaTaOption(v6IaNaOption iaTaOption) : base()
        {
            if (iaTaOption != null)
            {
                iaId = iaTaOption.iaId;
            }
            SetCode(DhcpConstants.V6OPTION_IA_TA);
        }

        public long GetIaId()
        {
            return iaId;
        }

        public void SetIaId(long iaId)
        {
            this.iaId = iaId;
        }

        /**
         * Gets the dhcp option map.
         * 
         * @return the dhcp option map
         */
        public Dictionary<int, DhcpOption> GetDhcpOptionMap()
        {
            return dhcpOptions;
        }

        /**
         * Sets the dhcp option map.
         * 
         * @param dhcpOptions the dhcp options
         */
        public void SetDhcpOptionMap(Dictionary<int, DhcpOption> dhcpOptions)
        {
            this.dhcpOptions = dhcpOptions;
        }

        /**
         * Put all dhcp options.
         * 
         * @param dhcpOptions the dhcp options
         */
        public void PutAllDhcpOptions(Dictionary<int, DhcpOption> dhcpOptions)
        {
            foreach (var option in dhcpOptions)
                this.dhcpOptions[option.Key] = option.Value;
        }

        /**
         * Implement DhcpOptionable.
         * 
         * @param dhcpOption the dhcp option
         */
        public void PutDhcpOption(DhcpOption dhcpOption)
        {
            dhcpOptions[dhcpOption.GetCode()] = dhcpOption;
        }

        /**
         * Gets the ia addr options.
         * 
         * @return the ia addr options
         */
        public List<DhcpV6IaAddrOption> GetIaAddrOptions()
        {
            return iaAddrOptions;
        }

        /**
         * Sets the ia addr options.
         * 
         * @param iaAddrOptions the new ia addr options
         */
        public void SetIaAddrOptions(List<DhcpV6IaAddrOption> iaAddrOptions)
        {
            this.iaAddrOptions = iaAddrOptions;
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.DhcpOption#getLength()
         */
        public override int GetLength()
        {
            return GetDecodedLength();
        }

        /**
         * Gets the decoded length.
         * 
         * @return the decoded length
         */
        public int GetDecodedLength()
        {
            int len = 4;    // iaId
            if (iaAddrOptions != null)
            {
                foreach (DhcpV6IaAddrOption iaAddrOption in iaAddrOptions)
                {
                    // code(short) + len(short) + data_len
                    len += 4 + iaAddrOption.GetDecodedLength();
                }
            }
            if (dhcpOptions != null)
            {
                foreach (DhcpOption dhcpOption in dhcpOptions.Values)
                {
                    // code(short) + len(short) + data_len
                    len += 4 + dhcpOption.GetLength();
                }
            }
            return len;
        }

        /* (non-Javadoc)
         * @see java.lang.Object#toString()
         */
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Util.LINE_SEPARATOR);
            sb.Append(base.GetName());
            sb.Append(": iaId=");
            sb.Append(iaId);
            if ((dhcpOptions != null) && dhcpOptions.Count > 0)
            {
                sb.Append(Util.LINE_SEPARATOR);
                sb.Append("IA_DHCPOPTIONS");
                foreach (DhcpOption dhcpOption in dhcpOptions.Values)
                {
                    sb.Append(dhcpOption.ToString());
                }
            }
            if ((iaAddrOptions != null) && iaAddrOptions.Count > 0)
            {
                sb.Append(Util.LINE_SEPARATOR);
                sb.Append("IA_ADDRS");
                sb.Append(Util.LINE_SEPARATOR);
                foreach (DhcpV6IaAddrOption iaAddrOption in iaAddrOptions)
                {
                    sb.Append(iaAddrOption.ToString());
                }
            }
            return sb.ToString();
        }

        public override void Decode(ByteBuffer buf)
        {
            if ((buf != null) && buf.hasRemaining())
            {
                // already have the code, so length is next
                int len = Util.GetUnsignedShort(buf);
                if (log.IsDebugEnabled)
                    log.Debug("IA_NA option reports length=" + len +
                              ":  bytes remaining in buffer=" + buf.remaining());
                long eof = buf.position() + len;
                if (buf.position() < eof)
                {
                    iaId = Util.GetUnsignedInt(buf);
                    if (buf.position() < eof)
                    {
                        DecodeOptions(buf, eof);
                    }
                }
            }
        }

        /// <summary>
        /// Decode any options sent by the client inside this IA_NA.  Mostly, we are
        /// a hint to which address(es) it may want.RFC 3315 does not specify if
        /// a client can actually provide any options other than IA_ADDR options in
        /// inside the IA_NA, but it does not say that the client cannot do so, and
        /// the IA_NA option definition supports any type of sub-options.
        /// </summary>
        /// <param name="buf">buf ByteBuffer positioned at the start of the options in the packet</param>
        /// <param name="eof">eof the eof</param>
        protected void DecodeOptions(ByteBuffer buf, long eof)
        {
            while (buf.position() < eof)
            {
                int code = Util.GetUnsignedShort(buf);
                log.Debug("Option code=" + code);
                DhcpOption option = DhcpV6OptionFactory.GetDhcpOption(code);
                if (option != null)
                {
                    option.Decode(buf);
                    if (option is DhcpV6IaAddrOption)
                    {
                        iaAddrOptions.Add((DhcpV6IaAddrOption)option);
                    }
                    else
                    {
                        PutDhcpOption(option);
                    }
                }
                else
                {
                    break;  // no more options, or one is malformed, so we're done
                }
            }
        }

        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            buf.putInt((int)iaId);

            if (iaAddrOptions != null)
            {
                foreach (DhcpV6IaAddrOption iaAddrOption in iaAddrOptions)
                {
                    ByteBuffer _buf = iaAddrOption.Encode();
                    if (_buf != null)
                    {
                        buf.put(_buf);
                    }
                }
            }
            if (dhcpOptions != null)
            {
                foreach (DhcpOption dhcpOption in dhcpOptions.Values)
                {
                    ByteBuffer _buf = dhcpOption.Encode();
                    if (_buf != null)
                    {
                        buf.put(_buf);
                    }
                }
            }
            return (ByteBuffer)buf.flip();
        }
    }
}
