using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Option.Generic;
using PIXIS.DHCP.Option.V4;
using PIXIS.DHCP.Xml;
using System.Collections.Generic;

namespace PIXIS.DHCP.Config
{
    public class DhcpV4ConfigOptions
    {
        /** The config options. */
        protected v4ConfigOptionsType configOptions;

        /** The option map. */
        protected Dictionary<int, DhcpOption> optionMap = new Dictionary<int, DhcpOption>();


        /**
         * Instantiates a new dhcp config options.
         */
        public DhcpV4ConfigOptions() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp config options.
         * 
         * @param configOptions the config options
         */
        public DhcpV4ConfigOptions(v4ConfigOptionsType configOptions)
        {
            if (configOptions != null)
                this.configOptions = configOptions;
            else
                this.configOptions = new v4ConfigOptionsType();

            InitDhcpV4OptionMap();
        }

        /**
         * Inits the dhcp option map.
         * 
         * @return the map< integer, dhcp option>
         */
        public Dictionary<int, DhcpOption> InitDhcpV4OptionMap()
        {
            optionMap.Clear();

            if (configOptions.v4DomainNameOption != null && !string.IsNullOrEmpty(configOptions.v4DomainNameOption.@string))
            {
                v4DomainNameOption domainNameOption = configOptions.v4DomainNameOption;
                if (domainNameOption != null)
                {
                    optionMap[(int)domainNameOption.code] = new DhcpV4DomainNameOption(domainNameOption);
                }
            }

            if (configOptions.v4DomainServersOption != null && configOptions.v4DomainServersOption.ipAddress.Count > 0)
            {
                v4DomainServersOption domainServersOption = configOptions.v4DomainServersOption;
                if (domainServersOption != null)
                {
                    optionMap[(int)domainServersOption.code] = new DhcpV4DomainServersOption(domainServersOption);
                }
            }

            if (configOptions.v4RoutersOption != null && configOptions.v4RoutersOption.ipAddress.Count > 0)
            {
                v4RoutersOption routersOption = configOptions.v4RoutersOption;
                if (routersOption != null)
                {
                    optionMap[(int)routersOption.code] = new DhcpV4RoutersOption(routersOption);
                }
            }

            if (configOptions.v4SubnetMaskOption != null && !string.IsNullOrEmpty(configOptions.v4SubnetMaskOption.ipAddress))
            {
                v4SubnetMaskOption subnetMaskOption = configOptions.v4SubnetMaskOption;
                if (subnetMaskOption != null)
                {
                    optionMap[(int)subnetMaskOption.code] = new DhcpV4SubnetMaskOption(subnetMaskOption);
                }
            }

            if (configOptions.v4TimeOffsetOption != null && configOptions.v4TimeOffsetOption.unsignedInt > 0)
            {
                v4TimeOffsetOption timeOffsetOption = configOptions.v4TimeOffsetOption;
                if (timeOffsetOption != null)
                {
                    optionMap[(int)timeOffsetOption.code] = new DhcpV4TimeOffsetOption(timeOffsetOption);
                }
            }

            if (configOptions.v4TimeServersOption != null && configOptions.v4TimeServersOption.ipAddress.Count > 0)
            {
                v4TimeServersOption timeServersOption = configOptions.v4TimeServersOption;
                if (timeServersOption != null)
                {
                    optionMap[(int)timeServersOption.code] = new DhcpV4TimeServersOption(timeServersOption);
                }
            }

            if (configOptions.v4VendorSpecificOption != null && configOptions.v4VendorSpecificOption.opaqueData != null
                && configOptions.v4VendorSpecificOption.opaqueData.asciiValue != null && configOptions.v4VendorSpecificOption.opaqueData.hexValue != null)
            {
                v4VendorSpecificOption vendorSpecificOption = configOptions.v4VendorSpecificOption;
                if (vendorSpecificOption != null)
                {
                    optionMap[(int)vendorSpecificOption.code] = new DhcpV4VendorSpecificOption(vendorSpecificOption);
                }
            }

            if (configOptions.v4NetbiosNameServersOption != null && configOptions.v4NetbiosNameServersOption.ipAddress.Count > 0)
            {
                v4NetbiosNameServersOption netbiosNameServersOption =
                    configOptions.v4NetbiosNameServersOption;
                if (netbiosNameServersOption != null)
                {
                    optionMap[(int)netbiosNameServersOption.code] = new DhcpV4NetbiosNameServersOption(netbiosNameServersOption);
                }
            }

            if (configOptions.v4NetbiosNodeTypeOption != null && configOptions.v4NetbiosNodeTypeOption.unsignedByte > 0)
            {
                v4NetbiosNodeTypeOption netbiosNodeTypeOption =
                    configOptions.v4NetbiosNodeTypeOption;
                if (netbiosNodeTypeOption != null)
                {
                    optionMap[(int)netbiosNodeTypeOption.code] =
                         new DhcpV4NetbiosNodeTypeOption(netbiosNodeTypeOption);
                }
            }

            if (configOptions.v4TftpServerNameOption != null && !string.IsNullOrEmpty(configOptions.v4TftpServerNameOption.@string))
            {
                v4TftpServerNameOption tftpServerNameOption = configOptions.v4TftpServerNameOption;
                if (tftpServerNameOption != null)
                {
                    optionMap[(int)tftpServerNameOption.code] =
                         new DhcpV4TftpServerNameOption(tftpServerNameOption);
                }
            }

            if (configOptions.v4BootFileNameOption != null && !string.IsNullOrEmpty(configOptions.v4BootFileNameOption.@string))
            {
                v4BootFileNameOption bootFileNameOption = configOptions.v4BootFileNameOption;
                if (bootFileNameOption != null)
                {
                    optionMap[(int)bootFileNameOption.code] =
                            new DhcpV4BootFileNameOption(bootFileNameOption);
                }
            }

            if (configOptions.otherOptions != null && configOptions.otherOptions.Count > 0)
            {
                optionMap.PutAll(GenericOptionFactory.GenericOptions(configOptions.otherOptions));
            }

            return optionMap;
        }

        /**
         * Gets the config options.
         * 
         * @return the config options
         */
        public v4ConfigOptionsType getV4ConfigOptions()
        {
            return configOptions;
        }

        /**
         * Sets the config options.
         * 
         * @param configOptions the new config options
         */
        public void FetV4ConfigOptions(v4ConfigOptionsType configOptions)
        {
            this.configOptions = configOptions;
            // reset the option map
            InitDhcpV4OptionMap();
        }

        /**
         * Gets the dhcp option map.
         * 
         * @return the dhcp option map
         */
        public Dictionary<int, DhcpOption> GetDhcpOptionMap()
        {
            return optionMap;
        }
    }
}