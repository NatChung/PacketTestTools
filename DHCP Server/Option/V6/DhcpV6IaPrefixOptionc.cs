/*
 * Copyright 2009-2014 Jagornet Technologies, LLC.  All Rights Reserved.
 *
 * This software is the proprietary information of Jagornet Technologies, LLC. 
 * Use is subject to license terms.
 *
 */

/*
 *   This file DhcpV6IaPrefixOption.java is part of Jagornet DHCP.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PIXIS.DHCP.Option.Base;

using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6IaPrefixOption : BaseDhcpOption
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private short prefixLength;
        private string ipAddress;
        private long preferredLifetime;     // long for unsigned int
        private long validLifetime;         // long for unsigned int

        /** The dhcp options inside this ia prefix option. */
        protected Dictionary<int, DhcpOption> dhcpOptions = new Dictionary<int, DhcpOption>();

        /**
	     * Instantiates a new dhcp ia prefix option.
	     */
        public DhcpV6IaPrefixOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp ia prefix option.
         * 
         * @param iaPrefixOption the ia prefix option
         */
        public DhcpV6IaPrefixOption(v6IaPrefixOption iaPrefixOption) : base()
        {
            if (iaPrefixOption != null)
            {
                prefixLength = iaPrefixOption.prefixLength;
                ipAddress = iaPrefixOption.ipv6Prefix;
                preferredLifetime = iaPrefixOption.prefixLength;
                validLifetime = iaPrefixOption.validLifetime;
            }
            SetCode(DhcpConstants.V6OPTION_IA_PD_PREFIX);
        }

        public void PutAllDhcpOptions(Dictionary<int, DhcpOption> optionMap)
        {
            foreach (var option in optionMap)
                this.dhcpOptions[option.Key] = option.Value;
        }

        public void SetIpAddress(IPAddress ipAddress)
        {
            this.ipAddress = ipAddress.ToString();
        }

        public void SetPrefixLength(short prefixLength)
        {
            this.prefixLength = prefixLength;
        }

        public void SetPreferredLifetime(long preferredLifetime)
        {
            this.preferredLifetime = preferredLifetime;
        }

        public long GetValidLifetime()
        {
            return validLifetime;
        }

        public void SetValidLifetime(long validLifetime)
        {
            this.validLifetime = validLifetime;
        }

        public string GetIpAddress()
        {
            return ipAddress;
        }
        /**
	 * Gets the inet address.
	 * 
	 * @return the inet address
	 */
        public IPAddress GetInetAddress()
        {
            IPAddress inetAddr = null;
            if (ipAddress != null)
            {
                try
                {
                    inetAddr = IPAddress.Parse(ipAddress);
                }
                catch (Exception ex)
                {
                    log.Error("Invalid IP address: " + ipAddress + ": " + ex);
                }
            }
            return inetAddr;
        }
        /**
     * Gets the decoded length.
     * 
     * @return the decoded length
     */
        public int GetDecodedLength()
        {
            int len = 4 + 4 + 1 + 16;   // iaid + preferred + valid + prefix_len + prefix_addr
            if (dhcpOptions != null)
            {
                foreach (DhcpOption dhcpOption in dhcpOptions.Values)
                {
                    // code(short) + len(short) + data_len
                    len += 4 + dhcpOption.GetLength();
                }
            }
            return len;
        }

        public override void Decode(ByteBuffer buf)
        {
            if ((buf != null) && buf.hasRemaining())
            {
                // already have the code, so length is next
                int len = Util.GetUnsignedShort(buf);
                if (log.IsDebugEnabled)
                    log.Debug("IA_PREFIX option reports length=" + len +
                              ":  bytes remaining in buffer=" + buf.remaining());
                long eof = buf.position() + len;
                if (buf.position() < eof)
                {
                    preferredLifetime = Util.GetUnsignedInt(buf);
                    if (buf.position() < eof)
                    {
                        validLifetime = Util.GetUnsignedInt(buf);
                        if (buf.position() < eof)
                        {
                            prefixLength = Util.GetUnsignedByte(buf);
                            if (buf.position() < eof)
                            {
                                ipAddress = BaseIpAddressOption.DecodeIpAddress(buf);
                                if (buf.position() < eof)
                                {
                                    DecodeOptions(buf);
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void DecodeOptions(ByteBuffer buf)
        {
            while (buf.hasRemaining())
            {
                int code = Util.GetUnsignedShort(buf);
                log.Debug("Option code=" + code);
                DhcpOption option = DhcpV6OptionFactory.GetDhcpOption(code);
                if (option != null)
                {
                    option.Decode(buf);
                    dhcpOptions[option.GetCode()] = option;
                }
                else
                {
                    break;  // no more options, or one is malformed, so we're done
                }
            }
        }

        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            if (ipAddress != null)
            {
                buf.putInt((int)preferredLifetime);
                buf.putInt((int)validLifetime);
                buf.put((byte)prefixLength);
                IPAddress inet6Prefix = IPAddress.Parse(ipAddress);
                buf.put(inet6Prefix.GetAddressBytes());
                // encode the configured options
                if (dhcpOptions != null)
                {
                    foreach (DhcpOption dhcpOption in dhcpOptions.Values)
                    {
                        ByteBuffer _buf = dhcpOption.Encode();
                        if (_buf != null)
                        {
                            buf.put(_buf);
                        }
                    }
                }
            }
            return (ByteBuffer)buf.flip();
        }
        public int getLength()
        {
            return getDecodedLength();
        }

        /**
         * Gets the decoded length.
         * 
         * @return the decoded length
         */
        public int getDecodedLength()
        {
            int len = 4 + 4 + 1 + 16;   // iaid + preferred + valid + prefix_len + prefix_addr
            if (dhcpOptions != null)
            {
                foreach (DhcpOption dhcpOption in dhcpOptions.Values)
                {
                    // code(short) + len(short) + data_len
                    len += 4 + dhcpOption.GetLength();
                }
            }
            return len;
        }

        public override int GetLength()
        {
            return GetDecodedLength();
        }
    }
}
