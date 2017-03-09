using PIXIS.DHCP.Config;
using PIXIS.DHCP.DB;
using PIXIS.DHCP.Message;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Request.Bind
{
    public class V4StaticAddressBinding : StaticBinding, DhcpV4OptionConfigObject
    {
        // the XML object wrapped by this class
        protected v4AddressBinding addressBinding;

        // The configured options for this binding
        protected DhcpV4ConfigOptions dhcpConfigOptions;

        public V4StaticAddressBinding(v4AddressBinding addressBinding)
        {
            this.addressBinding = addressBinding;
            dhcpConfigOptions =
                new DhcpV4ConfigOptions(addressBinding.configOptions);
        }

        public override bool Matches(byte[] duid, byte iatype, long iaid,
                DhcpMessage requestMsg)
        {
            bool rc = false;
            if (addressBinding != null)
            {
                if (iatype == IdentityAssoc.V4_TYPE)
                {
                    if (Array.Equals(duid, addressBinding.chaddr))
                    {
                        return true;
                    }
                }
            }
            return rc;
        }


        public override string GetIpAddress()
        {
            return addressBinding.ipAddress;
        }

        public v4AddressBinding GetV4AddressBinding()
        {
            return addressBinding;
        }

        public void SetV6AddressBinding(v4AddressBinding addressBinding)
        {
            this.addressBinding = addressBinding;
        }

        public DhcpV4ConfigOptions GetV4ConfigOptions()
        {
            return dhcpConfigOptions;
        }

        public override List<Policy> GetPolicies()
        {
            if (addressBinding != null)
                return addressBinding.policies;
            return null;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.GetType().Name + ": ip=");
            sb.Append(addressBinding.ipAddress);
            sb.Append(" chaddr=");
            sb.Append(addressBinding);
            return sb.ToString();
        }
    }
}
