using System;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using System.Text;

namespace PIXIS.DHCP.Option.V4
{
    /**
     * <p>Title: DhcpV4ClientFqdnOption </p>
     * <p>Description: </p>.
     * 
     * @author A. Gregory Rabil
     */
    public class DhcpV4ClientFqdnOption : BaseDomainNameOption
    {
        /**
	     * From RFC 4702:
	     * 
	     * 2.1.  The Flags Field
	     *
	     *   The format of the 1-octet Flags field is:
	     *
	     *        0 1 2 3 4 5 6 7
	     *       +-+-+-+-+-+-+-+-+
	     *       |  MBZ  |N|E|O|S|
	     *       +-+-+-+-+-+-+-+-+
	     *       
	     * 2.2.  The RCODE Fields
	     * 
	     *    The two 1-octet RCODE1 and RCODE2 fields are deprecated.  A client
	     *    SHOULD set these to 0 when sending the option and SHOULD ignore them
	     *    on receipt.  A server SHOULD set these to 255 when sending the option
	     *    and MUST ignore them on receipt.
	     * 
	     */
        // need short to handle unsigned byte
        private short flags;
        private short rcode1;
        private short rcode2;

        /**
         * Instantiates a new dhcp client fqdn option.
         */
        public DhcpV4ClientFqdnOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp client fqdn option.
         * 
         * @param clientFqdnOption the client fqdn option
         */
        public DhcpV4ClientFqdnOption(v4ClientFqdnOption clientFqdnOption) : base(clientFqdnOption)
        {
            SetCode(DhcpConstants.V4OPTION_CLIENT_FQDN);
            SetV4(true);
        }

        public short GetFlags()
        {
            return flags;
        }

        public void SetFlags(short flags)
        {
            this.flags = flags;
        }

        public short GetRcode1()
        {
            return rcode1;
        }

        public void SetRcode1(short rcode1)
        {
            this.rcode1 = rcode1;
        }

        public short GetRcode2()
        {
            return rcode2;
        }

        public void SetRcode2(short rcode2)
        {
            this.rcode2 = rcode2;
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.DhcpOption#getLength()
         */
        public override int GetLength()
        {
            int len = 3;    // size of flags (byte) + rcode1 (byte) + rcode2 (byte)
            if (GetEncodingBit())
            {
                len += base.GetLength();
            }
            else if (GetDomainName() != null)
            {
                // ASCII encoding, just add length of domain name string
                len += GetDomainName().Length;
            }
            return len;
        }

        /**
         * Get the S bit.
         * 
         * @return the update aaaa bit
         */
        public bool GetUpdateABit()
        {
            short sbit = (short)(GetFlags() & 0x01);
            return (sbit > 0);
        }

        /**
         * Set the S bit.
         * 
         * @param bit the bit
         */
        public void SetUpdateABit(bool bit)
        {
            if (bit)
                SetFlags((short)(GetFlags() | 0x01));   // 0001
            else
                SetFlags((short)(GetFlags() & 0x0e));   // 1110
        }

        /**
         * Get the O bit.
         * 
         * @return the override bit
         */
        public bool GetOverrideBit()
        {
            short obit = (short)(GetFlags() & 0x02);
            return (obit > 0);
        }

        /**
         * Set the O bit.
         * 
         * @param bit the bit
         */
        public void SetOverrideBit(bool bit)
        {
            if (bit)
                SetFlags((short)(GetFlags() | 0x02));   // 0010
            else
                SetFlags((short)(GetFlags() & 0x0d));   // 1101
        }

        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            buf.put((byte)GetFlags());
            buf.put((byte)GetRcode1());
            buf.put((byte)GetRcode2());
            if (domainName != null)
            {
                if (GetEncodingBit())
                {

                    EncodeDomainName(buf, domainName);
                }
                else
                {
                    // ASCII encoding, just append the domain name string
                    buf.put(Encoding.ASCII.GetBytes(domainName));
                }
            }
            return (ByteBuffer)buf.flip();
        }

        public override void Decode(ByteBuffer buf)
        {

            int len = base.DecodeLength(buf);
            if ((len > 0) && (len <= buf.remaining()))
            {
                long eof = buf.position() + len;
                if (buf.position() < eof)
                {
                    SetFlags(Util.GetUnsignedByte(buf));
                    SetRcode1(Util.GetUnsignedByte(buf));
                    SetRcode2(Util.GetUnsignedByte(buf));
                    String domain = null;
                    if (GetEncodingBit())
                    {
                        domain = DecodeDomainName(buf, eof);
                    }
                    else
                    {
                        // ASCII encoding (deprecated, but used by Microsoft)
                        byte[] b = buf.getBytes(len - 3);
                        domain = Encoding.ASCII.GetString(b);
                    }
                    SetDomainName(domain);
                }
            }
        }

        /**
         * Get the E bit.
         * 
         * @return the encoding bit
         */
        public bool GetEncodingBit()
        {
            short obit = (short)(GetFlags() & 0x04);
            return (obit > 0);
        }

        /**
         * Set the E bit.
         * 
         * @param bit the bit
         */
        public void SetEncodingBit(bool bit)
        {
            if (bit)
                SetFlags((short)(GetFlags() | 0x04));   // 0100
            else
                SetFlags((short)(GetFlags() & 0x0b));   // 1011
        }

        /**
         * Get the N bit.
         * 
         * @return the no update bit
         */
        public bool GetNoUpdateBit()
        {
            short nbit = (short)(GetFlags() & 0x08);
            return (nbit == 1);
        }

        /**
         * Set the N bit.  If set to true, will also set the S bit to 0.
         * 
         * @param bit the bit
         */
        public void SetNoUpdateBit(bool bit)
        {
            if (bit)
            {
                SetFlags((short)(GetFlags() | 0x08));   // 1000
                                                        // If the "N" bit is 1, the "S" bit MUST be 0.
                SetUpdateABit(false);
            }
            else
            {
                SetFlags((short)(GetFlags() & 0x07));   // 0111
            }
        }
        /* (non-Javadoc)
         * @see java.lang.Object#toString()
         */
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());
            sb.Append(" flags=");
            sb.Append(flags);
            sb.Append(" rcode1=");
            sb.Append(rcode1);
            sb.Append(" rcode2=");
            sb.Append(rcode2);
            return sb.ToString();
        }
    }
}