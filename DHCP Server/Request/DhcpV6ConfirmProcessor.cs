using System;
using System.Net;
using PIXIS.DHCP.Message;

using PIXIS.DHCP.Option.V6;
using System.Collections.Generic;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Request
{
    internal class DhcpV6ConfirmProcessor : BaseDhcpV6Processor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DhcpV6ConfirmProcessor(DhcpV6Message requestMsg, IPAddress clientLinkAddress) :
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
                log.Warn("Ignoring unicast Confirm Message");
                return false;
            }

            if ((requestMsg.GetDhcpClientIdOption() == null))
            {
                log.Warn(("Ignoring Solicit message: " + "ClientId option is null"));
                return false;
            }

            if ((requestMsg.GetDhcpServerIdOption() != null))
            {
                log.Warn(("Ignoring Solicit message: " + "ServerId option is not null"));
                return false;
            }

            return true;
        }
        public override bool Process()
        {
            //       When the server receives a Confirm message, the server determines
            //       whether the addresses in the Confirm message are appropriate for the
            //       link to which the client is attached.  If all of the addresses in the
            //       Confirm message pass this test, the server returns a status of
            //       Success.  If any of the addresses do not pass this test, the server
            //       returns a status of NotOnLink.  If the server is unable to perform
            //       this test (for example, the server does not have information about
            //       prefixes on the link to which the client is connected), or there were
            //       no addresses in any of the IAs sent by the client, the server MUST
            //       NOT send a reply to the client.
            bool sendReply = false;
            bool allOnLink = true;
            List<DhcpV6IaNaOption> iaNaOptions = requestMsg.GetIaNaOptions();
            if ((iaNaOptions != null))
            {
                foreach (DhcpV6IaNaOption dhcpIaNaOption in iaNaOptions)
                {
                    log.Info(("Processing IA_NA Confirm: " + dhcpIaNaOption.ToString()));
                    if (((dhcpIaNaOption.GetIaAddrOptions() != null)
                                && dhcpIaNaOption.GetIaAddrOptions().Count > 0))
                    {
                        if (!AllIaAddrsOnLink(dhcpIaNaOption, clientLink))
                        {
                            //  TAHI tests want the status at the message level
                            //                          addIaNaOptionStatusToReply(dhcpIaNaOption,
                            //                                 DhcpConstants.STATUS_CODE_NOTONLINK);
                            allOnLink = false;
                            sendReply = true;
                        }
                        else
                        {
                            //  TAHI tests want the status at the message level
                            //                         addIaNaOptionStatusToReply(dhcpIaNaOption,
                            //                                 DhcpConstants.STATUS_CODE_SUCCESS);
                            sendReply = true;
                        }

                    }

                }

            }

            if (sendReply)
            {
                //  TAHI tests want the status at the message level
                if (allOnLink)
                {
                    SetReplyStatus(DhcpConstants.V6STATUS_CODE_SUCCESS);
                }
                else
                {
                    SetReplyStatus(DhcpConstants.V6STATUS_CODE_NOTONLINK);
                }

                replyMsg.SetMessageType(DhcpConstants.V6MESSAGE_TYPE_REPLY);
            }

            return sendReply;
        }
    }
}