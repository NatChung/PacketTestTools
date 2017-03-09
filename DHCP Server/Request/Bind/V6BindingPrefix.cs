/*
 * Copyright 2009-2014 Jagornet Technologies, LLC.  All Rights Reserved.
 *
 * This software is the proprietary information of Jagornet Technologies, LLC. 
 * Use is subject to license terms.
 *
 */

/*
 *   This file V6BindingPrefix.java is part of Jagornet DHCP.
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
using PIXIS.DHCP.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Request.Bind
{
    public class V6BindingPrefix : IaPrefix, BindingObject
    {
        private DhcpConfigObject configObj;

        /**
         * Instantiates a new binding address.
         * 
         * @param iaPrefix the ia addr
         * @param configObj the config object
         */
        public V6BindingPrefix(IaPrefix iaPrefix, DhcpConfigObject configObj)
        {
            // populate *this* IaPrefix from the given one
            this.SetDhcpOptions(iaPrefix.GetDhcpOptions());
            this.SetId(iaPrefix.GetId());
            this.SetIdentityAssocId(iaPrefix.GetIdentityAssocId());
            this.SetIpAddress(iaPrefix.GetIpAddress());
            this.SetPreferredEndTime(iaPrefix.GetPreferredEndTime());
            this.SetPrefixLength(iaPrefix.GetPrefixLength());
            this.SetStartTime(iaPrefix.GetStartTime());
            this.SetState(iaPrefix.GetState());
            this.SetValidEndTime(iaPrefix.GetValidEndTime());
            this.configObj = configObj;
        }

        public DhcpConfigObject GetConfigObj()
        {
            return configObj;
        }

        public void SetConfigObj(DhcpV6OptionConfigObject configObj)
        {
            this.configObj = configObj;
        }
    }
}
