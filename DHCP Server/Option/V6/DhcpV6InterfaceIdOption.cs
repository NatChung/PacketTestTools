using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6InterfaceIdOption : BaseOpaqueDataOption
    {
        /**
         * Instantiates a new dhcp interface id option.
         */
        public DhcpV6InterfaceIdOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp interface id option.
         * 
         * @param interfaceIdOption the interface id option
         */
        public DhcpV6InterfaceIdOption(v6InterfaceIdOption interfaceIdOption):base(interfaceIdOption)
        {
            SetCode(DhcpConstants.V6OPTION_INTERFACE_ID);
        }
    }
}