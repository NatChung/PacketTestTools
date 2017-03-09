/*
 * Copyright 2009-2014 Jagornet Technologies, LLC.  All Rights Reserved.
 *
 * This software is the proprietary information of Jagornet Technologies, LLC. 
 * Use is subject to license terms.
 *
 */

/*
 *   This file DhcpV6RelayMessage.java is part of Jagornet DHCP.
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
using PIXIS.DHCP.Option.V6;
using PIXIS.DHCP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

/**
 * Title: DhcpV6RelayMessage
 * Description:Object that represents a DHCPv6 relay message as defined in
 * RFC 3315.
 * 
 * There are two relay agent messages, which share the following format:
 * 
 * <pre>
 * 0                   1                   2                   3
 * 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
 * +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 * |    msg-type   |   hop-count   |                               |
 * +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+                               |
 * |                                                               |
 * |                         link-address                          |
 * |                                                               |
 * |                               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-|
 * |                               |                               |
 * +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+                               |
 * |                                                               |
 * |                         peer-address                          |
 * |                                                               |
 * |                               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-|
 * |                               |                               |
 * +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+                               |
 * .                                                               .
 * .            options (variable number and length)   ....        .
 * |                                                               |
 * +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 * 
 * The following sections describe the use of the Relay Agent message
 * header.
 * 
 * 7.1. Relay-forward Message
 * 
 * The following table defines the use of message fields in a Relay-
 * forward message.
 * 
 * msg-type       RELAY-FORW
 * 
 * hop-count      Number of relay agents that have relayed this
 * message.
 * 
 * link-address   A global or site-local address that will be used by
 * the server to identify the link on which the client
 * is located.
 * 
 * peer-address   The address of the client or relay agent from which
 * the message to be relayed was received.
 * 
 * options        MUST include a "Relay Message option" (see
 * section 22.10); MAY include other options added by
 * the relay agent.
 * 
 * 7.2. Relay-reply Message
 * 
 * The following table defines the use of message fields in a
 * Relay-reply message.
 * 
 * msg-type       RELAY-REPL
 * 
 * hop-count      Copied from the Relay-forward message
 * 
 * link-address   Copied from the Relay-forward message
 * 
 * peer-address   Copied from the Relay-forward message
 * 
 * options        MUST include a "Relay Message option"; see
 * section 22.10; MAY include other options
 * </pre>
 * 
 * @author A. Gregory Rabil
 */

namespace PIXIS.DHCP.Message
{
    public class DhcpV6RelayMessage : DhcpV6Message
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /** The hop count.  Need a short to hold unsigned byte. */
        protected short hopCount = 0;

        /** The link address.  A global or site-local address that will be used by
         *  the server to identify the link on which the client is located. */
        protected IPAddress linkAddress = null;

        /** The peer address.  The address of the client or relay agent from which
         *  the message to be relayed was received. */
        protected IPAddress peerAddress = null;

        /** The relay option.  MUST have a relay option to be a relay message.
         * this object is here as a convenience, because
         * it must ALSO be in the dhcpOptions Map that is
         * in the DhcpMessage superclass */
        protected DhcpV6RelayOption relayOption = null;

        /**
        * Construct a DhcpRelayMessage.
        * 
        * @param localAddress InetSocketAddress on the local host on which
        * this message is received or sent
        * @param remoteAddress InetSocketAddress on the remote host on which
        * this message is sent or received
        */
        public DhcpV6RelayMessage(IPAddress localAddress, IPEndPoint remoteAddress) :
                base(localAddress, remoteAddress)
        {
        }
       
        public override int GetLength()
        {
            int len = 34;
            //  relay msg type (1) + hop count (1) +
            //  link addr (16) + peer addr (16)
            len = (len + GetOptionsLength());
            return len;
        }

        public short GetHopCount()
        {
            return this.hopCount;
        }

        public void SetHopCount(short hopCount)
        {
            this.hopCount = hopCount;
        }

        public IPAddress GetLinkAddress()
        {
            return this.linkAddress;
        }

        public void SetLinkAddress(IPAddress linkAddress)
        {
            this.linkAddress = linkAddress;
        }

        public IPAddress GetPeerAddress()
        {
            return this.peerAddress;
        }

        public void SetPeerAddress(IPAddress peerAddress)
        {
            this.peerAddress = peerAddress;
        }

        public DhcpV6RelayOption GetRelayOption()
        {
            return this.relayOption;
        }

        public void SetRelayOption(DhcpV6RelayOption relayOption)
        {
            this.relayOption = relayOption;
        }
    }
}
