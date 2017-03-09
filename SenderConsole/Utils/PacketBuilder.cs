using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MaxP.PacketDotNet;

namespace MaxP.Arpro.Probe.Utils
{
    internal class PacketBuilder
    {
        public static Packet BuildPacket(int vlanID, EthernetPacket ePacket, Packet ethPayloadPacket)
        {
            if (vlanID != 0)
            {
                VLanTaggedPacket vlanPacket = new VLanTaggedPacket(vlanID);
                vlanPacket.PayloadPacket = ethPayloadPacket;
                ePacket.PayloadPacket = vlanPacket;
            }
            else
                ePacket.PayloadPacket = ethPayloadPacket;
            return ePacket;
        }

        public static Packet BuildPacket(int vlanID, EthernetPacket ePacket, IPv4Packet iPacket, UdpPacket uPacket, byte[] udpData)
        {
            uPacket.PayloadData = udpData;
            iPacket.PayloadPacket = uPacket;

            iPacket.UpdateIPChecksum();
            uPacket.UpdateUDPChecksum();

            return BuildPacket(vlanID, ePacket, iPacket);
        }
    }
}
