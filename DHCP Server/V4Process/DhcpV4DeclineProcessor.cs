using PIXIS.DHCP.Option.V4;
using PIXIS.DHCP.Request.Bind;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Message;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.V4Process
{
    public class DhcpV4DeclineProcessor : BaseDhcpV4Processor
    {
        //Copyright 2009-2014 Jagornet Technologies, LLC.  All Rights Reserved. 

        //This software is the proprietary information of Jagornet Technologies, LLC.  
        //Use is subject to license terms. 
        //This file DhcpV4DeclineProcessor.java is part of Jagornet DHCP. 

        //Jagornet DHCP is free software: you can redistribute it and/or modify 
        //it under the terms of the GNU General Public License as published by 
        //the Free Software Foundation, either version 3 of the License, or 
        //(at your option) any later version. 

        //Jagornet DHCP is distributed in the hope that it will be useful, 
        //but WITHOUT ANY WARRANTY; without even the implied warranty of 
        //MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
        //GNU General Public License for more details. 

        //You should have received a copy of the GNU General Public License 
        //along with Jagornet DHCP.  If not, see <http://www.gnu.org/licenses/>. 

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected DhcpV4RequestedIpAddressOption requestedIpAddrOption;

        public DhcpV4DeclineProcessor(DhcpV4Message requestMsg, IPAddress clientLinkAddress)
            : base(requestMsg, clientLinkAddress)
        {
        }

        public override bool PreProcess()
        {
            if (!base.PreProcess())
            {
                return false;
            }

            requestedIpAddrOption = (DhcpV4RequestedIpAddressOption)_requestMsg.GetDhcpOption(DhcpConstants.V4OPTION_REQUESTED_IP);
            if (requestedIpAddrOption == null)
            {
                log.Warn("Ignoring Decline message: " + "Requested IP option is null");
                return false;
            }
            return true;
        }

        public override bool Process()
        {
            byte[] chAddr = _requestMsg.GetChAddr();

            V4AddrBindingManager bindingMgr = _dhcpServerConfig.GetV4AddrBindingMgr();
            if (bindingMgr != null)
            {
                log.Info("Processing Decline" + " from chAddr=" + Util.ToHexString(chAddr) + " requestedIp=" + requestedIpAddrOption.GetIpAddress());
                Binding binding = bindingMgr.FindCurrentBinding(_clientLink, chAddr, _requestMsg);
                if (binding != null)
                {
                    HashSet<BindingObject> bindingObjs = binding.GetBindingObjects();
                    if ((bindingObjs != null) && bindingObjs.Count != 0)
                    {
                        V4BindingAddress bindingAddr = (V4BindingAddress)bindingObjs.First();
                        bindingMgr.DeclineIaAddress(binding, bindingAddr);
                    }
                    else
                    {
                        log.Error("No binding addresses in binding for client: " + Util.ToHexString(chAddr));
                    }
                }
                else
                {
                    log.Error("No Binding available for client: " + Util.ToHexString(chAddr));
                }
            }
            else
            {
                log.Error("Unable to process V4 Decline:" + " No V4AddrBindingManager available");
            }

            return false; // no reply for v4 decline 
        }
    }
}
