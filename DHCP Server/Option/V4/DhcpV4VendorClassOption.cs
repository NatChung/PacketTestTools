using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option.V4
{
    public class DhcpV4VendorClassOption : BaseOpaqueDataOption
    {
        /**
	     * Instantiates a new dhcp vendor class option.
	     */
        public DhcpV4VendorClassOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp vendor class option.
         * 
         * @param v4VendorClassOption the vendor class option
         */
        public DhcpV4VendorClassOption(v4VendorClassOption v4VendorClassOption) : base(v4VendorClassOption)
        {
            SetCode(DhcpConstants.V4OPTION_VENDOR_CLASS);
            SetV4(true);
        }

        public bool Matches(v4VendorClassOption that, @operator op)
        {
            if (that == null)
                return false;
            if (that.code != this.GetCode())
                return false;
            if (that.opaqueData == null)
                return false;

            return OpaqueDataUtil.Matches(opaqueData, that.opaqueData, op);
        }
    }
}
