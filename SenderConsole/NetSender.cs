using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using MaxP.Arpro.Probe.Utils;
using MaxP.PacketDotNet;
using SharpPcap;
using MaxP.Arpro.Utility;
using MaxP.Arpro.Probe.ND;

namespace MaxP.Arpro.Probe
{
    internal class NetSender : INetSender
    {
        private ICaptureDevice _ncard;
        private PhysicalAddress _probeMAC;
        //private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public NetSender(PhysicalAddress probeMAC, NICHelper nicHelper)
        {
            _ncard = nicHelper.GetRawNIC(probeMAC);
            _ncard.Open();
            _probeMAC = probeMAC;
        }

        public void SendARPRequest(IPAddress senderIP, PhysicalAddress senderMAC, IPAddress targetIP, int vlanID)
        {
            _ncard.SendPacket(ArpPacketBuilder.BuildArpRequest(_probeMAC, NetAddress.BroadcastMAC, senderIP, targetIP, senderMAC, NetAddress.ZeroMAC, vlanID));
        }
        public void SendARPReply(IPAddress senderIP, PhysicalAddress senderMAC, IPAddress targetIP, PhysicalAddress targetMAC, int vlanID)
        {
            _ncard.SendPacket(ArpPacketBuilder.BuildArpReply(_probeMAC, targetMAC, senderIP, targetIP, senderMAC, targetMAC, vlanID));
        }
        public void UnicastARPRequest(PhysicalAddress dstMAC, IPAddress senderIP, PhysicalAddress senderMAC, IPAddress targetIP, int vlanID)
        {
            _ncard.SendPacket(ArpPacketBuilder.BuildArpRequest(_probeMAC, dstMAC, senderIP, targetIP, senderMAC, NetAddress.ZeroMAC, vlanID));
        }
        public void BroadcastARPReply(IPAddress senderIP, PhysicalAddress senderMAC, IPAddress targetIP, PhysicalAddress targetMAC, int vlanID)
        {
            _ncard.SendPacket(ArpPacketBuilder.BuildArpReply(_probeMAC, NetAddress.BroadcastMAC, senderIP, targetIP, senderMAC, targetMAC, vlanID));
        }
        public void FixSelfRecord(IPAddress fixIp, PhysicalAddress fixMac)
        {
            if (!fixMac.Equals(_probeMAC))
            {
                _ncard.SendPacket(ArpPacketBuilder.BuildArpReply(_probeMAC, _probeMAC, fixIp, IPAddress.Loopback, fixMac, _probeMAC, 0));
            }
        }
        public void SendDnsReply(IPAddress srcIP, ushort srcPort, IPAddress dstIP, PhysicalAddress dstMAC, ushort dstPort, byte[] requestingPayload, IPAddress answer, int vlanID)
        {
            Packet dnsPacket = DNSPacketBuilder.BuildDnsReply(srcIP, _probeMAC, srcPort, dstIP, dstMAC, dstPort, requestingPayload, answer, vlanID);
            _ncard.SendPacket(dnsPacket);
        }
        public void SendDHCPDiscover(IPAddress managementIp, int vlanID)
        {
            Packet discoverPacket = DHCPPacketBuilder.BuildDHCPDiscoverPacket(managementIp, _probeMAC, vlanID);
            _ncard.SendPacket(discoverPacket);
        }
        public void SendNBNSQuery(IPAddress srcIP, PhysicalAddress srcMAC, IPAddress dstIP, PhysicalAddress dstMAC, int vlanID)
        {
            Packet nbnsQueryPacket = NBNSPacketBuilder.BuildNBNSQuery(srcIP, srcMAC, dstIP, dstMAC, vlanID);
            _ncard.SendPacket(nbnsQueryPacket);
        }
        public void SendNA(PhysicalAddress dst, IPAddress srcIP, IPAddress dstIP, IPAddress targetIP, PhysicalAddress targetMAC, int vlanID)
        {
            Packet naPacket = NDP.BuildNAPacket(_probeMAC, dst, srcIP, dstIP, targetIP, targetMAC, NAType.Normal, vlanID);
            _ncard.SendPacket(naPacket);
        }
        public void BroadcastNA(IPAddress targetIP, PhysicalAddress targetMAC, int vlanID)
        {
            //Packet naPacket = NDP.BuildNAPacket(_probeMAC, NetAddress.MAC3333, targetIP, NetAddress.FF02, targetIP, targetMAC, NAType.Brocast, vlanID);
            Packet naPacket = NDP.BuildNAPacket(PhysicalAddress.Parse("98-5F-D3-3F-6D-B1"), NetAddress.MAC3333, targetIP, NetAddress.FF02, NetAddress.FF02, NetAddress.MAC3333, NAType.Brocast, vlanID);
            _ncard.SendPacket(naPacket);
        }
        public void SendNSPacket()
        {
            // 發送 NS TO FF01 DST MAC 333300000001
            Packet ndpPacket = NDP.BuildNSPacket(PhysicalAddress.Parse("98-5F-D3-3F-6D-B1"),PhysicalAddress.Parse("33-33-00-00-00-01"), IPAddress.Parse("fe80::d0d6:8c11:a087:3941"), NetAddress.FF02, NetAddress.FF02, PhysicalAddress.Parse("98-5F-D3-3F-6D-B1"), 0);


            //Packet ndpPacket = NDP.BuildNSPacket(PhysicalAddress.Parse("98-5F-D3-3F-6D-B1"), NetAddress.MAC3333, IPAddress.Parse("2001:b030:1128:161::102"), NetAddress.FF02, NetAddress.FF02, NetAddress.MAC3333, 0);
            

            //Packet ndpPacket = NDP.BuildNSPacket(PhysicalAddress.Parse("98-5F-D3-3F-6D-B1"), PhysicalAddress.Parse("33-33-FF-00-01-3D"), IPAddress.Parse("fe80::d0d6:8c11:a087:3941"), NetAddress.FF02, NetAddress.FF02, PhysicalAddress.Parse("98-5F-D3-3F-6D-B1"), 0);
            //Packet ndpPacket = NDP.BuildNSPacket(PhysicalAddress.Parse("98-5F-D3-3F-6D-B1"), NetAddress.BroadcastMAC, IPAddress.Parse("2001:b030:1128:161::102"), IPAddress.Parse("ff02::2"), NetAddress.FF02, PhysicalAddress.Parse("98-5F-D3-3F-6D-B1"), 0);
            _ncard.SendPacket(ndpPacket);
        }
        public void SendNAPacket()
        {
            //Packet naPacket = NDP.BuildNAPacket(_probeMAC, NetAddress.MAC3333, targetIP, NetAddress.FF02, targetIP, targetMAC, NAType.Brocast, vlanID);
            Packet naPacket = NDP.BuildNAPacket(PhysicalAddress.Parse("98-5F-D3-3F-6D-B1"), NetAddress.MAC3333, IPAddress.Parse("2001:b030:1128:161::102"), NetAddress.FF02, NetAddress.FF02, NetAddress.MAC3333, NAType.Brocast, 0);
            _ncard.SendPacket(naPacket);
        }
        public void SendMLDQuery()
        {
            EthernetPacket ePacket = new EthernetPacket(PhysicalAddress.Parse("98-5F-D3-3F-6D-B1"), PhysicalAddress.Parse("33-33-00-00-00-01"), EthernetPacketType.IpV6);
            IPv6Packet ipPacket = new IPv6Packet(IPAddress.Parse("fe80::d0d6:8c11:a087:3941"), IPAddress.Parse("ff02::1"));
            ePacket.PayloadPacket = ipPacket;
            ICMPv6Packet icmp = new ICMPv6Packet(new PacketDotNet.Utils.ByteArraySegment(new byte[24]));
            Array.Copy(IPAddress.Parse("ff02::1").GetAddressBytes(), 0, icmp.Header, 8, 16);
            icmp.Type = ICMPv6Types.MulticastLostenerQuery;
            icmp.Code = 0;
            icmp.Checksum = 0;
            ipPacket.PayloadPacket = icmp;
            icmp.UpdateCalculatedValues();

            _ncard.SendPacket(ePacket);
        }
    }
}
