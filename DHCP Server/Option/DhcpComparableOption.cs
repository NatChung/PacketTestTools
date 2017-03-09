using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option
{
    public interface DhcpComparableOption
    {
         bool Matches(optionExpression expression);
    }
}
