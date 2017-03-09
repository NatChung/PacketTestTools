using PIXIS.DHCP.Config;
using PIXIS.DHCP.DB;
using PIXIS.DHCP.Message;
using PIXIS.DHCP.Option.V6;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Request.Bind
{
    public class V6TaAddrBindingManagerImpl : V6AddrBindingManager, V6TaAddrBindingManager
    {
        /**
	     * Instantiates a new ta addr binding manager impl.
	     * 
	     * @throws DhcpServerConfigException the dhcp server config exception
	     */
        public V6TaAddrBindingManagerImpl() : base()
        {
        }

        protected override List<v6AddressBinding> GetV6AddressBindings(link link)
        {
            return link.v6TaAddrBindings;
        }

        protected override List<v6AddressPool> GetV6AddressPools(linkFilter linkFilter)
        {
            return linkFilter.v6TaAddrPools;
        }
        protected override List<v6AddressPool> GetV6AddressPools(link link)
        {
            return link.v6TaAddrPools;
        }
        public Binding FindCurrentBinding(DhcpLink clientLink,
                DhcpV6ClientIdOption clientIdOption, DhcpV6IaTaOption iaTaOption,
                DhcpMessage requestMsg)
        {

            byte[] duid = clientIdOption.GetDuid();
            long iaid = iaTaOption.GetIaId();

            return base.FindCurrentBinding(clientLink, duid, IdentityAssoc.TA_TYPE,
                    iaid, requestMsg);
        }

        public Binding CreateSolicitBinding(DhcpLink clientLink,
                DhcpV6ClientIdOption clientIdOption, DhcpV6IaTaOption iaTaOption,
                DhcpMessage requestMsg, byte state, IPAddress clientV4IPAddress)
        {

            byte[] duid = clientIdOption.GetDuid();
            long iaid = iaTaOption.GetIaId();

            StaticBinding staticBinding =
                FindStaticBinding(clientLink.GetLink(), duid, IdentityAssoc.TA_TYPE, iaid, requestMsg);

            if (staticBinding != null)
            {
                return base.CreateStaticBinding(clientLink, duid, IdentityAssoc.TA_TYPE,
                        iaid, staticBinding, requestMsg);
            }
            else
            {
                return base.CreateBinding(clientLink, duid, IdentityAssoc.TA_TYPE,
                        iaid, GetInetAddrs(iaTaOption), requestMsg, state, clientV4IPAddress);
            }
        }

        public Binding UpdateBinding(Binding binding, DhcpLink clientLink,
                DhcpV6ClientIdOption clientIdOption, DhcpV6IaTaOption iaTaOption,
                DhcpMessage requestMsg, byte state, IPAddress clientV4IPAddress)
        {

            byte[] duid = clientIdOption.GetDuid();
            long iaid = iaTaOption.GetIaId();

            StaticBinding staticBinding =
                FindStaticBinding(clientLink.GetLink(), duid, IdentityAssoc.TA_TYPE, iaid, requestMsg);

            if (staticBinding != null)
            {
                return base.UpdateStaticBinding(binding, clientLink, duid, IdentityAssoc.TA_TYPE,
                        iaid, staticBinding, requestMsg);
            }
            else
            {
                return base.UpdateBinding(binding, clientLink, duid, IdentityAssoc.TA_TYPE,
                        iaid, GetInetAddrs(iaTaOption), requestMsg, state, clientV4IPAddress);
            }
        }

        /**
         * Extract the list of IP addresses from within the given IA_TA option.
         * 
         * @param iaNaOption the IA_TA option
         * 
         * @return the list of InetAddresses for the IPs in the IA_TA option
         */
        private List<IPAddress> GetInetAddrs(DhcpV6IaTaOption iaTaOption)
        {
            List<IPAddress> inetAddrs = null;
            List<DhcpV6IaAddrOption> iaAddrs = iaTaOption.GetIaAddrOptions();
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
            return IdentityAssoc.TA_TYPE;
        }
    }
}
