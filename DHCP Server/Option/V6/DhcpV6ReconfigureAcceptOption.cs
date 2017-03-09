using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6ReconfigureAcceptOption : BaseEmptyOption
    {
        /**
	 * Instantiates a new dhcp reconfigure accept option.
	 */
        public DhcpV6ReconfigureAcceptOption():base()
        {
            SetCode(DhcpConstants.V6OPTION_RECONF_ACCEPT);
        }
    }
}