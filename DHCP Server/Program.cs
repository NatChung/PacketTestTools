using PIXIS.DHCP.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using PIXIS.DHCP.Option.V6;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Config;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Request.Bind;
using PIXIS.DHCP.V4Process;
using PIXIS.DHCP.Option.V4;
using System.Net.NetworkInformation;
using PIXIS.DHCP.Test;
using PIXIS.DHCP.Message;

namespace PIXIS.DHCP
{
    public class Program
    {
        static DhcpServerConfiguration config = new DhcpServerConfiguration();
        static V4AddrBindingManagerImpl v4Manager = new V4AddrBindingManagerImpl();
        static IPAddress serverAddr = IPAddress.Parse("192.168.61.48");
        static int serverPort = DhcpConstants.V4_SERVER_PORT;
        public static void Main(string[] args)
        {
            ///設定 POOL
            //192.168.101.0/24 (192.168.101.0-192.168.101.255)
            Dictionary<Subnet, DhcpLink> linkMap = new Dictionary<Subnet, DhcpLink>();
            Subnet v6subnet = new Subnet("fe80::e5dc:c884:7691:5104", 128);
            Subnet v4subnet = new Subnet("192.168.61.0", 24);
            link v6link = new link()
            {
                name = "Local IPv6 Client Link(Multicast traffic)",
                Address = "",
                v6MsgConfigOptions = new v6ConfigOptionsType()
                {
                    v6DnsServersOption = new v6DnsServersOption()
                    {
                        ipAddress = new List<string>() { "2001:b000:168::1" }
                    }
                },
                v6NaAddrPools = new List<v6AddressPool>() { new v6AddressPool() {
                    range = "2001:b030:1128:161:0:0:1:1-2001:b030:1128:161:0:0:1:200" } }
            };
            link v4link = new link()
            {
                name = "IPv4 Client Link 1",
                Address = "192.168.61.0/24",
                v4ConfigOptions = new v4ConfigOptionsType()
                {
                    v4SubnetMaskOption = new v4SubnetMaskOption()
                    {
                        ipAddress = "255.255.255.0"
                    },
                    v4RoutersOption = new v4RoutersOption()
                    {
                        ipAddress = new List<string>() { "192.168.61.254" }
                    }
                },
                v4AddrPools = new List<v4AddressPool>() {
                     new v4AddressPool() {  range="192.168.61.1-192.168.61.240"}
                 }
            };
            linkMap[v6subnet] = new DhcpLink(v6subnet, v6link);
            linkMap[v4subnet] = new DhcpLink(v4subnet, v4link);
            config.SetLinkMap(linkMap);

            v6ServerIdOption dhcpServerId = new v6ServerIdOption();
            dhcpServerId.opaqueData.asciiValue = "000100011dd31327001dbac1557b";
            config.SetV6ServerIdOption(dhcpServerId);

            //StartV4Test();
            //V6Solicit();
            //V6Request();
            //V6Release();
           // new DhcpServer().Start();
            Console.Read();
        }

        private static void StartV4Test()
        {
            v4Manager.Init();
            config.SetV4AddrBindingMgr(v4Manager);

            V4Discover();
            V4Request();
            V4Release();
        }

        private static void V4Discover()
        {
            var V4Message = new DhcpV4Message(IPAddress.Any,
                new IPEndPoint(IPAddress.Parse("192.168.61.48"), DhcpConstants.V4_SERVER_PORT));

            V4Message.SetOp((short)DhcpConstants.V4_OP_REQUEST); //V4_OP_REQUEST
            V4Message.SetGiAddr(IPAddress.Any);
            V4Message.SetChAddr(PhysicalAddress.Parse("9C-EB-E8-28-92-D4").GetAddressBytes());
            V4Message.SetHlen(6);
            V4Message.SetHtype((byte)6);
            V4Message.SetTransactionId(-1729018559);

            DhcpV4MsgTypeOption msgTypeOption = new DhcpV4MsgTypeOption();
            msgTypeOption.SetUnsignedByte((short)DhcpConstants.V4MESSAGE_TYPE_DISCOVER);
            V4Message.PutDhcpOption(msgTypeOption);

            var message = DhcpV4MessageHandler.HandleMessage(
                 IPAddress.Parse("192.168.61.48"), V4Message);
            Console.WriteLine(message.ToString());
        }
        private static void V4Request()
        {
            var V4Message = new DhcpV4Message(IPAddress.Any,
                new IPEndPoint(IPAddress.Parse("192.168.61.48"), DhcpConstants.V4_SERVER_PORT));
            V4Message.SetOp((short)DhcpConstants.V4_OP_REQUEST); //V4_OP_REQUEST
            V4Message.SetGiAddr(IPAddress.Any);
            V4Message.SetChAddr(PhysicalAddress.Parse("9C-EB-E8-28-92-D4").GetAddressBytes());

            DhcpV4MsgTypeOption msgTypeOption = new DhcpV4MsgTypeOption();
            msgTypeOption.SetUnsignedByte((short)DhcpConstants.V4MESSAGE_TYPE_REQUEST);
            V4Message.PutDhcpOption(msgTypeOption);

            V4Message.PutDhcpOption(new DhcpV4RequestedIpAddressOption(
                new v4RequestedIpAddressOption()
                {
                    code = DhcpConstants.V4OPTION_REQUESTED_IP,
                    ipAddress = "192.168.61.140"
                }));
            V4Message.SetDhcpV4ServerIdOption(
                new DhcpV4ServerIdOption(
                    new v4ServerIdOption()
                    {
                        code = DhcpConstants.V4OPTION_SERVERID,
                        ipAddress = "192.168.61.48"
                    }));
            var message = DhcpV4MessageHandler.HandleMessage(
                 IPAddress.Parse("192.168.61.48"), V4Message);
            Console.WriteLine(message.ToString());
        }

