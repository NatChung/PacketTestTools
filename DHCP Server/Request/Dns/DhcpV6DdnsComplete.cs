using PIXIS.DHCP.Config;
using PIXIS.DHCP.Option.V6;
using PIXIS.DHCP.Request.Bind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Request.Dns
{
    public class DhcpV6DdnsComplete : DdnsCallback
    {
        protected DhcpServerConfiguration dhcpServerConfig = new DhcpServerConfiguration();
        private V6BindingAddress bindingAddr;
        private DhcpV6ClientFqdnOption fqdnOption;

        public DhcpV6DdnsComplete(V6BindingAddress bindingAddr,
                DhcpV6ClientFqdnOption fqdnOption)
        {
            this.bindingAddr = bindingAddr;
            this.fqdnOption = fqdnOption;
        }

        public void FevAddComplete(bool success)
        {
            throw new NotImplementedException();
        }

        public void FevDeleteComplete(bool success)
        {
            throw new NotImplementedException();
        }

        public void FwdAddComplete(bool success)
        {
            if (success)
            {
                dhcpServerConfig.GetIaMgr().SaveDhcpOption(bindingAddr, fqdnOption);
            }
        }

        public void FwdDeleteComplete(bool success)
        {
            if (success)
            {
                dhcpServerConfig.GetIaMgr().DeleteDhcpOption(bindingAddr, fqdnOption);
            }
        }

        public void RevAddComplete(object p)
        {
            throw new NotImplementedException();
        }

        public void RevAddComplete(bool success)
        {
            if (success)
            {
                dhcpServerConfig.GetIaMgr().SaveDhcpOption(bindingAddr, fqdnOption);
            }
        }

        public void RevDeleteComplete(object p)
        {
            throw new NotImplementedException();
        }

        public void RevDeleteComplete(bool success)
        {
            if (success)
            {
                dhcpServerConfig.GetIaMgr().DeleteDhcpOption(bindingAddr, fqdnOption);
            }
        }
    }
}
