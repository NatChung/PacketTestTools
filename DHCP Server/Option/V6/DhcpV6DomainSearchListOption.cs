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
    public class DhcpV6DomainSearchListOption : BaseDomainNameListOption
    {
        /**
	     * Instantiates a new dhcp domain search list option.
	     */
        public DhcpV6DomainSearchListOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp domain search list option.
         * 
         * @param domainSearchListOption the domain search list option
         */
        public DhcpV6DomainSearchListOption(v6DomainSearchListOption domainSearchListOption) : base()
        {
            SetCode(DhcpConstants.V6OPTION_DOMAIN_SEARCH_LIST);
        }
    }
}
