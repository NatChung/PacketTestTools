/*
 * Copyright 2009-2014 Jagornet Technologies, LLC.  All Rights Reserved.
 *
 * This software is the proprietary information of Jagornet Technologies, LLC. 
 * Use is subject to license terms.
 *
 */

/*
 *   This file DhcpV6IaAddrOption.java is part of Jagornet DHCP.
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
    /**
     * The Class DhcpV6IaAddrOption.
     * 
     * @author A. Gregory Rabil
     */
    public class DhcpV6IaAddrOption : BaseDhcpOption
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string ipAddress;
        private long preferredLifetime;     // long for unsigned int
        private long validLifetime;         // long for unsigned int

        /** The dhcp options inside this ia addr option. */
        protected Dictionary<int, DhcpOption> dhcpOptions = new Dictionary<int, DhcpOption>();

        public DhcpV6IaAddrOption() : this(null)
        {
        }
        /**
	     * Instantiates a new dhcp ia addr option.
	     * 
	     * @param iaAddrOption the ia addr option
	     */
        public DhcpV6IaAddrOption(v6IaAddrOption iaAddrOption)
        {
            if (iaAddrOption != null)
            {
                this.ipAddress = iaAddrOption.ipv6Address;
                this.preferredLifetime = iaAddrOption.preferredLifetime;
                this.validLifetime = iaAddrOption.validLifetime;
            }
            SetCode(DhcpConstants.V6OPTION_IAADDR);
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

        public void SetIpAddress(IPAddress ipAddress)
        {
            this.ipAddress = ipAddress.ToString();
        }

        public long GetPreferredLifetime()
        {
            return preferredLifetime;
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

        public void PutAllDhcpOptions(Dictionary<int, DhcpOption> optionMap)
        {
            foreach (var option in optionMap)
                this.dhcpOptions[option.Key] = option.Value;
        }

        public int GetDecodedLength()
        {
            int len = 24;   // ipAddr(16) + preferred(4) + valid(4)
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
                    log.Debug("IA_ADDR option reports length=" + len +
                              ":  bytes remaining in buffer=" + buf.remaining());
                long eof = buf.position() + len;
                if (buf.position() < eof)
                {
                    ipAddress = BaseIpAddressOption.DecodeIpAddress(buf);
                    if (buf.position() < eof)
                    {
                        preferredLifetime = Util.GetUnsignedInt(buf);
                        if (buf.position() < eof)
                        {
                            validLifetime = Util.GetUnsignedInt(buf);
                            if (buf.position() < eof)
                            {
                                DecodeOptions(buf, eof);
                            }
                        }
                    }
                }
            }
        }


        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            if (ipAddress != null)
            {
                byte[] inet6AddrByte = IPAddress.Parse(ipAddress).GetAddressBytes();
                buf.put(inet6AddrByte, 0, inet6AddrByte.Length);
                buf.putInt((int)preferredLifetime);
                buf.putInt((int)validLifetime);
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

        /**
         * Decode any options sent by the client inside this IA_ADDR.  Note that this
         * should actually be a rare occurrence, in that such a client would have to
         * have requested specific IA_ADDRs inside an IA_NA/IA_TA, _and_ must have
         * requested some specific options for any such IA_ADDRs.  RFC 3315 does not
         * specify if a client can actually provide any options with requested IA_ADDRs,
         * but it does not say that the client cannot do so, and the IA_ADDR option
         * definition itself supports sub-options, thus we check for any when decoding.
         * Options within an IA_ADDR may come from a client when renewing an IA_ADDR
         * which contained options originally provided by the server, and the client is
         * requesting that those same options be renewed along with the address(es).
         * 
         * @param buf ByteBuffer positioned at the start of the options in the packet
         * @param eof the eof
         * 
         * @return a Map of DhcpOptions keyed by the option code
         * 
         * @throws IOException Signals that an I/O exception has occurred.
         */
        protected void DecodeOptions(ByteBuffer buf, long eof)
        {
            while (buf.position() < eof)
            {
                int code = Util.GetUnsignedShort(buf);
                log.Debug("Option code=" + code);
                DhcpOption option = DhcpV6OptionFactory.GetDhcpOption(code);
                if (option != null)
                {
                    option.Decode(buf);
                    PutDhcpOption(option);
                }
                else
                {
                    break;  // no more options, or one is malformed, so we're done
                }
            }
        }

        public override int GetLength()
        {
            return GetDecodedLength();
        }

        /// <summary>
        /// Implement DhcpOptionable.
        /// </summary>
        /// <param name="dhcpOption">dhcpOption the dhcp option</param>
        public void PutDhcpOption(DhcpOption dhcpOption)
        {
            dhcpOptions[dhcpOption.GetCode()] = dhcpOption;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.GetName());
            sb.Append(": ipAddress=");
            sb.Append(ipAddress);
            sb.Append(" preferredLifetime=");
            sb.Append(GetPreferredLifetime());
            sb.Append(" validLifetime=");
            sb.Append(GetValidLifetime());
            if (dhcpOptions != null && dhcpOptions.Count() > 0)
            {
                sb.Append(Util.LINE_SEPARATOR);
                sb.Append("IA_ADDR_DHCPOPTIONS");
                foreach (DhcpOption dhcpOption in dhcpOptions.Values)
                {
                    sb.Append(dhcpOption.ToString());
                }
            }
            return sb.ToString();
        }
    }
}
