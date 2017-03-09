using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6EchoRequestOption : BaseUnsignedShortListOption
    {
        /**
	 * Instantiates a new dhcp echo request option.
	 */
        public DhcpV6EchoRequestOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp echo request option.
         * 
         * @param echoRequestOption the echo request option
         */
        public DhcpV6EchoRequestOption(v6EchoRequestOption echoRequestOption) : base(echoRequestOption)
        {
            SetCode(DhcpConstants.V6OPTION_ECHO_REQUEST);
        }
    }
}