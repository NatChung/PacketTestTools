using System;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V4
{
    internal class DhcpV4TftpServerNameOption: BaseStringOption
    {
        public DhcpV4TftpServerNameOption() : this(null)
        {
        }
        public DhcpV4TftpServerNameOption(v4TftpServerNameOption v4TftpServerNameOption)
            :base(v4TftpServerNameOption)
        {
            SetCode(DhcpConstants.V4OPTION_TFTP_SERVER_NAME);
            SetV4(true);
        }
    }
}