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
    public class DhcpV6ClientIdOption : BaseOpaqueDataOption
    {

        public DhcpV6ClientIdOption() :
                this(null)
        {
        }

        public DhcpV6ClientIdOption(v6ClientIdOption clientIdOption) :
                base(clientIdOption)
        {
            SetCode(DhcpConstants.V6OPTION_CLIENTID);
        }

        public byte[] GetDuid()
        {
            if (!string.IsNullOrEmpty((this.opaqueData.GetAscii())))
            {
                return Encoding.ASCII.GetBytes(this.opaqueData.GetAscii());
            }
            else
            {
                return this.opaqueData.GetHex();
            }

        }
    }
}
