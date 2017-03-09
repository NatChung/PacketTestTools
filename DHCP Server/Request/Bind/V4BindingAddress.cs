using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PIXIS.DHCP.Config;
using PIXIS.DHCP.DB;

namespace PIXIS.DHCP.Request.Bind
{
    public class V4BindingAddress : IaAddress, BindingObject
    {
        private DhcpConfigObject configObj;

        /**
         * Instantiates a new binding address.
         * 
         * @param iaAddr the ia addr
         * @param configObj the config object
         */
        public V4BindingAddress(IaAddress iaAddr, DhcpConfigObject configObj)
        {
            // populate *this* IaAddress from the given one
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
            return configObj;
        }

        public void SetConfigObj(DhcpConfigObject configObj)
        {
            this.configObj = configObj;
        }
    }
}
