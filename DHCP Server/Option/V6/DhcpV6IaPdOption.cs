/*
 * Copyright 2009-2014 Jagornet Technologies, LLC.  All Rights Reserved.
 *
 * This software is the proprietary information of Jagornet Technologies, LLC. 
 * Use is subject to license terms.
 *
 */

/*
 *   This file DhcpV6IaPdOption.java is part of Jagornet DHCP.
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
using System.Text;
using System.Threading.Tasks;
using PIXIS.DHCP.Option.Base;

using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V6
{
    /**
     * The Class DhcpV6IaPdOption.
     * 
     * @author A. Gregory Rabil
     */
    public class DhcpV6IaPdOption : BaseDhcpOption
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected long iaId;
        protected long t1;
        protected long t2;

        /** The dhcp options sent by the client inside this ia pd option, _NOT_ including any requested ia prefix options. */
        protected Dictionary<int, DhcpOption> dhcpOptions = new Dictionary<int, DhcpOption>();

        /** The ia prefix options. */
        private List<DhcpV6IaPrefixOption> iaPrefixOptions = new List<DhcpV6IaPrefixOption>();

        /**
	     * Instantiates a new dhcp ia pd option.
	     */
        public DhcpV6IaPdOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp ia pd option.
         * 
         * @param iaPdOption the ia pd option
         */
        public DhcpV6IaPdOption(v6IaPdOption iaPdOption) : base()
        {
            if (iaPdOption != null)
            {
                iaId = iaPdOption.iaId;
                t1 = iaPdOption.t1;
                t1 = iaPdOption.t2;
            }
            SetCode(DhcpConstants.V6OPTION_IA_PD);
        }

        public void PutAllDhcpOptions(Dictionary<int, DhcpOption> optionMap)
        {
            foreach (var option in optionMap)
                this.dhcpOptions[option.Key] = option.Value;
        }

        public void SetIaId(long iaId)
        {
            this.iaId = iaId;
        }

        public long GetIaId()
        {
            return iaId;
        }

        public void PutDhcpOption(DhcpOption dhcpOption)
        {
            dhcpOptions[dhcpOption.GetCode()] = dhcpOption;
        }

        public void SetIaPrefixOptions(List<DhcpV6IaPrefixOption> iaPrefixOptions)
        {
            this.iaPrefixOptions = iaPrefixOptions;
        }

        public void SetT1(long t1)
        {
            this.t1 = t1;
        }

        public void SetT2(long t2)
        {
            this.t2 = t2;
        }

        public long GetT2()
        {
            return t2;
        }

        public long GetT1()
        {
            return t1;
        }

        /**
	     * Gets the ia prefix options.
	     * 
	     * @return the ia prefix options
	     */
        public List<DhcpV6IaPrefixOption> GetIaPrefixOptions()
        {
            return iaPrefixOptions;
        }

        public override int GetLength()
        {
            return GetDecodedLength();
        }

        /**
     * Gets the decoded length.
     * 
     * @return the decoded length
     */
        public int GetDecodedLength()
        {
            int len = 4 + 4 + 4;    // iaId + t1 + t2
            if (iaPrefixOptions != null)
            {
                foreach (DhcpV6IaPrefixOption iaPrefixOption in iaPrefixOptions)
                {
                    // code(short) + len(short) + data_len
                    len += 4 + iaPrefixOption.GetDecodedLength();
                }
            }
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
                    log.Debug("IA_PD option reports length=" + len +
                              ":  bytes remaining in buffer=" + buf.remaining());
                long eof = buf.position() + len;
                if (buf.position() < eof)
                {
                    iaId = Util.GetUnsignedInt(buf);
                    if (buf.position() < eof)
                    {
                        t1 = Util.GetUnsignedInt(buf);
                        if (buf.position() < eof)
                        {
                            t2 = Util.GetUnsignedInt(buf);
                            if (buf.position() < eof)
                            {
                                DecodeOptions(buf, eof);
                            }
                        }
                    }
                }
            }
        }
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
                    if (option is DhcpV6IaPrefixOption)
                    {
                        iaPrefixOptions.Add((DhcpV6IaPrefixOption)option);
                    }
                    else
                    {
                        dhcpOptions[option.GetCode()] = option;
                    }
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
            buf.putInt((int)iaId);
            buf.putInt((int)t1);
            buf.putInt((int)t2);

            if (iaPrefixOptions != null)
            {
                foreach (DhcpV6IaPrefixOption iaPrefixOption in iaPrefixOptions)
                {
                    ByteBuffer _buf = iaPrefixOption.Encode();
                    if (_buf != null)
                    {
                        buf.put(_buf);
                    }
                }
            }
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
            return (ByteBuffer)buf.flip();

        }
    }
}
