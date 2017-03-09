using System;
using System.Collections.Generic;
using System.Text;
using MaxP.PacketDotNet.Utils;
using MiscUtil.Conversion;

namespace MaxP.PacketDotNet
{
    public class VLanTaggedPacket : InternetLinkLayerPacket
    {
        public override Packet PayloadPacket
        {
            get
            {
                return base.PayloadPacket;
            }

            set
            {
                base.PayloadPacket = value;

                // set Type based on the type of the payload
                if (value is IPv4Packet)
                {
                    Type = EthernetPacketType.IpV4;
                }
                else if (value is IPv6Packet)
                {
                    Type = EthernetPacketType.IpV6;
                }
                else if (value is ARPPacket)
                {
                    Type = EthernetPacketType.Arp;
                }
                else if (value is LLDPPacket)
                {
                    Type = EthernetPacketType.LLDP;
                }
                else if (value is PPPoEPacket)
                {
                    Type = EthernetPacketType.PointToPointProtocolOverEthernetSessionStage;
                }
                else if (value is VLanTaggedPacket)
                {
                    Type = EthernetPacketType.VLanTaggedFrame;
                }
                else // NOTE: new types should be inserted here
                {
                    Type = EthernetPacketType.None;
                }
            }
        }
        
        public EthernetPacketType Type
        {
            get
            {
                return (EthernetPacketType)EndianBitConverter.Big.ToInt16(header.Bytes,
                                                                          header.Offset + 2);
            }

            set
            {
                Int16 val = (Int16)value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 header.Bytes,
                                                 header.Offset + 2);
            }
        }

        public int VLanID
        {
            get 
            {
                return EndianBitConverter.Big.ToInt16With12Bits(header.Bytes, header.Offset);
            }
            set 
            {
                EndianBitConverter.Big.CopyBytes((short)value, header.Bytes, header.Offset);
            }
        }
        
        public VLanTaggedPacket(ByteArraySegment bas)
        {
            header = new ByteArraySegment(bas);
            header.Length = 4;
            payloadPacketOrData = EthernetPacket.ParseEncapsulatedBytes(header, Type);
        }

        public VLanTaggedPacket(int vlanID)
        {
            byte[] headerBytes = new byte[4];
            header = new ByteArraySegment(headerBytes, 0, 4);
            this.VLanID = vlanID;
        }
    }
}
