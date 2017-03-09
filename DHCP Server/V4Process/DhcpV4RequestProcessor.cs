using PIXIS.DHCP.DB;
using PIXIS.DHCP.Option.V4;
using PIXIS.DHCP.Request.Bind;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.V4Process
{
    public class DhcpV4RequestProcessor : BaseDhcpV4Processor
    {
        /** The log. */
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected DhcpV4RequestedIpAddressOption requestedIpAddrOption;
        private readonly static object _lock = new object();
        protected enum RequestType
        {
            Request_Selecting,
            Request_Renewing,
            Request_Rebinding,
            Request_InitReboot
        }
        protected RequestType type;

        /**
         * Construct an DhcpV4RequestProcessor processor.
         * 
         * @param requestMsg the Request message
         * @param clientLinkAddress the client link address
         */
        public DhcpV4RequestProcessor(DhcpV4Message requestMsg, IPAddress clientLinkAddress)
            : base(requestMsg, clientLinkAddress)
        {
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.server.request.BaseDhcpProcessor#preProcess()
         */
        public override bool PreProcess()
        {
            if (!base.PreProcess())
            {
                return false;
            }

            DhcpV4ServerIdOption requestedServerIdOption = _requestMsg.GetDhcpV4ServerIdOption();
            requestedIpAddrOption = (DhcpV4RequestedIpAddressOption)
                                    _requestMsg.GetDhcpOption(DhcpConstants.V4OPTION_REQUESTED_IP);

            // first determine what KIND of request we are dealing with
            if (_requestMsg.GetCiAddr().Equals(DhcpConstants.ZEROADDR_V4))
            {
                // the ciAddr MUST be 0.0.0.0 for Init-Reboot and Selecting
                if (requestedServerIdOption == null)
                {
                    // init-reboot MUST NOT have server-id option
                    type = RequestType.Request_InitReboot;
                }
                else
                {
                    // selecting MUST have server-id option
                    type = RequestType.Request_Selecting;
                }
            }
            else
            {
                // the ciAddr MUST NOT be 0.0.0.0 for Renew and Rebind
                if (_requestMsg.IsUnicast())
                {
                    // renew is unicast
                    // NOTE: this will not happen if the v4 broadcast interface used at startup,
                    //		 but handling of DHCPv4 renew/rebind is the same anyway
                    type = RequestType.Request_Renewing;
                }
                else
                {
                    // rebind is broadcast
                    type = RequestType.Request_Rebinding;
                }
            }

            if ((type == RequestType.Request_InitReboot) || (type == RequestType.Request_Selecting))
            {
                if (requestedIpAddrOption == null)
                {
                    log.Warn("Ignoring " + type + " message: " +
                            "Requested IP option is null");
                    return false;
                }
                if (type == RequestType.Request_Selecting)
                {
                    String requestedServerId = requestedServerIdOption.GetIpAddress();
                    string myServerId = _dhcpV4ServerIdOption.GetIpAddress();
                    if (!myServerId.Equals(requestedServerId))
                    {
                        log.Warn("Ignoring " + type + " message: " +
                                 "Requested ServerId: " + requestedServerIdOption +
                                 " My ServerId: " + _dhcpV4ServerIdOption);
                        return false;
                    }
                }
            }
            else
            {   // type == Renewing or Rebinding
                if (requestedIpAddrOption != null)
                {
                    log.Warn("Ignoring " + type + " message: " +
                            "Requested IP option is not null");
                    return false;
                }
            }

            return true;
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.server.request.BaseDhcpProcessor#process()
         */
        public override bool Process()
        {
            bool sendReply = true;
            byte[] chAddr = _requestMsg.GetChAddr();
            lock (_lock)
            {
                V4AddrBindingManager bindingMgr = _dhcpServerConfig.GetV4AddrBindingMgr();
                if (bindingMgr != null)
                {
                    log.Info("Processing " + type +
                             " from chAddr=" + Util.ToHexString(chAddr) +
                             " ciAddr=" + _requestMsg.GetCiAddr().ToString() +
                             " requestedIpAddrOption=" + requestedIpAddrOption);

                    if (!AddrOnLink(requestedIpAddrOption, _clientLink))
                    {
                        log.Info("Client requested IP is off-link, returning NAK");
                        _replyMsg.SetMessageType((short)DhcpConstants.V4MESSAGE_TYPE_NAK);
                        return sendReply;
                    }
                    else
                    {
                        Binding binding = bindingMgr.FindCurrentBinding(_clientLink,
                                                                        chAddr, _requestMsg);
                        if (binding != null)
                        {
                            binding = bindingMgr.UpdateBinding(binding, _clientLink,
                                    chAddr, _requestMsg, IdentityAssoc.COMMITTED);
                            if (binding != null)
                            {
                                AddBindingToReply(_clientLink, binding);
                                _bindings.Add(binding);
                            }
                            else
                            {
                                log.Error("Failed to update binding for client: " +
                                        Util.ToHexString(chAddr));
                                sendReply = false;
                            }
                        }
                        else
                        {
                            log.Error("No Binding available for client: " +
                                    Util.ToHexString(chAddr));
                            sendReply = false;
                        }
                    }
                }
                else
                {
                    log.Error("Unable to process V4 Request:" +
                            " No V4AddrBindingManager available");
                }

                if (sendReply)
                {
                    _replyMsg.SetMessageType((short)DhcpConstants.V4MESSAGE_TYPE_ACK);
                    if (_bindings.Count > 0)
                    {
                        ProcessDdnsUpdates(true);
                    }
                }
            }
            return sendReply;
        }
    }
}
