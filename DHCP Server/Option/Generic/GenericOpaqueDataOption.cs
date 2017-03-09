using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option.Generic
{
    public class GenericOpaqueDataOption : BaseOpaqueDataOption
    {
        public GenericOpaqueDataOption(int code, string name) : this(code, name, null)
        {
        }

        /**
         * Instantiates a new generic opaque opaqueData option.
         * 
         * @param code the option code
         * @param name the option name
         * @param opaqueDataOption the opaque opaqueData option
         */
        public GenericOpaqueDataOption(int code, string name,
                                       opaqueDataOptionType opaqueDataOption) : base(opaqueDataOption)
        {
            SetCode(code);
            SetName(name);
        }
    }
}
