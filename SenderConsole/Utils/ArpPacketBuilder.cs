using System;
using System.Collections.Generic;
using System.Text;
using MaxP.PacketDotNet;
using System.Net;
using System.Net.NetworkInformation;
using MaxP.Arpro.Utility;

namespace MaxP.Arpro.Probe.Utils
{
    internal class ArpPacketBuilder
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static Packet BuildArpRequest(PhysicalAddress srcMAC, PhysicalAddress dstMAC, IPAddress senderIP, IPAddress targetIP, PhysicalAddress senderMAC, PhysicalAddress targetMAC, int vlanID)
        {
            EthernetPacket ePacket = new EthernetPacket(srcMAC, dstMAC, EthernetPacketType.Arp);
            ARPPacket aPacket = new ARPPacket(ARPOperation.Request, targetMAC, targetIP, senderMAC, senderIP);
            return PacketBuilder.BuildPacket(vlanID, ePacket, aPacket);
        }
        public static Packet BuildArpReply(PhysicalAddress srcMAC, PhysicalAddress dstMAC, IPAddress senderIP, IPAddress targetIP, PhysicalAddress senderMAC, PhysicalAddress targetMAC, int vlanID)
        {
            EthernetPacket ePacket = new EthernetPacket(srcMAC, dstMAC, EthernetPacketType.Arp);
            ARPPacket aPacket = new ARPPacket(ARPOperation.Response, targetMAC, targetIP, senderMAC, senderIP);
            return PacketBuilder.BuildPacket(vlanID, ePacket, aPacket);
        }
    }
}
