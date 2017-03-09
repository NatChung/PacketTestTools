using PIXIS.DHCP.Config;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIXIS.DHCP.Message;
using PIXIS.DHCP.Option.V6;

namespace PIXIS.DHCP.Request.Bind
{
    public class V6StaticAddressBinding : StaticBinding, DhcpV6OptionConfigObject
    {
        protected v6AddressBinding addressBinding;
        protected byte iaType;

        // The configured options for this binding
        protected DhcpV6ConfigOptions dhcpConfigOptions;

        public V6StaticAddressBinding(v6AddressBinding addressBinding, byte iaType)
        {
            this.addressBinding = addressBinding;
            this.iaType = iaType;
            dhcpConfigOptions =
                new DhcpV6ConfigOptions(addressBinding.addrConfigOptions);
        }

        public override string GetIpAddress()
        {
            return addressBinding.ipAddress;
        }

        public v6AddressBinding GetV6AddressBinding()
        {
            return addressBinding;
        }

        public void SetV6AddressBinding(v6AddressBinding addressBinding)
        {
            this.addressBinding = addressBinding;
        }

        public DhcpV6ConfigOptions GetDhcpConfigOptions()
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
            // sb.Append(ype(this.getClass().getSimpleName() + ": iatype=");
            sb.Append(iaType);
            sb.Append(" ip=");
            sb.Append(addressBinding.policies.ToString());
            sb.Append(" duid=");
            sb.Append(addressBinding.duid);
            sb.Append(" iaid=");
            sb.Append(addressBinding.iaid);
            return sb.ToString();
        }

        public override bool Matches(byte[] duid, byte iatype, long iaid,
            DhcpMessage requestMsg)
        {
            bool rc = false;
            if (addressBinding != null)
            {
                if (iatype == this.iaType)
                {
                    if (Array.Equals(duid, addressBinding.duid))
                    {
                        //if (!addressBinding.isSetIaid()) {
                        if (addressBinding.iaid == 0)
                        {
                            return true;
                        }
                        else
                        {
                            if (iaid == addressBinding.iaid)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return rc;
        }
    }
}
