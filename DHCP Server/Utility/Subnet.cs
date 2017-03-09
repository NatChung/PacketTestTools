

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Utility
{
    /*
     * Copyright 2009-2014 Jagornet Technologies, LLC.  All Rights Reserved.
     *
     * This software is the proprietary information of Jagornet Technologies, LLC.
     * Use is subject to license terms.
     *
     */

    /*
     *   This file Subnet.java is part of Jagornet DHCP.
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
    public class Subnet
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The subnet address.
        /// </summary>
        private IPAddress subnetAddress;

        /// <summary>
        /// The prefix length.
        /// </summary>
        private int prefixLength;

        /// <summary>
        /// Instantiates a new subnet.
        /// </summary>
        /// <param name="subnetAddress">subnetAddress the subnet address</param>
        /// <param name="prefixLength">prefixLength the prefix length</param>
        public Subnet(string subnetAddress, string prefixLength) :
            this(Dns.GetHostAddresses(subnetAddress)[0], int.Parse(prefixLength))
        {
        }

        /// <summary>
        /// Instantiates a new subnet.
        /// </summary>
        /// <param name="subnetAddress">subnetAddress the subnet address</param>
        /// <param name="prefixLength">prefixLength the prefix length</param>
        public Subnet(string subnetAddress, int prefixLength) : this(Dns.GetHostAddresses(subnetAddress)[0], prefixLength)
        {
        }

        /// <summary>
        /// Instantiates a new subnet.
        /// </summary>
        /// <param name="subnetAddress">subnetAddress the subnet address</param>
        /// <param name="prefixLength">prefixLength the prefix length</param>
        public Subnet(IPAddress subnetAddress, int prefixLength)
        {
            this.subnetAddress = subnetAddress;
            this.prefixLength = prefixLength;
        }

        /// <summary>
        /// Gets the subnet address.
        /// </summary>
        /// <returns>subnet address</returns>
        public IPAddress GetSubnetAddress()
        {
            return subnetAddress;
        }

        /// <summary>
        /// Sets the subnet address.
        /// </summary>
        /// <param name="subnetAddress">subnetAddress the new subnet address</param>
        public void SetSubnetAddress(IPAddress subnetAddress)
        {
            this.subnetAddress = subnetAddress;
        }

        /// <summary>
        ///  Gets the prefix length.
        /// </summary>
        /// <returns>the prefix length</returns>
        public int GetPrefixLength()
        {
            return prefixLength;
        }

        /// <summary>
        /// Sets the prefix length.
        /// </summary>
        /// <param name="prefixLength">prefixLength the new prefix length</param>
        public void SetPrefixLength(int prefixLength)
        {
            this.prefixLength = prefixLength;
        }

        /// <summary>
        /// Gets the end address.
        /// </summary>
        /// <returns>the end address</returns>
        public IPAddress GetEndAddress()
        {
            IPAddress endAddr = null;
            int maxPrefix = 0;
            if (subnetAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                maxPrefix = 32;
            }
            else
            {
                maxPrefix = 128;
            }
            BigInteger start = new BigInteger(subnetAddress.GetAddressBytes());
            // turn on each bit that isn't masked by the prefix
            // note that bit zero(0) is the lowest order bit, so
            // this loop logically moves from right to left
            for (uint i = 0; i < (maxPrefix - prefixLength); i++)
            {
                start.SetBit(i);
            }
            try
            {

                endAddr = new IPAddress(start.GetBytes());
            }
            catch (Exception ex)
            {
                log.Error("Failed to calculate subnet end address: " + ex.Message);
            }
            return endAddr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inetAddr">the IP address to check</param>
        /// <returns>Contains.  Test if an IP address falls within a subnet.</returns>
        public bool Contains(IPAddress inetAddr)
        {
            //if ((inetAddr.IsV6IP() == subnetAddress.IsV6IP()) == false) return false;
            bool rc = false;
            BigInteger start = new BigInteger(GetSubnetAddress().GetAddressBytes());
            BigInteger end = new BigInteger(GetEndAddress().GetAddressBytes());
            BigInteger addr = new BigInteger(inetAddr.GetAddressBytes());
            if ((addr.CompareTo(start) >= 0) && (addr.CompareTo(end) <= 0))
            {
                rc = true;
            }
            return rc;
        }

        public int CompareTo(Subnet that)
        {
            BigInteger thisAddr = new BigInteger(this.GetSubnetAddress().GetAddressBytes());
            BigInteger thatAddr = new BigInteger(that.GetSubnetAddress().GetAddressBytes());
            if (thisAddr.Equals(thatAddr))
            {
                int thisPrefix = this.GetPrefixLength();
                int thatPrefix = that.GetPrefixLength();
                // if we have two subnets with the same starting address
                // then the _smaller_ subnet is the one with the _larger_
                // prefix length, which logically places the more specific
                // subnet _before_ the less specific subnet in the map
                // this allows us to work from "inside-out"?
                // must negate the comparison to make bigger smaller
                return -1 * thisPrefix.CompareTo(thatPrefix);
            }
            else
            {
                // subnet addresses are different, so return
                // the standard compare for the address
                return thisAddr.CompareTo(thatAddr);
            }
        }

        public string Network()
        {
            return (subnetAddress.ToString() + "/" + prefixLength);
        }

        public override bool Equals(Object o)
        {
            if (o is Subnet)
            {
                return this.Network().Equals(((Subnet)o).Network());
            }
            else if (o is string)
            {
                return this.Network().Equals(o);
            }
            return o == this;
        }

    }
}
