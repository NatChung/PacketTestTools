using PIXIS.DHCP.Config;
using PIXIS.DHCP.Option.V4;
using PIXIS.DHCP.Request.Bind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Request.Dns
{
    public class DhcpV4DdnsComplete : DdnsCallback
    {
        private DhcpServerConfiguration dhcpServerConfig = new DhcpServerConfiguration();
        private V4BindingAddress _bindingAddr;
        private DhcpV4ClientFqdnOption _fqdnOption;

        public DhcpV4DdnsComplete(V4BindingAddress bindingAddr, DhcpV4ClientFqdnOption replyFqdnOption)
        {
            _bindingAddr = bindingAddr;
            _fqdnOption = replyFqdnOption;
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
            throw new NotImplementedException();
        }

        public void FwdDeleteComplete(bool success)
        {
            throw new NotImplementedException();
        }

        public void RevAddComplete(object p)
        {
            throw new NotImplementedException();
        }

        public void RevDeleteComplete(object p)
        {
            throw new NotImplementedException();
        }
    }
}
