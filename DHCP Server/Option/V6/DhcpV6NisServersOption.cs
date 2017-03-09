using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6NisServersOption : BaseIpAddressListOption
    {
        /**
        * Instantiates a new dhcp nis servers option.
        */
        public DhcpV6NisServersOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp nis servers option.
         * 
         * @param nisServersOption the nis servers option
         */
        public DhcpV6NisServersOption(v6NisServersOption nisServersOption) : base(nisServersOption)
        {
            SetCode(DhcpConstants.V6OPTION_NIS_SERVERS);
        }
    }
}