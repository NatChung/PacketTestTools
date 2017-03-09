using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6RapidCommitOption : BaseEmptyOption
    {
        /**
	     * Instantiates a new dhcp rapid commit option.
	     */
        public DhcpV6RapidCommitOption() : base()
        {
            SetCode(DhcpConstants.V6OPTION_RAPID_COMMIT);
        }
    }
}