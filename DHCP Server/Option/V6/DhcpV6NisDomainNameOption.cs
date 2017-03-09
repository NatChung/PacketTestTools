using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6NisDomainNameOption : BaseDomainNameOption
    {
        /**
          * Instantiates a new dhcp nis domain name option.
         */
        public DhcpV6NisDomainNameOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp nis domain name option.
         * 
         * @param nisDomainNameOption the nis domain name option
         */
        public DhcpV6NisDomainNameOption(v6NisDomainNameOption nisDomainNameOption):base(nisDomainNameOption)
        {
            SetCode(DhcpConstants.V6OPTION_NIS_DOMAIN_NAME);
        }
    }
}