using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6ServerIdOption : BaseOpaqueDataOption
    {
        public DhcpV6ServerIdOption() : this(null)
        {
        }
        public DhcpV6ServerIdOption(v6ServerIdOption serverIdOption)
            : base(serverIdOption)
        {
            SetCode(DhcpConstants.V6OPTION_SERVERID);
        }
    }
}
