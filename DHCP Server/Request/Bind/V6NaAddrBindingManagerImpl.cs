using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIXIS.DHCP.Config;
using PIXIS.DHCP.DB;
using PIXIS.DHCP.Message;
using PIXIS.DHCP.Option.V6;
using PIXIS.DHCP.Xml;
using System.Net;

namespace PIXIS.DHCP.Request.Bind
{
    public class V6NaAddrBindingManagerImpl : V6AddrBindingManager, V6NaAddrBindingManager
    {
        /**
	     * Instantiates a new na addr binding manager impl.
	     * 
	     * @throws DhcpServerConfigException the dhcp server config exception
	     */
        public V6NaAddrBindingManagerImpl() : base()
        {

        }

        protected override List<v6AddressBinding> GetV6AddressBindings(link link)
        {
            return link.v6NaAddrBindings;
        }

        protected override List<v6AddressPool> GetV6AddressPools(linkFilter linkFilter)
        {
            return linkFilter.v6NaAddrPools;
        }
        protected override List<v6AddressPool> GetV6AddressPools(link link)
        {
            return link.v6NaAddrPools;
        }

        public Binding FindCurrentBinding(DhcpLink clientLink, DhcpV6ClientIdOption clientIdOption, DhcpV6IaNaOption iaNaOption, DhcpMessage requestMsg)
        {
            byte[] duid = clientIdOption.GetDuid();
            long iaid = iaNaOption.GetIaId();

            return base.FindCurrentBinding(clientLink, duid, IdentityAssoc.NA_TYPE,
                    iaid, requestMsg);
        }

        public Binding CreateSolicitBinding(DhcpLink clientLink, DhcpV6ClientIdOption clientIdOption, DhcpV6IaNaOption iaNaOption, DhcpMessage requestMsg, byte state, IPAddress clientV4IPAddress)
        {
            byte[] duid = clientIdOption.GetDuid();
            long iaid = iaNaOption.GetIaId();

            StaticBinding staticBinding = FindStaticBinding(clientLink.GetLink(), duid, IdentityAssoc.NA_TYPE, iaid, requestMsg);

            if (staticBinding != null)
            {
                return base.CreateStaticBinding(clientLink, duid, IdentityAssoc.NA_TYPE, iaid, staticBinding, requestMsg);
            }
            else
            {
                return base.CreateBinding(clientLink, duid, IdentityAssoc.NA_TYPE,
                        iaid, GetInetAddrs(iaNaOption), requestMsg, state, clientV4IPAddress);
            }
        }



        public Binding UpdateBinding(Binding binding, DhcpLink clientLink,
            DhcpV6ClientIdOption clientIdOption, DhcpV6IaNaOption iaNaOption,
            DhcpMessage requestMsg, byte state, IPAddress clientV4IPAddress)
        {
            byte[] duid = clientIdOption.GetDuid();
            long iaid = iaNaOption.GetIaId();

            StaticBinding staticBinding =
                FindStaticBinding(clientLink.GetLink(), duid, IdentityAssoc.NA_TYPE, iaid, requestMsg);

            if (staticBinding != null)
            {
                return base.UpdateStaticBinding(binding, clientLink, duid, IdentityAssoc.NA_TYPE,
                        iaid, staticBinding, requestMsg);
            }
            else
            {
                return base.UpdateBinding(binding, clientLink, duid, IdentityAssoc.NA_TYPE,
                        iaid, GetInetAddrs(iaNaOption), requestMsg, state, clientV4IPAddress);
            }
        }

        /**
          * Extract the list of IP addresses from within the given IA_NA option.
          * 
          * @param iaNaOption the IA_NA option
          * 
          * @return the list of InetAddresses for the IPs in the IA_NA option
          */
        private List<IPAddress> GetInetAddrs(DhcpV6IaNaOption iaNaOption)
        {
            List<IPAddress> inetAddrs = null;
            List<DhcpV6IaAddrOption> iaAddrs = iaNaOption.GetIaAddrOptions();
            if ((iaAddrs != null) && iaAddrs.Count > 0)
            {
                inetAddrs = new List<IPAddress>();
                foreach (DhcpV6IaAddrOption iaAddr in iaAddrs)
                {
                    IPAddress inetAddr = iaAddr.GetInetAddress();
                    inetAddrs.Add(inetAddr);
                }
            }
            return inetAddrs;
        }


        protected override byte GetIaType()
        {
            return IdentityAssoc.NA_TYPE;
        }



    }
}