        private static void V4Release()
        {
            var V4Message = new DhcpV4Message(IPAddress.Any,
                new IPEndPoint(IPAddress.Parse("192.168.61.48"), DhcpConstants.V4_SERVER_PORT));
            V4Message.SetOp((short)DhcpConstants.V4_OP_REQUEST); //V4_OP_REQUEST
            V4Message.SetGiAddr(IPAddress.Parse("192.168.101.1"));
            V4Message.SetCiAddr(IPAddress.Parse("192.168.5.126"));
            V4Message.SetChAddr(PhysicalAddress.Parse("9C-EB-E8-28-92-D4").GetAddressBytes());
            V4Message.SetHlen(6);
            V4Message.SetHtype((byte)6);
            V4Message.SetTransactionId(-1729018559);

            DhcpV4MsgTypeOption msgTypeOption = new DhcpV4MsgTypeOption();
            msgTypeOption.SetUnsignedByte((short)DhcpConstants.V4MESSAGE_TYPE_RELEASE);
            V4Message.PutDhcpOption(msgTypeOption);

            var message = DhcpV4MessageHandler.HandleMessage(
                 IPAddress.Parse("192.168.61.48"), V4Message);
            Console.WriteLine(V4Message.ToString());
        }

        private static void V6Solicit()
        {
            DhcpV6Message msg = new DhcpV6Message(IPAddress.Any, new IPEndPoint(serverAddr, serverPort));

            msg.SetTransactionId(-1729018559);
            DhcpV6ClientIdOption dhcpClientId = new DhcpV6ClientIdOption();
            dhcpClientId.GetOpaqueData().SetAscii("000100011dd31327001dbac1557b");

            msg.PutDhcpOption(dhcpClientId);

            DhcpV6ElapsedTimeOption dhcpElapsedTime = new DhcpV6ElapsedTimeOption();
            dhcpElapsedTime.SetUnsignedShort(1);
            msg.PutDhcpOption(dhcpElapsedTime);

            msg.SetMessageType(DhcpConstants.V6MESSAGE_TYPE_SOLICIT);
            msg.SetIaNaOptions(new List<DhcpV6IaNaOption>() { new DhcpV6IaNaOption(new v6IaNaOption() { iaId = 285220282, t1 = 0, t2 = 0 }) });

            var message = DhcpV6MessageHandler.HandleMessage(serverAddr, msg);

            Console.WriteLine(message.ToString());
        }

        private static void V6Request()
        {

            DhcpV6Message msg = new DhcpV6Message(IPAddress.Any, new IPEndPoint(serverAddr, serverPort));

            msg.SetTransactionId(-1729018559);
            //msg.PutDhcpOption(advertisement.getDhcpClientIdOption());
            DhcpV6ClientIdOption dhcpClientId = new DhcpV6ClientIdOption();
            dhcpClientId.GetOpaqueData().SetAscii("000100011dd31327001dbac1557b");
            msg.PutDhcpOption(dhcpClientId);

            DhcpV6ServerIdOption dhcpServerId = new DhcpV6ServerIdOption();
            dhcpServerId.GetOpaqueData().SetAscii("000100011dd31327001dbac1557b");
            msg.PutDhcpOption(dhcpServerId);

            msg.SetMessageType(DhcpConstants.V6MESSAGE_TYPE_REQUEST);
            msg.SetIaNaOptions(new List<DhcpV6IaNaOption>() { new DhcpV6IaNaOption(new v6IaNaOption() { iaId = 285220282, t1 = 0, t2 = 0 } ) });

            var message = DhcpV6MessageHandler.HandleMessage(
                serverAddr, msg);

            Console.WriteLine(message.ToString());
        }

        private static void V6Release()
        {

            DhcpV6Message msg = new DhcpV6Message(IPAddress.Any, new IPEndPoint(serverAddr, serverPort));

            msg.SetTransactionId(-1729018559);

            DhcpV6ClientIdOption dhcpClientId = new DhcpV6ClientIdOption();
            dhcpClientId.GetOpaqueData().SetAscii("000100011dd31327001dbac1557b");
            msg.PutDhcpOption(dhcpClientId);

            DhcpV6ServerIdOption dhcpServerId = new DhcpV6ServerIdOption();
            dhcpServerId.GetOpaqueData().SetAscii("000100011dd31327001dbac1557b");
            msg.PutDhcpOption(dhcpServerId);
            msg.SetMessageType(DhcpConstants.V6MESSAGE_TYPE_RELEASE);
            msg.SetIaNaOptions(new List<DhcpV6IaNaOption>() { new DhcpV6IaNaOption(new v6IaNaOption() { iaId = 285220282, t1 = 0, t2 = 0 }) });
            var message = DhcpV6MessageHandler.HandleMessage(
               serverAddr, msg);

            Console.WriteLine(message.ToString());

        }
    }
}
