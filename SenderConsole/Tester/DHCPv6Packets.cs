using MaxP.PacketDotNet;
using PIXIS.DHCP;
using PIXIS.DHCP.Message;
using PIXIS.DHCP.Option.V6;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SenderConsole.Tester
{
    abstract class DHCPv6ExpectedMessage : IExpectedPacket
    {
        protected int _expectedTransactionId;
        protected byte[] _data;

        public DHCPv6ExpectedMessage(int expectedTransactionId)
        {
            _expectedTransactionId = expectedTransactionId;
        }

        public bool IsPass(byte[] data)
        {
            _data = data;
            return assertDhcpV6Message(parseToDhcpV6Message());
        }

        protected DhcpV6Message parseToDhcpV6Message()
        {
            DhcpV6Message msg = new DhcpV6Message(null, null);
            try
            {
                msg.Decode(ByteBuffer.allocate(_data.Length).put(_data).flip());
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                msg = null;
            }

           

            return msg;
        }
        
        public byte[] GetData()
        {
            return _data;
        }

        abstract public bool assertDhcpV6Message(DhcpV6Message message);
    }

    abstract class DHCPv6SendMessage : ISendPacket
    {
        private DhcpV6Message DhcpV6MessagePacket;
        private const ushort SOURCE_PORT = 546;
        private const ushort DEST_PORT = 547;
        private static byte duid = 0;
        public DHCPv6SendMessage(int transactionId)
        {
            DhcpV6MessagePacket = new DhcpV6Message(IPAddress.Any, new IPEndPoint(IPAddress.Parse("ff02::1:2"), DEST_PORT));
            DhcpV6MessagePacket.SetTransactionId(transactionId);

            DhcpV6ClientIdOption dhcpClientId = new DhcpV6ClientIdOption();
            dhcpClientId.GetOpaqueData().SetHex(new byte[] { 0x00 , 0x01, 0x00, 0x01, 0x20, 0x39, 0xBB, 0x3F, 0x98, 0x5F, 0xD3, 0x58, 0x7D, ++duid });//.SetAscii("000100011dd31327001dbac1557b");
            DhcpV6MessagePacket.PutDhcpOption(dhcpClientId);

            DhcpV6ElapsedTimeOption dhcpElapsedTime = new DhcpV6ElapsedTimeOption();
            dhcpElapsedTime.SetUnsignedShort(1);
            DhcpV6MessagePacket.PutDhcpOption(dhcpElapsedTime);
        }

        public ushort GetDestnationPort()
        {
            return DEST_PORT;
        }

        public Packet GetSendPacket(PhysicalAddress sourceMAC, IPAddress sourceIPAddress)
        {
            // EthernetPacket ePacket = new EthernetPacket(PhysicalAddress.Parse("00-E0-4C-68-02-91"), PhysicalAddress.Parse("33-33-00-01-00-02"), EthernetPacketType.IpV6);
            // IPv6Packet ipPacket = new IPv6Packet(IPAddress.Parse("fe80::1c66:e750:c259:df88"), IPAddress.Parse("ff02::1:2"));

            EthernetPacket ePacket = new EthernetPacket(sourceMAC, PhysicalAddress.Parse("33-33-00-01-00-02"), EthernetPacketType.IpV6);
            IPv6Packet ipPacket = new IPv6Packet(sourceIPAddress, IPAddress.Parse("ff02::1:2"));
            ePacket.PayloadPacket = ipPacket;

            UdpPacket udpPacket = new UdpPacket(SOURCE_PORT, DEST_PORT);
            udpPacket.PayloadData = AddOtherOptions(DhcpV6MessagePacket).Encode().getAllBytes();
            udpPacket.UpdateCalculatedValues();

            ipPacket.PayloadPacket = udpPacket;
            udpPacket.UpdateUDPChecksum();
            Console.WriteLine("Send Type({0}) transactionID:{1:X} ", DhcpV6MessagePacket.GetMessageType().ToString(), DhcpV6MessagePacket.GetTransactionId());
            return ePacket;
        }

        public ushort GetSourcePort()
        {
            return SOURCE_PORT;
        }

        public bool IsTcp()
        {
            return false;
        }

        abstract public DhcpV6Message AddOtherOptions(DhcpV6Message dhcpV6MessagePacket);
    }
}
