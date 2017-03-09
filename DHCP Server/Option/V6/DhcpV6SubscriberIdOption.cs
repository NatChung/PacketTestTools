using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6SubscriberIdOption : BaseOpaqueDataOption
    {
        /**
	 * Instantiates a new dhcp subscriber id option.
	 */
        public DhcpV6SubscriberIdOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp subscriber id option.
         * 
         * @param subscriberIdOption the subscriber id option
         */
        public DhcpV6SubscriberIdOption(v6SubscriberIdOption subscriberIdOption) : base(subscriberIdOption)
        {
            SetCode(DhcpConstants.V6OPTION_SUBSCRIBER_ID);
        }
    }
}