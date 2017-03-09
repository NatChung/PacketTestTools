using System;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V4
{
    public class DhcpV4BootFileNameOption : BaseStringOption
    {
        public DhcpV4BootFileNameOption() : this(null)
        { }
        public DhcpV4BootFileNameOption(v4BootFileNameOption v4BootFileNameOption)
            : base(v4BootFileNameOption)
        {
            SetCode(DhcpConstants.V4OPTION_BOOT_FILE_NAME);
            SetV4(true);
        }
    }
}