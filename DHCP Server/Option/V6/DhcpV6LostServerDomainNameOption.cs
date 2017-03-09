using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6LostServerDomainNameOption : BaseDomainNameOption
    {
        /**
       * Instantiates a new dhcp lost server domain name option.
       */
        public DhcpV6LostServerDomainNameOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp lost server domain name option.
         * 
         * @param lostServerDomainNameOption the lost server domain name option
         */
        public DhcpV6LostServerDomainNameOption(v6LostServerDomainNameOption lostServerDomainNameOption):base(lostServerDomainNameOption)
        {
            SetCode(DhcpConstants.V6OPTION_LOST_SERVER_DOMAIN_NAME);
        }
    }
}