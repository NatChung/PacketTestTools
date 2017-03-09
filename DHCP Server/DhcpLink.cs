using DHCP_Server.Config;
using DHCP_Server.Utility;
using DHCP_Server.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHCP_Server
{
    public class DhcpLink
    {
        DhcpV6ConfigOptions _msgConfigOptions;
        DhcpV6ConfigOptions _naAddrConfigOptions;
        DhcpV6ConfigOptions _iaNaConfigOptions;
        DhcpV6ConfigOptions _iaTaConfigOptions;
        DhcpV6ConfigOptions _taAddrConfigOptions;
        DhcpV6ConfigOptions _iaPdConfigOptions;
        DhcpV6ConfigOptions _prefixConfigOptions;
        DhcpV4ConfigOptions _v4ConfigOptions;
        Subnet _subnet;
        link _link;


        //Instantiates a new dhcp link.

        //@param subnet the subnet
        //@param link the link

        public DhcpLink(Subnet subnet, link link)
        {
            _subnet = subnet;
            _link = link;
            _msgConfigOptions = new DhcpV6ConfigOptions(link.GetV6MsgConfigOptions());
            _iaNaConfigOptions = new DhcpV6ConfigOptions(link.GetV6IaNaConfigOptions());
            _naAddrConfigOptions = new DhcpV6ConfigOptions(link.GetV6NaAddrConfigOptions());
            _iaTaConfigOptions = new DhcpV6ConfigOptions(link.GetV6IaTaConfigOptions());
            _taAddrConfigOptions = new DhcpV6ConfigOptions(link.GetV6TaAddrConfigOptions());
            _iaPdConfigOptions = new DhcpV6ConfigOptions(link.GetV6IaPdConfigOptions());
            _prefixConfigOptions = new DhcpV6ConfigOptions(link.GetV6PrefixConfigOptions());
            _v4ConfigOptions = new DhcpV4ConfigOptions(link.GetV4ConfigOptions());
        }

        //Convenience method to get the XML Link object's address element.
        public String GetLinkAddress()
        {
            return _link.GetAddress();
        }

        //Gets the subnet.
        //@return the subnet
        public Subnet GetSubnet()
        {
            return _subnet;
        }

        //Sets the subnet.
        //@param subnet the new subnet
        public void SetSubnet(Subnet subnet)
        {
            _subnet = subnet;
        }

        //Gets the link.
        //@return the link
        public link GetLink()
        {
            return _link;
        }
        //Sets the link.
        //@param link the new link
        public void setLink(link link)
        {
            _link = link;
        }
        public DhcpV6ConfigOptions getMsgConfigOptions()
        {
            return _msgConfigOptions;
        }

        public void setMsgConfigOptions(DhcpV6ConfigOptions msgConfigOptions)
        {
            _msgConfigOptions = msgConfigOptions;
        }

        public DhcpV6ConfigOptions getIaNaConfigOptions()
        {
            return _iaNaConfigOptions;
        }

        public void setIaNaConfigOptions(DhcpV6ConfigOptions iaNaConfigOptions)
        {
            _iaNaConfigOptions = iaNaConfigOptions;
        }

        public DhcpV6ConfigOptions getIaTaConfigOptions()
        {
            return _iaTaConfigOptions;
        }

        public void setIaTaConfigOptions(DhcpV6ConfigOptions iaTaConfigOptions)
        {
            _iaTaConfigOptions = iaTaConfigOptions;
        }

        public DhcpV6ConfigOptions getIaPdConfigOptions()
        {
            return _iaPdConfigOptions;
        }

        public void setIaPdConfigOptions(DhcpV6ConfigOptions iaPdConfigOptions)
        {
            _iaPdConfigOptions = iaPdConfigOptions;
        }

        public DhcpV6ConfigOptions getNaAddrConfigOptions()
        {
            return _naAddrConfigOptions;
        }

        public void setNaAddrConfigOptions(DhcpV6ConfigOptions naAddrConfigOptions)
        {
            _naAddrConfigOptions = naAddrConfigOptions;
        }

        public DhcpV6ConfigOptions getTaAddrConfigOptions()
        {
            return _taAddrConfigOptions;
        }

        public void setTaAddrConfigOptions(DhcpV6ConfigOptions taAddrConfigOptions)
        {
            _taAddrConfigOptions = taAddrConfigOptions;
        }

        public DhcpV6ConfigOptions getPrefixConfigOptions()
        {
            return _prefixConfigOptions;
        }

        public void setPrefixConfigOptions(DhcpV6ConfigOptions prefixConfigOptions)
        {
            _prefixConfigOptions = prefixConfigOptions;
        }

        public DhcpV4ConfigOptions getV4ConfigOptions()
        {
            return _v4ConfigOptions;
        }

        public void setV4ConfigOptions(DhcpV4ConfigOptions v4ConfigOptions)
        {
            _v4ConfigOptions = v4ConfigOptions;
        }
    }
}
