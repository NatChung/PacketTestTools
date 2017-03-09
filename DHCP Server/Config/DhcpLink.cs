using PIXIS.DHCP.Option.V4;
using PIXIS.DHCP.Option.V6;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
namespace PIXIS.DHCP.Config
{
    public class DhcpLink
    {
        private Subnet subnet;

        /** The link. */
        private link link;

        private DhcpV6ConfigOptions msgConfigOptions;
        private DhcpV6ConfigOptions iaNaConfigOptions;
        private DhcpV6ConfigOptions naAddrConfigOptions;
        private DhcpV6ConfigOptions iaTaConfigOptions;
        private DhcpV6ConfigOptions taAddrConfigOptions;
        private DhcpV6ConfigOptions iaPdConfigOptions;
        private DhcpV6ConfigOptions prefixConfigOptions;
        private DhcpV4ConfigOptions v4ConfigOptions;

        public DhcpLink(Subnet subnet, link link)
        {
            this.subnet = subnet;
            this.link = link;
            msgConfigOptions = new DhcpV6ConfigOptions(link.v6MsgConfigOptions);
            iaNaConfigOptions = new DhcpV6ConfigOptions(link.v6IaNaConfigOptions);
            naAddrConfigOptions = new DhcpV6ConfigOptions(link.v6NaAddrConfigOptions);
            iaTaConfigOptions = new DhcpV6ConfigOptions(link.v6IaTaConfigOptions);
            taAddrConfigOptions = new DhcpV6ConfigOptions(link.v6TaAddrConfigOptions);
            iaPdConfigOptions = new DhcpV6ConfigOptions(link.v6IaPdConfigOptions);
            prefixConfigOptions = new DhcpV6ConfigOptions(link.v6PrefixConfigOptions);
            v4ConfigOptions = new DhcpV4ConfigOptions(link.v4ConfigOptions);
        }

        /**
	 * Convenience method to get the XML Link object's address element.
	 */
        public string GetLinkAddress()
        {
            return link.Address;
        }

        /**
         * Gets the subnet.
         * 
         * @return the subnet
         */
        public Subnet GetSubnet()
        {
            return subnet;
        }

        /**
         * Sets the subnet.
         * 
         * @param subnet the new subnet
         */
        public void setSubnet(Subnet subnet)
        {
            this.subnet = subnet;
        }

        /**
         * Gets the link.
         * 
         * @return the link
         */
        public link GetLink()
        {
            return link;
        }

        /**
         * Sets the link.
         * 
         * @param link the new link
         */
        public void SetLink(link link)
        {
            this.link = link;
        }

        public DhcpV6ConfigOptions GetMsgConfigOptions()
        {
            return msgConfigOptions;
        }

        public void SetMsgConfigOptions(DhcpV6ConfigOptions msgConfigOptions)
        {
            this.msgConfigOptions = msgConfigOptions;
        }

        public DhcpV6ConfigOptions GetIaNaConfigOptions()
        {
            return iaNaConfigOptions;
        }

        public void SetIaNaConfigOptions(DhcpV6ConfigOptions iaNaConfigOptions)
        {
            this.iaNaConfigOptions = iaNaConfigOptions;
        }

        public DhcpV6ConfigOptions GetIaTaConfigOptions()
        {
            return iaTaConfigOptions;
        }

        public void SetIaTaConfigOptions(DhcpV6ConfigOptions iaTaConfigOptions)
        {
            this.iaTaConfigOptions = iaTaConfigOptions;
        }

        public DhcpV6ConfigOptions GetIaPdConfigOptions()
        {
            return iaPdConfigOptions;
        }

        public void SetIaPdConfigOptions(DhcpV6ConfigOptions iaPdConfigOptions)
        {
            this.iaPdConfigOptions = iaPdConfigOptions;
        }

        public DhcpV6ConfigOptions GetNaAddrConfigOptions()
        {
            return naAddrConfigOptions;
        }

        public void SetNaAddrConfigOptions(DhcpV6ConfigOptions naAddrConfigOptions)
        {
            this.naAddrConfigOptions = naAddrConfigOptions;
        }

        public DhcpV6ConfigOptions GetTaAddrConfigOptions()
        {
            return taAddrConfigOptions;
        }

        public void SetTaAddrConfigOptions(DhcpV6ConfigOptions taAddrConfigOptions)
        {
            this.taAddrConfigOptions = taAddrConfigOptions;
        }

        public DhcpV6ConfigOptions GetPrefixConfigOptions()
        {
            return prefixConfigOptions;
        }

        public void SetPrefixConfigOptions(DhcpV6ConfigOptions prefixConfigOptions)
        {
            this.prefixConfigOptions = prefixConfigOptions;
        }

        public DhcpV4ConfigOptions GetV4ConfigOptions()
        {
            return v4ConfigOptions;
        }

        public void SetV4ConfigOptions(DhcpV4ConfigOptions v4ConfigOptions)
        {
            this.v4ConfigOptions = v4ConfigOptions;
        }
    }
}
