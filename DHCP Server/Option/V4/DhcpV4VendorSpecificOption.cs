using System;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V4
{
    public class DhcpV4VendorSpecificOption : BaseOpaqueDataOption
    {
        public DhcpV4VendorSpecificOption() : this(null)
        { }
        public DhcpV4VendorSpecificOption(v4VendorSpecificOption v4VendorSpecificOption)
            : base(v4VendorSpecificOption)
        {
            SetCode(DhcpConstants.V4OPTION_VENDOR_INFO);
            SetV4(true);
        }
    }
}