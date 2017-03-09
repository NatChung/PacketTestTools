using PIXIS.DHCP.Config;
using PIXIS.DHCP.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Request.Bind
{
    public class V6BindingAddress : IaAddress, BindingObject
    {

        private DhcpConfigObject configObj;

        public V6BindingAddress(IaAddress iaAddr, DhcpConfigObject configObj)
        {
            //  populate *this* IaAddress from the given one
            this.SetDhcpOptions(iaAddr.GetDhcpOptions());
            this.SetId(iaAddr.GetId());
            this.SetIdentityAssocId(iaAddr.GetIdentityAssocId());
            this.SetIpAddress(iaAddr.GetIpAddress());
            this.SetPreferredEndTime(iaAddr.GetPreferredEndTime());
            this.SetStartTime(iaAddr.GetStartTime());
            this.SetState(iaAddr.GetState());
            this.SetValidEndTime(iaAddr.GetValidEndTime());
            this.configObj = configObj;
        }

        public DhcpConfigObject GetConfigObj()
        {
            return this.configObj;
        }

        public void SetConfigObj(DhcpV6OptionConfigObject configObj)
        {
            this.configObj = configObj;
        }
        
    }
}
