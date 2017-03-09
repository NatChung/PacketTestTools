using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6NisPlusServersOption : BaseIpAddressListOption
    {
        /**
      * Instantiates a new dhcp nis plus domain name option.
      */
        public DhcpV6NisPlusServersOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp nis plus domain name option.
         * 
         * @param nisPlusDomainNameOption the nis plus domain name option
         */
        public DhcpV6NisPlusServersOption(v6NisPlusServersOption nisPlusServersOption) : base(nisPlusServersOption)
        {
            SetCode(DhcpConstants.V6OPTION_NISPLUS_SERVERS);
        }
    }
}