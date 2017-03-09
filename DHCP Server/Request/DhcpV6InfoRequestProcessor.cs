using System;
using System.Net;
using PIXIS.DHCP.Message;

using PIXIS.DHCP.Option.V6;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Request
{
    public class DhcpV6InfoRequestProcessor : BaseDhcpV6Processor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DhcpV6InfoRequestProcessor(DhcpV6Message requestMsg, IPAddress clientLinkAddress) :
            base(requestMsg, clientLinkAddress)
        {
        }
        public override bool PreProcess()
        {
            if (!base.PreProcess())
            {
                return false;
            }

            //  this check enforced by TAHI DHCP server tests
            if (requestMsg.IsUnicast())
            {
                log.Warn("Ignoring unicast InfoRequest Message");
                return false;
            }

            //  if the client provided a ServerID option, then it MUST
            //  match our configured ServerID, otherwise ignore the request
            DhcpV6ServerIdOption requestedServerIdOption = requestMsg.GetDhcpServerIdOption();
            if (((requestedServerIdOption != null)
                        && !dhcpServerIdOption.Equals(requestedServerIdOption)))
            {
                log.Warn(("Ignoring Info-Request message: " + ("Requested ServerId: "
                                + (requestedServerIdOption + (" My ServerId: " + dhcpServerIdOption)))));
                return false;
            }

            //  if the client message has an IA option (IA_NA, IA_TA)
            //  then the DHCPv6 server must ignore the request
            if ((requestMsg.GetIaNaOptions() != null && requestMsg.GetIaNaOptions().Count > 0) 
                || ((requestMsg.GetIaTaOptions() != null && requestMsg.GetIaTaOptions().Count > 0) 
                || (requestMsg.GetIaPdOptions() != null && requestMsg.GetIaPdOptions().Count > 0)))
            {
                log.Warn(("Ignoring Info-Request message: " + " client message contains IA option(s)."));
                return false;
            }

            return true;
        }
        public override bool Process()
        {
            //            When the server receives an Information-request message, the client
            //            is requesting configuration information that does not include the
            //            assignment of any addresses.  The server determines all configuration
            //            parameters appropriate to the client, based on the server
            //            configuration policies known to the server.
            replyMsg.SetMessageType(DhcpConstants.V6MESSAGE_TYPE_REPLY);
            PopulateReplyMsgOptions(clientLink);
            return true;
        }
    }
}