using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6InfoRefreshTimeOption : BaseUnsignedIntOption
    {
        /**
      * Instantiates a new dhcp info refresh time option.
      */
        public DhcpV6InfoRefreshTimeOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp info refresh time option.
         * 
         * @param infoRefreshTimeOption the info refresh time option
         */
        public DhcpV6InfoRefreshTimeOption(v6InfoRefreshTimeOption infoRefreshTimeOption):base(infoRefreshTimeOption)
        {
            SetCode(DhcpConstants.V6OPTION_INFO_REFRESH_TIME);
        }
    }
}