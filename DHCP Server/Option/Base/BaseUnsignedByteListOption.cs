using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace PIXIS.DHCP.Option
{
    public abstract class BaseUnsignedByteListOption : BaseDhcpOption, DhcpComparableOption
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected List<short> unsignedByteList;

        /**
        * Instantiates a new unsigned short list option.
        */
        public BaseUnsignedByteListOption() : this(null)
        {
        }

        /**
         * Instantiates a new unsigned short list option.
         * 
         * @param uByteListOption the option request option
         */
        public BaseUnsignedByteListOption(unsignedByteListOptionType uByteListOption) : base()
        {
            if (uByteListOption != null)
            {
                if (uByteListOption.unsignedByte != null)
                {
                    unsignedByteList = uByteListOption.unsignedByte;
                }
            }
        }

        public List<short> GetUnsignedByteList()
        {
            return unsignedByteList;
        }

        public void SetUnsignedByteList(List<short> unsignedBytes)
        {
            this.unsignedByteList = unsignedBytes;
        }

        public void AddUnsignedByte(short ubyte)
        {
            if (unsignedByteList == null)
            {
                unsignedByteList = new List<short>();
            }
            unsignedByteList.Add(ubyte);
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.DhcpOption#getLength()
         */
        public override int GetLength()
        {
            int len = 0;
            if ((unsignedByteList != null) && unsignedByteList.Count > 0)
            {
                len = unsignedByteList.Count;
            }
            return len;
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
            if (unsignedByteList == null)
                return false;

            unsignedByteListOptionType exprOption = (unsignedByteListOptionType)expression.Item;
            if (exprOption != null)
            {
                List<short> exprUbytes = exprOption.unsignedByte;
                @operator op = expression.@operator;
                if (op.Equals(@operator.equals))
                {
                    return unsignedByteList.Equals(exprUbytes);
                }
                else if (op.Equals(@operator.contains))
                {
                    foreach (var item in exprUbytes)
                    {
                        if (!unsignedByteList.Contains(item))
                            return false;
                    }
                    return true;
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
            sb.Append(": unsignedByteList=");
            if ((unsignedByteList != null) && unsignedByteList.Count > 0)
            {
                foreach (short ubyte in unsignedByteList)
                {
                    sb.Append(ubyte);
                    sb.Append(',');
                }
            }
            return sb.ToString().Trim(',');
        }
        /* (non-Javadoc)
     * @see com.jagornet.dhcpv6.option.Encodable#encode()
     */
        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            if ((unsignedByteList != null) && unsignedByteList.Count > 0)
            {
                foreach (short ubyte in unsignedByteList)
                {
                    buf.put((byte)ubyte);
                }
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
                for (int i = 0; i < len; i++)
                {
                    if (buf.hasRemaining())
                    {

                        AddUnsignedByte(Util.GetUnsignedByte(buf));
                    }
                }
            }
        }
    }
}