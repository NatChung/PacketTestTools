using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6NewPosixTimezoneOption : BaseStringOption
    {
        /**
         * Instantiates a new dhcp new posix timezone option.
         */
        public DhcpV6NewPosixTimezoneOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp new posix timezone option.
         * 
         * @param newPosixTimezoneOption the new posix timezone option
         */
        public DhcpV6NewPosixTimezoneOption(v6NewPosixTimezoneOption newPosixTimezoneOption) : base(newPosixTimezoneOption)
        {
            SetCode(DhcpConstants.V6OPTION_NEW_POSIX_TIMEZONE);
        }
    }
}