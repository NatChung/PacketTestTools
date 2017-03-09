using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6NisPlusDomainNameOption : BaseDomainNameOption
    {
        /**
	 * Instantiates a new dhcp nis plus domain name option.
	 */
        public DhcpV6NisPlusDomainNameOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp nis plus domain name option.
         * 
         * @param nisPlusDomainNameOption the nis plus domain name option
         */
        public DhcpV6NisPlusDomainNameOption(v6NisPlusDomainNameOption nisPlusDomainNameOption) : base(nisPlusDomainNameOption)
        {
            SetCode(DhcpConstants.V6OPTION_NISPLUS_DOMAIN_NAME);
        }
    }
}