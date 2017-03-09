/*
 * Copyright 2009-2014 Jagornet Technologies, LLC.  All Rights Reserved.
 *
 * This software is the proprietary information of Jagornet Technologies, LLC. 
 * Use is subject to license terms.
 *
 */

/*
 *   This file DhcpV6ClientFqdnOption.java is part of Jagornet DHCP.
 *
 *   Jagornet DHCP is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   Jagornet DHCP is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with Jagornet DHCP.  If not, see <http://www.gnu.org/licenses/>.
 *
 */
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6ClientFqdnOption : BaseDomainNameOption
    {

        /**
	     * From RFC 4704:
	     * 
	     * 4.1.  The Flags Field
	     * 
	     *   The format of the Flags field is:
	     *        0 1 2 3 4 5 6 7
	     *       +-+-+-+-+-+-+-+-+
	     *       |  MBZ    |N|O|S|
	     *       +-+-+-+-+-+-+-+-+
	     * 
	     */
        // need short to handle unsigned byte
        private short flags;

        /**
	     * Instantiates a new dhcp client fqdn option.
	     */
        public DhcpV6ClientFqdnOption() : this(null)
        {
        }

        /**
	     * Instantiates a new dhcp client fqdn option.
	     * 
	     * @param clientFqdnOption the client fqdn option
	     */
        public DhcpV6ClientFqdnOption(v6ClientFqdnOption clientFqdnOption) : base(clientFqdnOption)
        {
            SetCode(DhcpConstants.V6OPTION_CLIENT_FQDN);
        }

        public short GetFlags()
        {
            return flags;
        }

        public void SetFlags(short flags)
        {
            this.flags = flags;
        }
        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.DhcpOption#getLength()
         */
        public override int GetLength()
        {
            int len = 1 + base.GetLength();  // size of flags (byte)
            return len;
        }


        public void SetUpdateAaaaBit(bool bit)
        {
            if (bit)
                SetFlags((short)(GetFlags() | 0x01));   // 0001
            else
                SetFlags((short)(GetFlags() & 0x06));	// 0110
        }

        public void SetOverrideBit(bool bit)
        {
            if (bit)
                SetFlags((short)(GetFlags() | 0x02));   // 0010
            else
                SetFlags((short)(GetFlags() & 0x05));	// 0101
        }

        public void SetNoUpdateBit(bool bit)
        {
            if (bit)
            {
                SetFlags((short)(GetFlags() | 0x04));   // 0100
                                                        // If the "N" bit is 1, the "S" bit MUST be 0.
                SetUpdateAaaaBit(false);
            }
            else
            {
                SetFlags((short)(GetFlags() & 0x03));   // 0011
            }
        }

        public bool GetNoUpdateBit()
        {
            short nbit = (short)(GetFlags() & 0x04);
            return (nbit == 1);
        }

        public bool GetUpdateAaaaBit()
        {
            short sbit = (short)(GetFlags() & 0x01);
            return (sbit > 0);
        }

        public override void Decode(ByteBuffer buf)
        {

            int len = base.DecodeLength(buf);
            if ((len > 0) && (len <= buf.remaining()))
            {
                long eof = buf.position() + len;
                if (buf.position() < eof)
                {
                    SetFlags(Util.GetUnsignedByte(buf));
                    string domain = DecodeDomainName(buf, eof);
                    SetDomainName(domain);
                }
            }
        }
        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            buf.put((byte)GetFlags());
            if (GetDomainName() != null)
            {
                EncodeDomainName(buf, GetDomainName());
            }
            return (ByteBuffer)buf.flip();
        }
    }
}
