using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6BcmcsDomainNamesOption : BaseDomainNameListOption
    {
        /**
	 * Instantiates a new dhcp bcmcs domain names option.
	 */
        public DhcpV6BcmcsDomainNamesOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp bcmcs domain names option.
         * 
         * @param bcmcsDomainNamesOption the bcmcs domain names option
         */
        public DhcpV6BcmcsDomainNamesOption(v6BcmcsDomainNamesOption bcmcsDomainNamesOption):base(bcmcsDomainNamesOption)
        {
            SetCode(DhcpConstants.V6OPTION_BCMCS_DOMAIN_NAMES);
        }
    }
}
