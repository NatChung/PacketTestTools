using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6ElapsedTimeOption : BaseUnsignedShortOption
    {
        /* Instantiates a new dhcp elapsed time option.
	    */
        public DhcpV6ElapsedTimeOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp elapsed time option.
         * 
         * @param elapsedTimeOption the elapsed time option
         */
        public DhcpV6ElapsedTimeOption(v6ElapsedTimeOption elapsedTimeOption):base(elapsedTimeOption)
        {
            SetCode(DhcpConstants.V6OPTION_ELAPSED_TIME);
        }
    }
}