using System;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V4
{
    public class DhcpV4TimeOffsetOption : BaseUnsignedIntOption
    {
        public DhcpV4TimeOffsetOption() : this(null) { }
        public DhcpV4TimeOffsetOption(v4TimeOffsetOption v4TimeOffsetOption) : base(v4TimeOffsetOption)
        {
            SetCode(DhcpConstants.V4OPTION_TIME_OFFSET);
            SetV4(true);
        }
    }
}