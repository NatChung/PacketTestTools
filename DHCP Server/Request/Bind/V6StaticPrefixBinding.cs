using PIXIS.DHCP.Config;
using PIXIS.DHCP.DB;
using PIXIS.DHCP.Message;
using PIXIS.DHCP.Option.V6;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Request.Bind
{
    public class V6StaticPrefixBinding : StaticBinding, DhcpV6OptionConfigObject
    {
        // the XML object wrapped by this class
        protected v6PrefixBinding prefixBinding;

        // The configured options for this binding
        protected DhcpV6ConfigOptions dhcpConfigOptions;

        public V6StaticPrefixBinding(v6PrefixBinding prefixBinding)
        {
            this.prefixBinding = prefixBinding;
            dhcpConfigOptions =
                new DhcpV6ConfigOptions(prefixBinding.prefixConfigOptions);
        }

        public override bool Matches(byte[] duid, byte iatype, long iaid,
                DhcpMessage requestMsg)
        {
            bool rc = false;
            if (prefixBinding != null)
            {
                if (iatype == IdentityAssoc.PD_TYPE)
                {
                    if (Array.Equals(duid, prefixBinding.duid.hexValue))
                    {
                        if (prefixBinding.iaid == 0)
                        {
                            return true;
                        }
                        else
                        {
                            if (iaid == prefixBinding.iaid)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return rc;
        }

        public override string GetIpAddress()
        {
            return prefixBinding.prefix;
        }

        public v6PrefixBinding GetV6PrefixBinding()
        {
            return prefixBinding;
        }

        public void SetV6PrefixBinding(v6PrefixBinding prefixBinding)
        {
            this.prefixBinding = prefixBinding;
        }

        public DhcpV6ConfigOptions GetDhcpConfigOptions()
        {
            return dhcpConfigOptions;
        }

        public override List<Policy> GetPolicies()
        {
            if (prefixBinding != null)
                return prefixBinding.policies;
            return null;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.GetType().Name + ": Prefix=");
            sb.Append(prefixBinding.prefix);
            sb.Append('/');
            sb.Append(prefixBinding.prefixLength);
            sb.Append(" duid=");
            sb.Append(prefixBinding.duid);
            sb.Append(" iaid=");
            sb.Append(prefixBinding.iaid);
            return sb.ToString();
        }
    }
}
