using System;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V4
{
    public class DhcpV4ParamRequestOption : BaseUnsignedByteListOption
    {
        public DhcpV4ParamRequestOption(): this(null)
        {
        }

        /**
	 * Instantiates a new dhcp v4 param request option.
	 * 
	 * @param optionRequestOption the v4 param request option
	 */
        public DhcpV4ParamRequestOption(v4ParamRequestOption paramRequestOption):base(paramRequestOption)
        {
            SetCode(DhcpConstants.V4OPTION_PARAM_REQUEST_LIST);
            SetV4(true);
        }
    }
}