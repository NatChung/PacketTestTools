using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Option.Base;

namespace PIXIS.DHCP.Option.V4
{
    public class DhcpV4ConfigOptions
    {
        private v4ConfigOptionsType configOptions;

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

            //if (configOptions.isSetV4DomainNameOption())
            //{
            //    V4DomainNameOption domainNameOption = configOptions.getV4DomainNameOption();
            //    if (domainNameOption != null)
            //    {
            //        optionMap.put((int)domainNameOption.getCode(),
            //                new DhcpV4DomainNameOption(domainNameOption));
            //    }
            //}

            //if (configOptions.isSetV4DomainServersOption())
            //{
            //    V4DomainServersOption domainServersOption = configOptions.getV4DomainServersOption();
            //    if (domainServersOption != null)
            //    {
            //        optionMap.put((int)domainServersOption.getCode(),
            //                new DhcpV4DomainServersOption(domainServersOption));
            //    }
            //}

            //if (configOptions.isSetV4RoutersOption())
            //{
            //    V4RoutersOption routersOption = configOptions.getV4RoutersOption();
            //    if (routersOption != null)
            //    {
            //        optionMap.put((int)routersOption.getCode(),
            //                new DhcpV4RoutersOption(routersOption));
            //    }
            //}

            //if (configOptions.isSetV4SubnetMaskOption())
            //{
            //    V4SubnetMaskOption subnetMaskOption = configOptions.getV4SubnetMaskOption();
            //    if (subnetMaskOption != null)
            //    {
            //        optionMap.put((int)subnetMaskOption.getCode(),
            //                new DhcpV4SubnetMaskOption(subnetMaskOption));
            //    }
            //}

            //if (configOptions.isSetV4TimeOffsetOption())
            //{
            //    V4TimeOffsetOption timeOffsetOption = configOptions.getV4TimeOffsetOption();
            //    if (timeOffsetOption != null)
            //    {
            //        optionMap.put((int)timeOffsetOption.getCode(),
            //                new DhcpV4TimeOffsetOption(timeOffsetOption));
            //    }
            //}

            //if (configOptions.isSetV4TimeServersOption())
            //{
            //    V4TimeServersOption timeServersOption = configOptions.getV4TimeServersOption();
            //    if (timeServersOption != null)
            //    {
            //        optionMap.put((int)timeServersOption.getCode(),
            //                new DhcpV4TimeServersOption(timeServersOption));
            //    }
            //}

            //if (configOptions.isSetV4VendorSpecificOption())
            //{
            //    V4VendorSpecificOption vendorSpecificOption = configOptions.getV4VendorSpecificOption();
            //    if (vendorSpecificOption != null)
            //    {
            //        optionMap.put((int)vendorSpecificOption.getCode(),
            //                new DhcpV4VendorSpecificOption(vendorSpecificOption));
            //    }
            //}

            //if (configOptions.isSetV4NetbiosNameServersOption())
            //{
            //    V4NetbiosNameServersOption netbiosNameServersOption =
            //        configOptions.getV4NetbiosNameServersOption();
            //    if (netbiosNameServersOption != null)
            //    {
            //        optionMap.put((int)netbiosNameServersOption.getCode(),
            //                new DhcpV4NetbiosNameServersOption(netbiosNameServersOption));
            //    }
            //}

            //if (configOptions.isSetV4NetbiosNodeTypeOption())
            //{
            //    V4NetbiosNodeTypeOption netbiosNodeTypeOption =
            //        configOptions.getV4NetbiosNodeTypeOption();
            //    if (netbiosNodeTypeOption != null)
            //    {
            //        optionMap.put((int)netbiosNodeTypeOption.getCode(),
            //                new DhcpV4NetbiosNodeTypeOption(netbiosNodeTypeOption));
            //    }
            //}

            //if (configOptions.isSetV4TftpServerNameOption())
            //{
            //    V4TftpServerNameOption tftpServerNameOption =
            //        configOptions.getV4TftpServerNameOption();
            //    if (tftpServerNameOption != null)
            //    {
            //        optionMap.put((int)tftpServerNameOption.getCode(),
            //                new DhcpV4TftpServerNameOption(tftpServerNameOption));
            //    }
            //}

            //if (configOptions.isSetV4BootFileNameOption())
            //{
            //    V4BootFileNameOption bootFileNameOption =
            //        configOptions.getV4BootFileNameOption();
            //    if (bootFileNameOption != null)
            //    {
            //        optionMap.put((int)bootFileNameOption.getCode(),
            //                new DhcpV4BootFileNameOption(bootFileNameOption));
            //    }
            //}

            //if (configOptions.isSetOtherOptions())
            //{
            //    optionMap.putAll(GenericOptionFactory.genericOptions(configOptions.getOtherOptions()));
            //}

            return optionMap;
        }

        /**
         * Gets the config options.
         * 
         * @return the config options
         */
        public v4ConfigOptionsType GetV4ConfigOptions()
        {
            return configOptions;
        }

        /**
         * Sets the config options.
         * 
         * @param configOptions the new config options
         */
        public void SetV4ConfigOptions(v4ConfigOptionsType configOptions)
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
