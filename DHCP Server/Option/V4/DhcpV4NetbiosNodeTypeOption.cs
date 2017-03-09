using System;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V4
{
    public class DhcpV4NetbiosNodeTypeOption : BaseUnsignedByteOption
    {
        /**
      * Instantiates a new dhcpv4 netbios node type option.
      */
        public DhcpV4NetbiosNodeTypeOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcpv4 netbios node type option.
         * 
         * @param v4NetbiosNodeTypeOption the v4 netbios node type option
         */
        public DhcpV4NetbiosNodeTypeOption(v4NetbiosNodeTypeOption v4NetbiosNodeTypeOption) : base(v4NetbiosNodeTypeOption)
        {
            SetCode(DhcpConstants.V4OPTION_NETBIOS_NODE_TYPE);
            SetV4(true);
        }
    }
}