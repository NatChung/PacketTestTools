using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Utility
{
    public class DhcpConstants
    {

        //  private static Logger log = LoggerFactory.getLogger(DhcpConstants.class);

        public static string JAGORNET_DHCP_HOME = "dhcp.home";// System.getProperty("jagornet.dhcp.home");

        public static bool IS_WINDOWS = (Environment.OSVersion.Platform == PlatformID.Win32NT) ||
            (Environment.OSVersion.Platform == PlatformID.Win32Windows);

        public static IPAddress ZEROADDR_V4 = IPAddress.Parse("0.0.0.0");

        public static IPAddress ZEROADDR_V6 = IPAddress.Parse("::");

        public static IPAddress LOCALHOST_V4 = IPAddress.Parse("127.0.0.1");

        public static IPAddress LOCALHOST_V6 = IPAddress.Parse("::1");

        public static IPAddress BROADCAST = IPAddress.Parse("255.255.255.255");

        public static IPAddress ALL_DHCP_RELAY_AGENTS_AND_SERVERS = IPAddress.Parse("FF02::1:2");
        public static IPAddress ALL_ROUTER_LISTEN_ADDRESS = IPAddress.Parse("FF02::2");

        public static IPAddress ALL_DHCP_SERVERS = IPAddress.Parse("FF05::1:3");


        public const int V6_CLIENT_PORT = 546;
        public const int V6_SERVER_PORT = 547;
        public const short V6MESSAGE_TYPE_SOLICIT = 1;
        public const short V6MESSAGE_TYPE_ADVERTISE = 2;
        public const short V6MESSAGE_TYPE_REQUEST = 3;
        public const short V6MESSAGE_TYPE_CONFIRM = 4;
        public const short V6MESSAGE_TYPE_RENEW = 5;
        public const short V6MESSAGE_TYPE_REBIND = 6;
        public const short V6MESSAGE_TYPE_REPLY = 7;
        public const short V6MESSAGE_TYPE_RELEASE = 8;
        public const short V6MESSAGE_TYPE_DECLINE = 9;
        public const short V6MESSAGE_TYPE_RECONFIGURE = 10;
        public const short V6MESSAGE_TYPE_INFO_REQUEST = 11;
        public const short V6MESSAGE_TYPE_RELAY_FORW = 12;
        public const short V6MESSAGE_TYPE_RELAY_REPL = 13;
        public static string[] V6MESSAGE_STRING = { "Unknown",
                                                    "Solicit",
                                                    "Advertise",
                                                    "Request",
                                                    "Confirm",
                                                    "Renew",
                                                    "Rebind",
                                                    "Reply",
                                                    "Release",
                                                    "Decline",
                                                    "Reconfigure",
                                                    "Info-Request",
                                                    "Relay-Forward",
                                                    "Relay-Reply" };

        public static String getV6MessageString(short msg)
        {
            if (((msg > 0)
                        && (msg <= V6MESSAGE_TYPE_RELAY_REPL)))
            {
                return V6MESSAGE_STRING[msg];
            }

            return "Unknown";
        }

        public const int V6OPTION_CLIENTID = 1;

        public const int V6OPTION_SERVERID = 2;

        public const int V6OPTION_IA_NA = 3;

        public const int V6OPTION_IA_TA = 4;

        public const int V6OPTION_IAADDR = 5;

        public const int V6OPTION_ORO = 6;

        public const int V6OPTION_PREFERENCE = 7;

        public const int V6OPTION_ELAPSED_TIME = 8;

        public const int V6OPTION_RELAY_MSG = 9;

        //  option code 10 is unassigned
        public const int V6OPTION_AUTH = 11;

        public const int V6OPTION_UNICAST = 12;

        public const int V6OPTION_STATUS_CODE = 13;

        public const int V6OPTION_RAPID_COMMIT = 14;

        public const int V6OPTION_USER_CLASS = 15;

        public const int V6OPTION_VENDOR_CLASS = 16;

        public const int V6OPTION_VENDOR_OPTS = 17;

        public const int V6OPTION_INTERFACE_ID = 18;

        public const int V6OPTION_RECONF_MSG = 19;

        public const int V6OPTION_RECONF_ACCEPT = 20;

        public const int V6OPTION_SIP_SERVERS_DOMAIN_LIST = 21;

        public const int V6OPTION_SIP_SERVERS_ADDRESS_LIST = 22;

        public const int V6OPTION_DNS_SERVERS = 23;

        public const int V6OPTION_DOMAIN_SEARCH_LIST = 24;

        public const int V6OPTION_IA_PD = 25;

        public const int V6OPTION_IA_PD_PREFIX = 26;

        public const int V6OPTION_NIS_SERVERS = 27;

        public const int V6OPTION_NISPLUS_SERVERS = 28;

        public const int V6OPTION_NIS_DOMAIN_NAME = 29;

        public const int V6OPTION_NISPLUS_DOMAIN_NAME = 30;

        public const int V6OPTION_SNTP_SERVERS = 31;

        public const int V6OPTION_INFO_REFRESH_TIME = 32;

        public const int V6OPTION_BCMCS_DOMAIN_NAMES = 33;

        public const int V6OPTION_BCMCS_ADDRESSES = 34;

        public const int V6OPTION_GEOCONF_CIVIC = 36;

        public const int V6OPTION_REMOTE_ID = 37;

        public const int V6OPTION_SUBSCRIBER_ID = 38;

        public const int V6OPTION_CLIENT_FQDN = 39;

        public const int V6OPTION_PANA_AGENT_ADDRESSES = 40;

        public const int V6OPTION_NEW_POSIX_TIMEZONE = 41;

        public const int V6OPTION_NEW_TZDB_TIMEZONE = 42;

        public const int V6OPTION_ECHO_REQUEST = 43;

        public const int V6OPTION_LOST_SERVER_DOMAIN_NAME = 51;

        public const int V6STATUS_CODE_SUCCESS = 0;

        public const int V6STATUS_CODE_UNSPEC_FAIL = 1;

        public const int V6STATUS_CODE_NOADDRSAVAIL = 2;

        public const int V6STATUS_CODE_NOBINDING = 3;

        public const int V6STATUS_CODE_NOTONLINK = 4;

        public const int V6STATUS_CODE_USEMULTICAST = 5;

        public const int V6STATUS_CODE_NOPREFIXAVAIL = 6;

        //  V4 Constants
        public const int V4_CLIENT_PORT = 68;

        public const int V4_SERVER_PORT = 67;

        public const int V4_OP_REQUEST = 1;

        public const int V4_OP_REPLY = 2;

        public const int V4MESSAGE_TYPE_DISCOVER = 1;

        public const int V4MESSAGE_TYPE_OFFER = 2;

        public const int V4MESSAGE_TYPE_REQUEST = 3;

        public const int V4MESSAGE_TYPE_DECLINE = 4;

        public const int V4MESSAGE_TYPE_ACK = 5;

        public const int V4MESSAGE_TYPE_NAK = 6;

        public const int V4MESSAGE_TYPE_RELEASE = 7;

        public const int V4MESSAGE_TYPE_INFORM = 8;

        public static string[] V4MESSAGE_STRING = { "Unknown",
                                                    "Discover",
                                                    "Offer",
                                                    "Request",
                                                    "Decline",
                                                    "Ack",
                                                    "Nack",
                                                    "Release",
                                                    "Inform" };

        public static string GetV4MessageString(short msg)
        {
            if (((msg > 0) && (msg <= V4MESSAGE_TYPE_INFORM)))
            {
                return V4MESSAGE_STRING[msg];
            }

            return "Unknown";
        }

        public const int V4OPTION_SUBNET_MASK = 1;

        public const int V4OPTION_TIME_OFFSET = 2;

        public const int V4OPTION_ROUTERS = 3;

        public const int V4OPTION_TIME_SERVERS = 4;

        public const int V4OPTION_DOMAIN_SERVERS = 6;

        public const int V4OPTION_HOSTNAME = 12;

        public const int V4OPTION_DOMAIN_NAME = 15;

        public const int V4OPTION_VENDOR_INFO = 43;

        public const int V4OPTION_NETBIOS_NAME_SERVERS = 44;

        public const int V4OPTION_NETBIOS_NODE_TYPE = 46;

        public const int V4OPTION_REQUESTED_IP = 50;

        public const int V4OPTION_LEASE_TIME = 51;

        public const int V4OPTION_MESSAGE_TYPE = 53;

        public const int V4OPTION_SERVERID = 54;

        public const int V4OPTION_PARAM_REQUEST_LIST = 55;

        public const int V4OPTION_VENDOR_CLASS = 60;

        public const int V4OPTION_CLIENT_ID = 61;

        public const int V4OPTION_TFTP_SERVER_NAME = 66;

        public const int V4OPTION_BOOT_FILE_NAME = 67;

        public const int V4OPTION_RAPID_COMMIT = 80;

        public const int V4OPTION_CLIENT_FQDN = 81;

        public const int V4OPTION_RELAY_INFO = 82;

        public const int V4OPTION_EOF = 255;
    }
}
