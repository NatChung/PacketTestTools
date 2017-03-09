using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6PreferenceOption : BaseUnsignedByteOption
    {
        /**
       * Instantiates a new dhcp preference option.
       */
        public DhcpV6PreferenceOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp preference option.
         * 
         * @param preferenceOption the preference option
         */
        public DhcpV6PreferenceOption(v6PreferenceOption preferenceOption):base(preferenceOption)
        {
            SetCode(DhcpConstants.V6OPTION_PREFERENCE);
        }
    }
}