using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6SntpServersOption : BaseIpAddressListOption
    {
        /**
      * Instantiates a new dhcp sntp servers option.
      */
        public DhcpV6SntpServersOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp sntp servers option.
         * 
         * @param sntpServersOption the sntp servers option
         */
        public DhcpV6SntpServersOption(v6SntpServersOption sntpServersOption) : base(sntpServersOption)
        {
            SetCode(DhcpConstants.V6OPTION_SNTP_SERVERS);
        }
    }
}