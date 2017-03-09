/*
 * Copyright 2009-2014 Jagornet Technologies, LLC.  All Rights Reserved.
 *
 * This software is the proprietary information of Jagornet Technologies, LLC. 
 * Use is subject to license terms.
 *
 */

/*
 *   This file Range.java is part of Jagornet DHCP.
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
using PIXIS.DHCP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Request.Bind
{


    public class Range
    {
        protected IPAddress startAddress;
        protected IPAddress endAddress;
        /**
	     * Instantiates a new range.
	     * 
	     * @param range the range
	     * @throws UnknownHostException 
	     * @throws NumberFormatException 
	     * 
	     * @throws NumberFormatException, UnknownHostException, DhcpServerConfigException
	     */
        public Range(string range)
        {
            // assume the range is in preferred CIDR notation
            String[] cidr = range.Split('/');
            if ((cidr != null) && (cidr.Length == 2))
            {
                Subnet subnet = new Subnet(cidr[0], cidr[1]);
                startAddress = subnet.GetSubnetAddress();
                endAddress = subnet.GetEndAddress();
            }
            else
            {
                // otherwise assume the range is in start-end format
                cidr = range.Split('-');
                if ((cidr != null) && (cidr.Length == 2))
                {
                    startAddress = System.Net.Dns.GetHostAddresses(cidr[0])[0];
                    endAddress = System.Net.Dns.GetHostAddresses(cidr[1])[0];
                }
                else
                {
                    throw new Exception("Failed to parse range: " + range);
                }
            }
        }

        /**
         * Instantiates a new range.
         * 
         * @param startAddress the start address
         * @param endAddress the end address
         */
        public Range(IPAddress startAddress, IPAddress endAddress)
        {
            this.startAddress = startAddress;
            this.endAddress = endAddress;
        }

        /**
         * Gets the start address.
         * 
         * @return the start address
         */
        public IPAddress GetStartAddress()
        {
            return startAddress;
        }

        /**
         * Sets the start address.
         * 
         * @param startAddress the new start address
         */
        public void SetStartAddress(IPAddress startAddress)
        {
            this.startAddress = startAddress;
        }

        /**
         * Gets the end address.
         * 
         * @return the end address
         */
        public IPAddress GetEndAddress()
        {
            return endAddress;
        }

        /**
         * Sets the end address.
         * 
         * @param endAddress the new end address
         */
        public void SetEndAddress(IPAddress endAddress)
        {
            this.endAddress = endAddress;
        }

        /**
         * Contains.
         * 
         * @param inetAddr the inet addr
         * 
         * @return true, if successful
         */
        public bool Contains(IPAddress inetAddr)
        {
            if ((Util.CompareInetAddrs(startAddress, inetAddr) <= 0) &&
                    (Util.CompareInetAddrs(endAddress, inetAddr) >= 0))
            {
                return true;
            }
            return false;
        }

        public BigInteger Size()
        {
            return new BigInteger(endAddress.GetAddressBytes()) -
                   new BigInteger(startAddress.GetAddressBytes()) +
                   new BigInteger(1);
        }
    }
}
