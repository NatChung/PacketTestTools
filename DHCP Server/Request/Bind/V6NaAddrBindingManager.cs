/*
 * Copyright 2009-2014 Jagornet Technologies, LLC.  All Rights Reserved.
 *
 * This software is the proprietary information of Jagornet Technologies, LLC. 
 * Use is subject to license terms.
 *
 */

/*
 *   This file V6NaAddrBindingManager.java is part of Jagornet DHCP.
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
using PIXIS.DHCP.Config;
using PIXIS.DHCP.Message;
using PIXIS.DHCP.Option.V6;
using PIXIS.DHCP.DB;
using PIXIS.DHCP.Xml;
using System.Net;

namespace PIXIS.DHCP.Request.Bind
{
    public interface V6NaAddrBindingManager : StaticBindingManager
    {
        /**
	 * Initialize the manager.
	 * 
	 * @throws DhcpServerConfigException
	 */
        void Init();
        void UpdatePool();
        /// <summary>
        /// DHCP 派發IP前，確認IP是否已經使用
        /// </summary>
        /// <param name="ip">檢查IP</param>
        /// <param name="wait">等待時間(毫秒)</param>
        /// <returns></returns>
        Func<IPAddress, int, bool> CheckIPIsUsed { get; set; }
        /**
         * Find current binding.
         * 
         * @param clientLink the client link
         * @param clientIdOption the client id option
         * @param iaNaOption the ia na option
         * @param requestMsg the request msg
         * 
         * @return the binding
         */
        Binding FindCurrentBinding(DhcpLink clientLink, DhcpV6ClientIdOption clientIdOption,
               DhcpV6IaNaOption iaNaOption, DhcpMessage requestMsg);

        /**
         * Creates the solicit binding.
         * 
         * @param clientLink the client link
         * @param clientIdOption the client id option
         * @param iaNaOption the ia na option
         * @param requestMsg the request msg
         * @param rapidCommit the rapid commit
         * 
         * @return the binding
         */
        Binding CreateSolicitBinding(DhcpLink clientLink, DhcpV6ClientIdOption clientIdOption,
               DhcpV6IaNaOption iaNaOption, DhcpMessage requestMsg, byte state, IPAddress clientV4IPAddress);

        /**
         * Update binding.
         * 
         * @param binding the binding
         * @param clientLink the client link
         * @param clientIdOption the client id option
         * @param iaNaOption the ia na option
         * @param requestMsg the request msg
         * @param state the state
         * 
         * @return the binding
         */
        Binding UpdateBinding(Binding binding, DhcpLink clientLink,
               DhcpV6ClientIdOption clientIdOption, DhcpV6IaNaOption iaNaOption,
               DhcpMessage requestMsg, byte state, IPAddress clientV4IPAddress);

        /**
         * Release ia address.
         * 
         * @param iaAddr the ia addr
         */
        void ReleaseIaAddress(IdentityAssoc ia, IaAddress iaAddr);

        /**
         * Decline ia address.
         * 
         * @param iaAddr the ia addr
         */
        void DeclineIaAddress(IdentityAssoc ia, IaAddress iaAddr);


    }
}
