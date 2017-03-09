using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option.Base
{
    public class DhcpV6BcmcsAddressesOption: BaseIpAddressListOption
    {
        /**
	     * Instantiates a new dhcp bcmcs addresses option.
	     */
        public DhcpV6BcmcsAddressesOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp bcmcs addresses option.
         * 
         * @param bcmcsAddressesOption the bcmcs addresses option
         */
        public DhcpV6BcmcsAddressesOption(v6BcmcsAddressesOption bcmcsAddressesOption):base(bcmcsAddressesOption)
        {
            SetCode(DhcpConstants.V6OPTION_BCMCS_ADDRESSES);
        }
    }
}
