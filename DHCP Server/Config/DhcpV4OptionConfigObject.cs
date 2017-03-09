using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Config
{
    public interface DhcpV4OptionConfigObject: DhcpConfigObject
    {
        DhcpV4ConfigOptions GetV4ConfigOptions();
    }
}
