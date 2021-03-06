﻿/*
 * Copyright 2009-2014 Jagornet Technologies, LLC.  All Rights Reserved.
 *
 * This software is the proprietary information of Jagornet Technologies, LLC. 
 * Use is subject to license terms.
 *
 */

/*
 *   This file IaPrefix.java is part of Jagornet DHCP.
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

namespace PIXIS.DHCP.DB
{
    public class IaPrefix: IaAddress
    {
        protected short prefixLength;

        /**
         * Gets the prefix length.
         *
         * @return the prefix length
         */
        public short GetPrefixLength()
        {
            return prefixLength;
        }

        /**
         * Sets the prefix length.
         *
         * @param prefixLength the new prefix length
         */
        public void SetPrefixLength(short prefixLength)
        {
            this.prefixLength = prefixLength;
        }
        
    }
}
