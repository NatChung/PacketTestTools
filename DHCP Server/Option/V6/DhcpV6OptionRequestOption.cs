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
    public class DhcpV6OptionRequestOption: BaseUnsignedShortListOption
    {
        public DhcpV6OptionRequestOption():this(null)
        {
        }

        /**
         * Instantiates a new dhcp option request option.
         * 
         * @param optionRequestOption the option request option
         */
        public DhcpV6OptionRequestOption(v6OptionRequestOption optionRequestOption):base(optionRequestOption)
        {
            SetCode(DhcpConstants.V6OPTION_ORO);
        }
    }
}
