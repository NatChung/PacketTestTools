using PIXIS.DHCP.Option.V6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Config
{
    public interface DhcpV6OptionConfigObject : DhcpConfigObject
    {
        DhcpV6ConfigOptions GetDhcpConfigOptions();
    }
}
