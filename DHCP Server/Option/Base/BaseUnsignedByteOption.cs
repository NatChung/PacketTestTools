using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.Base
{
    public abstract class BaseUnsignedByteOption : BaseDhcpOption, DhcpComparableOption
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private short unsignedByte;

        public BaseUnsignedByteOption()
            : base()
        { }
        public BaseUnsignedByteOption(unsignedByteOptionType uByteOption)
            : base()
        {
            if (uByteOption != null)
            {
                unsignedByte = uByteOption.unsignedByte;
            }
        }

        public short GetUnsignedByte()
        {
            return unsignedByte;
        }
        public void SetUnsignedByte(short unsignedByte)
        {
            this.unsignedByte = unsignedByte;
        }
        public override int GetLength()
        {
            return 1;   // always one bytes 
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Util.LINE_SEPARATOR);
            sb.Append(GetName());
            sb.Append(": unsignedByte=");
            sb.Append(unsignedByte);
            return sb.ToString();
        }

        /* (non-Javadoc)
    * @see com.jagornet.dhcpv6.option.Decodable#decode(java.nio.ByteBuffer)
    */
        public override void Decode(ByteBuffer buf)
        {

            int len = base.DecodeLength(buf);
            if ((len > 0) && (len <= buf.remaining()))
            {
                unsignedByte = Util.GetUnsignedByte(buf);
            }
        }
        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            buf.put((byte)unsignedByte);
            return (ByteBuffer)buf.flip();
        }
        public bool Matches(optionExpression expression)
        {
            return false;
        }

    }
}
