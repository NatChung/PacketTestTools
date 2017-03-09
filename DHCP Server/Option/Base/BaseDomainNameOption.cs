/*
 * Copyright 2009-2014 Jagornet Technologies, LLC.  All Rights Reserved.
 *
 * This software is the proprietary information of Jagornet Technologies, LLC. 
 * Use is subject to license terms.
 *
 */

/*
 *   This file BaseDomainNameOption.java is part of Jagornet DHCP.
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
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIXIS.DHCP.Option.V4;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.Base
{
    /**
     * Title: BaseDomainNameOption
     * Description: The abstract base class for domain name DHCP options.
     * 
     * @author A. Gregory Rabil
     */
    public class BaseDomainNameOption : BaseDhcpOption
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected string domainName;

        public BaseDomainNameOption() : base()
        {
        }
        public BaseDomainNameOption(domainNameOptionType domainNameOption) : base()
        {
            if (domainNameOption != null)
            {
                domainName = domainNameOption.domainName;
            }
        }
        /**
     * Gets the domain name length.
     * 
     * @param domainName the domain name
     * 
     * @return the domain name length
     */
        public static int GetDomainNameLength(string domainName)
        {
            int len = 0;
            if (domainName != null)
            {
                bool fqdn = domainName.EndsWith(".");
                String[] labels = domainName.Split("\\.".ToCharArray());
                if (labels != null)
                {
                    foreach (string label in labels)
                    {
                        // each label consists of a length byte and opaqueData
                        len += 1 + label.Length;
                    }
                }
                if (fqdn)
                {
                    len += 1;   // one extra byte for the zero length terminator
                }
            }
            return len;
        }

        public override void Decode(ByteBuffer buf)
        {
            int len = base.DecodeLength(buf);
            if ((len > 0) && (len <= buf.remaining()))
            {
                long eof = buf.position() + len;
                domainName = DecodeDomainName(buf, eof);
            }
        }

        public static string DecodeDomainName(ByteBuffer buf, long eof)
        {
            StringBuilder domain = new StringBuilder();
            while (buf.position() < eof)
            {
                short len = Util.GetUnsignedByte(buf);  // length byte as short
                if (len == 0)
                    break;      // terminating null "root" label
                byte[] b = buf.getBytes(len); // get next label
                domain.Append(Encoding.ASCII.GetString(b));
                if (buf.position() < eof)
                    domain.Append('.');     // build the FQDN by appending labels
            }
            return domain.ToString();
        }

        public string GetDomainName()
        {
            return domainName;
        }

        public void SetDomainName(string domainName)
        {
            this.domainName = domainName;
        }

        /**
         * Encode domain name.
         * 
         * @param buf the buf
         * @param domain the domain
         */
        public static void EncodeDomainName(ByteBuffer buf, String domain)
        {
            if (domain != null)
            {
                bool fqdn = domain.EndsWith(".");
                // we split the human-readable string representing the
                // fully-qualified domain name along the dots, which
                // gives us the list of labels that make up the FQDN
                string[] labels = domain.Split("\\.".ToCharArray());
                if (labels != null)
                {
                    foreach (string label in labels)
                    {
                        // domain names are encoded according to RFC1035 sec 3.1
                        // a 'label' consists of a length byte (i.e. octet) with
                        // the two high order bits set to zero (which means each
                        // label is limited to 63 bytes) followed by length number
                        // of bytes (i.e. octets) which make up the name
                        buf.put((byte)label.Length);
                        if (label.Length > 0)
                        {
                            var bytes = Encoding.ASCII.GetBytes(label);
                            buf.put(bytes, 0, bytes.Length);
                        }
                    }
                    if (fqdn)
                    {
                        buf.put((byte)0);    // terminate with zero-length "root" label
                    }
                }
            }
        }

        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            if (domainName != null)
            {
                EncodeDomainName(buf, domainName);
            }
            return (ByteBuffer)buf.flip();
        }

        public override int GetLength()
        {
            int len = 0;
            if (domainName != null)
            {
                len = GetDomainNameLength(domainName);
            }
            return len;
        }
    }
}
