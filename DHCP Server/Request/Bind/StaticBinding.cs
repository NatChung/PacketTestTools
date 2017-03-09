/*
 * Copyright 2009-2014 Jagornet Technologies, LLC.  All Rights Reserved.
 *
 * This software is the proprietary information of Jagornet Technologies, LLC. 
 * Use is subject to license terms.
 *
 */

/*
 *   This file StaticBinding.java is part of Jagornet DHCP.
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
using PIXIS.DHCP.Config;
using PIXIS.DHCP.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Request.Bind
{
    public abstract class StaticBinding : DhcpConfigObject
    {

        public abstract bool Matches(byte[] duid, byte iatype, long iaid,
                DhcpMessage requestMsg);

        public abstract string GetIpAddress();

        public IPAddress GetInetAddress()
        {
            string ip = GetIpAddress();
            if (ip != null)
            {
                try
                {
                    return IPAddress.Parse(ip);
                }
                catch (Exception ex)
                {
                    //TODO
                }
            }
            return null;
        }

        public List<filter> GetFilters()
        {
            // no filters for static binding
            return null;
        }

        // all times are infinite for static bindings


        public long GetPreferredLifetime()
        {
            return 0xffffffff;
        }

        public long GetValidLifetime()
        {
            return 0xffffffff;
        }

        public long GetPreferredLifetimeMs()
        {
            return 0xffffffff;
        }

        public long GetValidLifetimeMs()
        {
            return 0xffffffff;
        }

        public virtual List<Policy> GetPolicies()
        {
            return null;
        }
    }
}
