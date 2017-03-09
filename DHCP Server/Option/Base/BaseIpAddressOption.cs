using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option.Base
{
    public class BaseIpAddressOption : BaseDhcpOption
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected String ipAddress;

        public BaseIpAddressOption() : this(null)
        {
        }

        public BaseIpAddressOption(ipAddressOptionType ipAddressOption) : base()
        {
            if (ipAddressOption != null)
            {
                ipAddress = ipAddressOption.ipAddress;
            }
        }

        public string GetIpAddress()
        {
            return ipAddress;
        }

        public void SetIpAddress(string ipAddress)
        {
            this.ipAddress = ipAddress;
        }
        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.DhcpOption#getLength()
         */
        public override int GetLength()
        {
            if (!base.IsV4())
                return 16;      // 128-bit IPv6 address
            else
                return 4;       // 32-bit IPv4 address
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.DhcpComparableOption#matches(com.jagornet.dhcp.xml.OptionExpression)
         */
        public bool Matches(optionExpression expression)
        {
            if (expression == null)
                return false;
            if (expression.code != this.GetCode())
                return false;
            if (ipAddress == null)
                return false;

            ipAddressOptionType exprOption = (ipAddressOptionType)expression.Item;
            if (exprOption != null)
            {
                String exprIpAddress = exprOption.ipAddress;
                @operator op = expression.@operator;
                if (op.Equals(@operator.equals))
                {
                    return ipAddress.Equals(exprIpAddress);
                }
                else if (op.Equals(@operator.startsWith))
                {
                    return ipAddress.StartsWith(exprIpAddress);
                }
                else if (op.Equals(@operator.endsWith))
                {
                    return ipAddress.EndsWith(exprIpAddress);
                }
                else if (op.Equals(@operator.contains))
                {
                    return ipAddress.Contains(exprIpAddress);
                }
                else if (op.Equals(@operator.regExp))
                {
                    Match m = Regex.Match(ipAddress, exprIpAddress);
                    return m.Success;
                }
                else
                {
                    log.Warn("Unsupported expression operator: " + op);
                }
            }

            return false;
        }

        /* (non-Javadoc)
         * @see java.lang.Object#toString()
         */
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Util.LINE_SEPARATOR);
            sb.Append(base.name);
            sb.Append(": ipAddress=");
            sb.Append(ipAddress);
            return sb.ToString();
        }
        /**
     * Convert an IPv6 address received from the wire to a string.
     * 
     * @param buf the ByteBuffer containing the IPv6 address to be decoded from the wire
     * @return the string representation of the IPv6 address
     * @throws IOException
     */
        public static string DecodeIpAddress(ByteBuffer buf)
        {
            // it has to be hex from the wire, right?
            byte[] b = new byte[16];
            buf.get(b, 0, b.Length);
            IPAddress inetAddr = new IPAddress(b);
            return inetAddr.ToString();
        }
        public override void Decode(ByteBuffer buf)
        {
            int len = base.DecodeLength(buf);
            if ((len > 0) && (len <= buf.remaining()))
            {
                if (!base.IsV4())
                {
                    ipAddress = DecodeIpAddress(buf);
                }
                else
                {
                    ipAddress = DecodeIpV4Address(buf);
                }
            }
        }
        /**
     * Convert an IPv4 address received from the wire to a string.
     * 
     * @param buf the ByteBuffer containing the IPv4 address to be decoded from the wire
     * @return the string representation of the IPv4 address
     * @throws IOException
     */
        public static string DecodeIpV4Address(ByteBuffer buf)
        {
            // it has to be hex from the wire, right?
            byte[] b = new byte[4];
            buf.get(b, 0, b.Length);
            IPAddress inetAddr = new IPAddress(b);
            return inetAddr.ToString();
        }

        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            if (ipAddress != null)
            {
                IPAddress inetAddr = null;
                if (!base.IsV4())
                {
                    inetAddr = IPAddress.Parse(ipAddress);
                }
                else
                {
                    inetAddr = IPAddress.Parse(ipAddress);
                }
                buf.put(inetAddr.GetAddressBytes(), 0, inetAddr.GetAddressBytes().Length);
            }
            return (ByteBuffer)buf.flip();
        }
    }
}
