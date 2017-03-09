using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPcap;
using MaxP.PacketDotNet;
using System.Net.NetworkInformation;
using System.Net;
using PIXIS.DHCP.Message;
using PIXIS.DHCP.Option.V6;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP;

namespace SenderConsole.Tester
{
    class DHCPv6Solicit : DHCPv6SendMessage
    {
        public DHCPv6Solicit(int transactionId):base(transactionId) { }
        public override DhcpV6Message AddOtherOptions(DhcpV6Message dhcpV6MessagePacket)
        {
            dhcpV6MessagePacket.SetMessageType(DhcpConstants.V6MESSAGE_TYPE_SOLICIT);
            dhcpV6MessagePacket.SetIaNaOptions(new List<DhcpV6IaNaOption>() { new DhcpV6IaNaOption(new v6IaNaOption() { iaId = 285220282, t1 = 0, t2 = 0 }) });
            return dhcpV6MessagePacket;
        }
    }

    class DHCPv6Advertisement : DHCPv6ExpectedMessage
    {
        public DHCPv6Advertisement(int expectedTransactionId):base(expectedTransactionId) {}

        public override bool assertDhcpV6Message(DhcpV6Message message)
        {
            if (message != null && message.GetMessageType() == DhcpConstants.V6MESSAGE_TYPE_ADVERTISE && message.GetTransactionId() == _expectedTransactionId)
            {
                Console.WriteLine("Got  Type({0}) transactionID:{1:X} Passed", message.GetMessageType(), message.GetTransactionId());
                return true;
            }

            return false;
        }
    }

    class DHCPv6Request : DHCPv6SendMessage
    {
        private IExpectedPacket _replyPacke;
        public DHCPv6Request(int transactionId, IExpectedPacket replyPacket):base(transactionId)
        {
            _replyPacke = replyPacket;
        }

        public override DhcpV6Message AddOtherOptions(DhcpV6Message dhcpV6MessagePacket)
        {
            DhcpV6Message replyDhcpV6Message = new DhcpV6Message(null, null);
            byte[] replyData = _replyPacke.GetData();
            replyDhcpV6Message.Decode(ByteBuffer.allocate(replyData.Length).put(replyData).flip());

            dhcpV6MessagePacket.SetMessageType(DhcpConstants.V6MESSAGE_TYPE_REQUEST);
            dhcpV6MessagePacket.PutDhcpOption(replyDhcpV6Message.GetDhcpServerIdOption());
            dhcpV6MessagePacket.SetIaNaOptions(replyDhcpV6Message.GetIaNaOptions());

            return dhcpV6MessagePacket;
        }
    }

    class DHCPv6Reply : DHCPv6ExpectedMessage
    {
        private static int count = 0;
        public DHCPv6Reply(int expectedTransactionId) : base(expectedTransactionId) {}
        public override bool assertDhcpV6Message(DhcpV6Message message)
        {
            if (message != null && message.GetMessageType() == DhcpConstants.V6MESSAGE_TYPE_REPLY && message.GetTransactionId() == _expectedTransactionId)
            {
                Console.WriteLine("Got  Type({0}) transactionID:{1:X} Passed, COUNT({2})", message.GetMessageType(), message.GetTransactionId(), (++count));
                return true;
            }

            return false;
        }
    }

    class DHCPv6Tester : BaseTester
    {
        public DHCPv6Tester(ISendPacket sendPacket, IExpectedPacket expectedPacket, BaseTester next) : base(sendPacket, expectedPacket, next) {}
        public override void Start(ITestDevice device)
        {
            DoNext(device);
        }
    }
}
