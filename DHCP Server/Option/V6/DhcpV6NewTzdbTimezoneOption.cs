using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6NewTzdbTimezoneOption: BaseStringOption
    {
        public DhcpV6NewTzdbTimezoneOption(): this(null)
        {
        }

        /**
	     * Instantiates a new dhcp new tzdb timezone option.
	     * 
	     * @param newTzdbTimezoneOption the new tzdb timezone option
	     */
        public DhcpV6NewTzdbTimezoneOption(v6NewTzdbTimezoneOption newTzdbTimezoneOption):base(newTzdbTimezoneOption)
        {
            SetCode(DhcpConstants.V6OPTION_NEW_TZDB_TIMEZONE);
        }
    }
}