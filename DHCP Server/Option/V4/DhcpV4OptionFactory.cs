﻿using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.V4Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option.V4
{
    public class DhcpV4OptionFactory
    {
        public static DhcpOption GetDhcpOption(int code)
        {
            DhcpOption option = null;
            switch (code)
            {
                case DhcpConstants.V4OPTION_SUBNET_MASK:    // 1
                    option = new DhcpV4SubnetMaskOption();
                    break;
                case DhcpConstants.V4OPTION_TIME_OFFSET:    // 2
                    option = new DhcpV4TimeOffsetOption();
                    break;
                case DhcpConstants.V4OPTION_ROUTERS:        // 3
                    option = new DhcpV4RoutersOption();
                    break;
                case DhcpConstants.V4OPTION_TIME_SERVERS:   // 4
                    option = new DhcpV4TimeServersOption();
                    break;
                case DhcpConstants.V4OPTION_DOMAIN_SERVERS: // 6
                    option = new DhcpV4DomainServersOption();
                    break;
                case DhcpConstants.V4OPTION_HOSTNAME:       // 12
                    option = new DhcpV4HostnameOption();
                    break;
                case DhcpConstants.V4OPTION_DOMAIN_NAME:    // 15
                    option = new DhcpV4DomainNameOption();
                    break;
                case DhcpConstants.V4OPTION_VENDOR_INFO:    // 43
                    option = new DhcpV4VendorSpecificOption();
                    break;
                case DhcpConstants.V4OPTION_NETBIOS_NAME_SERVERS:   // 44
                    option = new DhcpV4NetbiosNameServersOption();
                    break;
                case DhcpConstants.V4OPTION_NETBIOS_NODE_TYPE:  // 46
                    option = new DhcpV4NetbiosNodeTypeOption();
                    break;
                case DhcpConstants.V4OPTION_REQUESTED_IP:   // 50
                    option = new DhcpV4RequestedIpAddressOption();
                    break;
                case DhcpConstants.V4OPTION_LEASE_TIME:     // 51
                    option = new DhcpV4LeaseTimeOption();
                    break;
                case DhcpConstants.V4OPTION_MESSAGE_TYPE:   // 53
                    option = new DhcpV4MsgTypeOption();
                    break;
                case DhcpConstants.V4OPTION_SERVERID:       // 54
                    option = new DhcpV4ServerIdOption();
                    break;
                case DhcpConstants.V4OPTION_PARAM_REQUEST_LIST:     // 55
                    option = new DhcpV4ParamRequestOption();
                    break;
                case DhcpConstants.V4OPTION_VENDOR_CLASS:   // 60
                    option = new DhcpV4VendorClassOption();
                    break;
                case DhcpConstants.V4OPTION_CLIENT_ID:      // 61
                    option = new DhcpV4ClientIdOption();
                    break;
                case DhcpConstants.V4OPTION_TFTP_SERVER_NAME:   // 66
                    option = new DhcpV4TftpServerNameOption();
                    break;
                case DhcpConstants.V4OPTION_BOOT_FILE_NAME: // 67
                    option = new DhcpV4BootFileNameOption();
                    break;
                case DhcpConstants.V4OPTION_CLIENT_FQDN:    // 81
                    option = new DhcpV4ClientFqdnOption();
                    break;
                case DhcpConstants.V4OPTION_EOF:            // 255
                    break;
                default:
                    // Unknown option code, build an opaque option to hold it
                    DhcpUnknownOption unknownOption = new DhcpUnknownOption();
                    unknownOption.SetCode(code);
                    unknownOption.SetV4(true);
                    option = unknownOption;
                    break;
            }
            return option;
        }
    }
}
