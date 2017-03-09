using System;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V4
{
    public class DhcpV4ClientIdOption : BaseOpaqueDataOption
    {
        /**
      * Instantiates a new dhcpv4 client id option.
      */
        public DhcpV4ClientIdOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcpv4 client id option.
         * 
         * @param v4ClientIdOption the client id option
         */
        public DhcpV4ClientIdOption(v4ClientIdOption v4ClientIdOption) : base(v4ClientIdOption)
        {
            SetCode(DhcpConstants.V4OPTION_CLIENT_ID);
            SetV4(true);
        }

        public bool Matches(v6ClientIdOption that, @operator op)
        {
            if (that == null)
                return false;
            if (that.code != this.code)
                return false;
            if (that.opaqueData == null)
                return false;

            return OpaqueDataUtil.Matches(opaqueData, that.opaqueData, op);
        }
    }
}