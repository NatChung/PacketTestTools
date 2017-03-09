using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6ReconfigureMessageOption : BaseUnsignedByteOption
    {
        /**
	     * Instantiates a new dhcp reconfigure message option.
	     */
        public DhcpV6ReconfigureMessageOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp reconfigure message option.
         * 
         * @param reconfigureMessageOption the reconfigure message option
         */
        public DhcpV6ReconfigureMessageOption(v6ReconfigureMessageOption reconfigureMessageOption) : base(reconfigureMessageOption)
        {
            SetCode(DhcpConstants.V6OPTION_RECONF_MSG);
        }
    }
}