using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using MaxP.PacketDotNet;

namespace MaxP.Arpro.Probe.Utils
{
    internal class DNSPacketBuilder
    {
        public static Packet BuildDnsReply(IPAddress senderIp, PhysicalAddress senderMac, ushort senderPort, IPAddress targetIp, PhysicalAddress targetMac, ushort targetPort, byte[] requestData, IPAddress answerIP, int vlanId)
        {
            byte[] answerContent = new byte[requestData.Length + 16];
            requestData.CopyTo(answerContent, 0);

            int contentLen = answerContent.Length;
            // DNS Header
            answerContent[2] = 0x81; answerContent[3] = 0x80; // Flags : 81 80 (Standard Response, No Error)
            answerContent[6] = 0x00; answerContent[7] = 0x01; // Answer RRs : 1
            // DNS Answer
            answerContent[contentLen - 16] = 0xc0; answerContent[contentLen - 15] = 0x0c; // Name : c0 0c 
            answerContent[contentLen - 14] = 0x00; answerContent[contentLen - 13] = 0x01; // Type : A (Host Address)
            answerContent[contentLen - 12] = 0x00; answerContent[contentLen - 11] = 0x01; // Class : IN 
            answerContent[contentLen - 10] = 0x00; answerContent[contentLen - 9] = 0x00; answerContent[contentLen - 8] = 0x00; answerContent[contentLen - 7] = 0x05; // 5 (seconds)//0x3c; // Time To Live : 60 (seconds)
            answerContent[contentLen - 6] = 0x00; answerContent[contentLen - 5] = 0x04; // Data Length : 4

            byte[] ipBytes = answerIP.GetAddressBytes();
            answerContent[contentLen - 4] = ipBytes[0];
            answerContent[contentLen - 3] = ipBytes[1];
            answerContent[contentLen - 2] = ipBytes[2];
            answerContent[contentLen - 1] = ipBytes[3];

            EthernetPacket ePacket = new EthernetPacket(senderMac, targetMac, EthernetPacketType.IpV4);
            IPv4Packet v4Packet = new IPv4Packet(senderIp, targetIp);
            UdpPacket uPacket = new UdpPacket(senderPort, targetPort);

            return PacketBuilder.BuildPacket(vlanId, ePacket, v4Packet, uPacket, answerContent);
        }
    }
}
