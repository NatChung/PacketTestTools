using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.Base
{
    public abstract class BaseUnsignedIntOption : BaseDhcpOption, DhcpComparableOption
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private long unsignedInt;

        public BaseUnsignedIntOption() : base() { }
        public BaseUnsignedIntOption(unsignedIntOptionType uIntOption)
            : base()
        {
            if (uIntOption != null)
            {
                unsignedInt = uIntOption.unsignedInt;
            }
        }
        public long GetUnsignedInt()
        {
            return unsignedInt;
        }
        public void SetUnsignedInt(long unsigned)
        {
            unsignedInt = unsigned;
        }
        public override int GetLength()
        {
            return 4;   // always four bytes (int) 
        }
        public bool Matches(optionExpression expression)
        {
            throw new NotImplementedException();
        }
        /* (non-Javadoc)
     * @see com.jagornet.dhcpv6.option.Encodable#encode()
     */
        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            buf.putInt((int)unsignedInt);
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
                unsignedInt = Util.GetUnsignedInt(buf);
            }
        }
    }
}
