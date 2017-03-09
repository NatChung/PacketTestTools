using System;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V4
{
    public class DhcpV4MsgTypeOption : BaseUnsignedByteOption
    {
        public DhcpV4MsgTypeOption() : this(null)
        { }

        public DhcpV4MsgTypeOption(v4MsgTypeOption v4MsgTypeOption): base(v4MsgTypeOption)
        {
            SetCode(DhcpConstants.V4OPTION_MESSAGE_TYPE);
            SetV4(true);
        }
    }
}