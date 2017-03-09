using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Config
{
    public interface DhcpConfigObject
    {
        List<filter> GetFilters();
        List<Policy> GetPolicies();
        long GetPreferredLifetime();
        long GetValidLifetime();
        long GetPreferredLifetimeMs();
        long GetValidLifetimeMs();
    }
}
