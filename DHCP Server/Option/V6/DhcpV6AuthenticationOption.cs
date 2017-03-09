using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using System.Text;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6AuthenticationOption : BaseDhcpOption
    {
        private short protocol;
        private short algorithm;
        private short rdm;
        private BigInteger replayDetection;
        private BaseOpaqueData authInfo;

        /**
         * Instantiates a new dhcp authentication option.
         */
        public DhcpV6AuthenticationOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp authentication option.
         * 
         * @param authenticationOption the authentication option
         */
        public DhcpV6AuthenticationOption(v6AuthenticationOption authenticationOption) : base()
        {
            if (authenticationOption != null)
            {
                protocol = authenticationOption.protocol;
                algorithm = authenticationOption.algorithm;
                rdm = authenticationOption.rdm;
                replayDetection = authenticationOption.replayDetection;
                authInfo = new BaseOpaqueData(authenticationOption.authInfo);
            }
            SetCode(DhcpConstants.V6OPTION_AUTH);
        }

        public short FetProtocol()
        {
            return protocol;
        }

        public void SetProtocol(short protocol)
        {
            this.protocol = protocol;
        }

        public short GetAlgorithm()
        {
            return algorithm;
        }

        public void SetAlgorithm(short algorithm)
        {
            this.algorithm = algorithm;
        }

        public short GetRdm()
        {
            return rdm;
        }

        public void SetRdm(short rdm)
        {
            this.rdm = rdm;
        }

        public BigInteger GetReplayDetection()
        {
            return replayDetection;
        }

        public void SetReplayDetection(BigInteger replayDetection)
        {
            this.replayDetection = replayDetection;
        }

        public BaseOpaqueData GetAuthInfo()
        {
            return authInfo;
        }

        public void SetAuthInfo(BaseOpaqueData authInfo)
        {
            this.authInfo = authInfo;
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.DhcpOption#getLength()
         */
        public override int GetLength()
        {
            int len = 3 + 8;    // size of protocol + algorithm + rdm + replayDetection
            if (authInfo != null)
            {
                len += authInfo.GetLength();
            }
            return len;
        }

        /* (non-Javadoc)
     * @see com.jagornet.dhcpv6.option.Encodable#encode()
     */
        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            buf.put((byte)algorithm);
            buf.put((byte)protocol);
            buf.put((byte)rdm);
            buf.put(replayDetection.GetBytes());
            if (authInfo != null)
            {
                authInfo.Encode(buf);
            }
            return (ByteBuffer)buf.flip();
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.Decodable#decode(java.nio.ByteBuffer)
         */
        public override void Decode(ByteBuffer buf)
        {
            int len = base.DecodeLength(buf);
            if ((len > 0) && (len <= buf.remaining()))
            {
                long eof = buf.position() + len;
                if (buf.position() < eof)
                {
                    algorithm = Util.GetUnsignedByte(buf);
                    if (buf.position() < eof)
                    {
                        protocol = Util.GetUnsignedByte(buf);
                        if (buf.position() < eof)
                        {
                            rdm = Util.GetUnsignedByte(buf);
                            if (buf.position() < eof)
                            {
                                replayDetection = Util.GetUnsignedByte(buf);
                                if (buf.position() < eof)
                                {
                                    authInfo.Decode(buf, len - 8 - 3);
                                }
                            }
                        }
                    }
                }
            }
        }

        /* (non-Javadoc)
         * @see java.lang.Object#toString()
         */
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Util.LINE_SEPARATOR);
            sb.Append(base.GetName());
            sb.Append(": protocol=");
            sb.Append(protocol);
            sb.Append(" algorithm=");
            sb.Append(algorithm);
            sb.Append(" rdm=");
            sb.Append(rdm);
            sb.Append(" replayDetection=");
            sb.Append(replayDetection);
            sb.Append(" authInfo=");
            sb.Append(authInfo);
            return sb.ToString();
        }

    }
}