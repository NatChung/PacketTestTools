using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option
{
    public class DhcpUnknownOption: BaseOpaqueDataOption
    {
        public DhcpUnknownOption() : base() { }
        public DhcpUnknownOption(opaqueDataOptionType unknownOption) : base(unknownOption) { }
    }
}
