using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Option.Generic;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6ConfigOptions
    {

        protected v6ConfigOptionsType configOptions;
        protected Dictionary<int, DhcpOption> optionMap = new Dictionary<int, DhcpOption>();

        public DhcpV6ConfigOptions() : this(null)
        {
        }

        /**
	     * Instantiates a new dhcp config options.
	     * 
	     * @param configOptions the config options
	     */
        public DhcpV6ConfigOptions(v6ConfigOptionsType configOptions)
        {
            if ((configOptions != null))
            {
                this.configOptions = configOptions;
            }
            else
            {
                this.configOptions = new v6ConfigOptionsType();
            }
            //暫時無設定Option
            this.InitDhcpOptionMap();
        }

        public Dictionary<int, DhcpOption> InitDhcpOptionMap()
        {
            this.optionMap.Clear();

            v6BcmcsAddressesOption bcmcsAddressesOption = this.configOptions.v6BcmcsAddressesOption;
            if (bcmcsAddressesOption != null && bcmcsAddressesOption.ipAddress.Count > 0)
            {
                this.optionMap[bcmcsAddressesOption.code] = new DhcpV6BcmcsAddressesOption(bcmcsAddressesOption);
            }

            v6BcmcsDomainNamesOption bcmcsDomainNamesOption = this.configOptions.v6BcmcsDomainNamesOption;
            if (bcmcsDomainNamesOption != null && bcmcsDomainNamesOption.domainName.Count > 0)
            {
                this.optionMap[bcmcsDomainNamesOption.code] = new DhcpV6BcmcsDomainNamesOption(bcmcsDomainNamesOption);
            }

            v6DnsServersOption dnsServersOption = this.configOptions.v6DnsServersOption;
            if (dnsServersOption != null && dnsServersOption.ipAddress.Count > 0)
            {
                this.optionMap[dnsServersOption.code] = new DhcpV6DnsServersOption(dnsServersOption);
            }

            v6DomainSearchListOption domainSearchListOption = this.configOptions.v6DomainSearchListOption;
            if (domainSearchListOption != null && domainSearchListOption.domainName.Count > 0)
            {
                this.optionMap[domainSearchListOption.code] = new DhcpV6DomainSearchListOption(domainSearchListOption);
            }

            v6GeoconfCivicOption geoconfCivicOption = this.configOptions.v6GeoconfCivicOption;
            if (geoconfCivicOption != null && geoconfCivicOption.civicAddressElement.Count > 0)
            {
                this.optionMap[geoconfCivicOption.code] = new DhcpV6GeoconfCivicOption(geoconfCivicOption);
            }

            v6InfoRefreshTimeOption infoRefreshTimeOption = this.configOptions.v6InfoRefreshTimeOption;
            if (infoRefreshTimeOption != null && infoRefreshTimeOption.unsignedInt > 0)
            {
                this.optionMap[infoRefreshTimeOption.code] = new DhcpV6InfoRefreshTimeOption(infoRefreshTimeOption);
            }

            v6LostServerDomainNameOption lostServerDomainNameOption = this.configOptions.v6LostServerDomainNameOption;
            if (lostServerDomainNameOption != null && !String.IsNullOrEmpty(lostServerDomainNameOption.domainName))
            {
                this.optionMap[lostServerDomainNameOption.code] = new DhcpV6LostServerDomainNameOption(lostServerDomainNameOption);
            }

            v6NewPosixTimezoneOption newPosixTimezoneOption = this.configOptions.v6NewPosixTimezoneOption;
            if (newPosixTimezoneOption != null && !String.IsNullOrEmpty(newPosixTimezoneOption.@string))
            {
                this.optionMap[newPosixTimezoneOption.code] = new DhcpV6NewPosixTimezoneOption();
            }

            v6NewTzdbTimezoneOption newTzdbTimezoneOption = this.configOptions.v6NewTzdbTimezoneOption;
            if (newTzdbTimezoneOption != null && !String.IsNullOrEmpty(newTzdbTimezoneOption.@string))
            {
                this.optionMap[newTzdbTimezoneOption.code] = new DhcpV6NewTzdbTimezoneOption();
            }

            v6NisDomainNameOption nisDomainNameOption = this.configOptions.v6NisDomainNameOption;
            if (nisDomainNameOption != null && !String.IsNullOrEmpty(nisDomainNameOption.domainName))
            {
                this.optionMap[nisDomainNameOption.code] = new DhcpV6NisDomainNameOption(nisDomainNameOption);
            }

            v6NisPlusDomainNameOption nisPlusDomainNameOption = this.configOptions.v6NisPlusDomainNameOption;
            if (nisPlusDomainNameOption != null && !String.IsNullOrEmpty(nisPlusDomainNameOption.domainName))
            {
                this.optionMap[nisPlusDomainNameOption.code] = new DhcpV6NisPlusDomainNameOption(nisPlusDomainNameOption);
            }

            v6NisPlusServersOption nisPlusServersOption = this.configOptions.v6NisPlusServersOption;
            if (nisPlusServersOption != null && nisPlusServersOption.ipAddress.Count > 0)
            {
                this.optionMap[nisPlusServersOption.code] = new DhcpV6NisPlusServersOption(nisPlusServersOption);
            }

            v6NisServersOption nisServersOption = this.configOptions.v6NisServersOption;
            if (nisServersOption != null && nisServersOption.ipAddress.Count > 0)
            {
                this.optionMap[nisServersOption.code] = new DhcpV6NisServersOption(nisServersOption);
            }

            v6PanaAgentAddressesOption panaAgentAddressesOption = this.configOptions.v6PanaAgentAddressesOption;
            if (panaAgentAddressesOption != null && panaAgentAddressesOption.ipAddress.Count > 0)
            {
                this.optionMap[panaAgentAddressesOption.code] = new DhcpV6PanaAgentAddressesOption(panaAgentAddressesOption);
            }

            v6PreferenceOption preferenceOption = this.configOptions.v6PreferenceOption;
            if (preferenceOption != null && preferenceOption.unsignedByte != 0)
            {
                this.optionMap[preferenceOption.code] = new DhcpV6PreferenceOption(preferenceOption);
            }

            v6ServerUnicastOption serverUnicastOption = this.configOptions.v6ServerUnicastOption;
            if (serverUnicastOption != null && !String.IsNullOrEmpty(serverUnicastOption.ipAddress))
            {
                this.optionMap[serverUnicastOption.code] = new DhcpV6ServerUnicastOption(serverUnicastOption);
            }

            v6SipServerAddressesOption sipServerAddressesOption = this.configOptions.v6SipServerAddressesOption;
            if (sipServerAddressesOption != null && sipServerAddressesOption.ipAddress.Count > 0)
            {
                this.optionMap[sipServerAddressesOption.code] = new DhcpV6SipServerAddressesOption(sipServerAddressesOption);
            }

            v6SipServerDomainNamesOption sipServerDomainNamesOption = this.configOptions.v6SipServerDomainNamesOption;
            if (sipServerDomainNamesOption != null && sipServerDomainNamesOption.domainName.Count > 0)
            {
                this.optionMap[sipServerDomainNamesOption.code] = new DhcpV6SipServerDomainNamesOption(sipServerDomainNamesOption);
            }

            v6SntpServersOption sntpServersOption = this.configOptions.v6SntpServersOption;
            if (sntpServersOption != null && sntpServersOption.ipAddress.Count > 0)
            {
                this.optionMap[sntpServersOption.code] = new DhcpV6SntpServersOption(sntpServersOption);
            }

            v6StatusCodeOption statusCodeOption = this.configOptions.v6StatusCodeOption;
            if (statusCodeOption != null && !String.IsNullOrEmpty(statusCodeOption.message))
            {
                this.optionMap[statusCodeOption.code] = new DhcpV6StatusCodeOption(statusCodeOption);
            }


            v6VendorInfoOption vendorInfoOption = this.configOptions.v6VendorInfoOption;
            if (vendorInfoOption != null && vendorInfoOption.suboptionList.Count > 0)
            {
                this.optionMap[vendorInfoOption.code] = new DhcpV6VendorInfoOption(vendorInfoOption);
            }

            if (this.configOptions.v6OtherOptions.Count > 0)
            {
                optionMap.PutAll(GenericOptionFactory.GenericOptions(configOptions.v6OtherOptions));
            }
            return this.optionMap;
        }

        public v6ConfigOptionsType GetV6ConfigOptions()
        {
            return this.configOptions;
        }

        public void SetV6ConfigOptions(v6ConfigOptionsType configOptions)
        {
            this.configOptions = configOptions;
            //  reset the option map
            InitDhcpOptionMap();
        }

        public Dictionary<int, DhcpOption> GetDhcpOptionMap()
        {
            return this.optionMap;
        }
    }
}
