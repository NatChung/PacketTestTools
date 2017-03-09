using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6UserClassOption : BaseOpaqueDataListOption
    {
        /**
	 * Instantiates a new dhcp user class option.
	 */
        public DhcpV6UserClassOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp user class option.
         * 
         * @param userClassOption the user class option
         */
        public DhcpV6UserClassOption(v6UserClassOption userClassOption):base(userClassOption)
        {
            SetCode(DhcpConstants.V6OPTION_USER_CLASS);
        }

        public bool Matches(DhcpV6UserClassOption that, @operator op)
        {
            if (that == null)
                return false;
            if (that.GetCode() != this.GetCode())
                return false;

            return base.Matches(that, op);
        }
    }
}
