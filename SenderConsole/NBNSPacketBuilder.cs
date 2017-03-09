using MaxP.PacketDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace MaxP.Arpro.Probe.Utils
{
    internal class NBNSPacketBuilder
    {
        private static readonly byte[] _nbnsQueryHeader = new byte[] // Length : 12
        {
            0xF0, 0x0F, // Transaction ID
            0x00, 0x00, // Flag (Name Query)
            0x00, 0x01, // Questions
            0x00, 0x00, // Answer RRs
            0x00, 0x00, // Authority RRs
            0x00, 0x00, // Additional RRs
        };
        private static readonly byte[] _nbnsQueryDetail = new byte[] // Length : 38
        {
            0x20, 0x43, 0x4b, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 
            0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 
            0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 
            0x41, 0x41, 0x41, 0x00, 0x00, 0x21, 0x00, 0x01 
        };
        private static readonly byte[] _nbnsQuery = new byte[_nbnsQueryHeader.Length + _nbnsQueryDetail.Length];

        public static byte[] NBNSQueryHeader { get { return _nbnsQueryHeader; } }
        public static byte[] NBNSQuery
        {
            get
            {
                if (_nbnsQuery[0] != 0xF0) // Not initial yet
                {
                    Array.Copy(_nbnsQueryHeader, _nbnsQuery, _nbnsQueryHeader.Length);
                    Array.Copy(_nbnsQueryDetail, 0, _nbnsQuery, _nbnsQueryHeader.Length, _nbnsQueryDetail.Length);
                }
                return _nbnsQuery;
            }
        }

        public static bool IsArProNBNSQueryResponsePacket(byte[] nbnsPacket)
        {
            if (nbnsPacket.Length < 12)
            {
                return false;
            }

            return (nbnsPacket[0] == _nbnsQueryHeader[0]
                && nbnsPacket[1] == _nbnsQueryHeader[1]
                && (nbnsPacket[2] == 0x84 && nbnsPacket[3] == 0x00)       // [0x84, 0x00] --> Windows
                    || (nbnsPacket[2] == 0x85 && nbnsPacket[3] == 0x00)   // [0x85, 0x00] --> Non-Windows Device
                    || (nbnsPacket[2] == 0x85 && nbnsPacket[3] == 0x80)   // [0x85, 0x80] --> Non-Windows Device
                && nbnsPacket[4] == _nbnsQueryHeader[4]
                && nbnsPacket[5] == _nbnsQueryHeader[5]
                && nbnsPacket[6] == _nbnsQueryHeader[6]
                && nbnsPacket[7] == _nbnsQueryHeader[7]
                && nbnsPacket[8] == _nbnsQueryHeader[8]
                && nbnsPacket[9] == _nbnsQueryHeader[9]
                && nbnsPacket[10] == _nbnsQueryHeader[10]
                && nbnsPacket[11] == _nbnsQueryHeader[11]);
        }
        public static Packet BuildNBNSQuery(IPAddress srcIP, PhysicalAddress srcMAC, IPAddress dstIP, PhysicalAddress dstMAC, int vlanId)
        {
            // ??

            byte[] UdpData = new byte[NBNSQuery.Length];
            Array.Copy(NBNSQuery, 0, UdpData, 0, NBNSQuery.Length);
            UdpData[14] = 0x45;

            EthernetPacket ePacket = new EthernetPacket(srcMAC, dstMAC, EthernetPacketType.IpV4);
            IPv4Packet v4Packet = new IPv4Packet(srcIP, dstIP);
            UdpPacket uPacket = new UdpPacket(137, 137);

            return PacketBuilder.BuildPacket(vlanId, ePacket, v4Packet, uPacket, NBNSQuery);
        }
    }
}
