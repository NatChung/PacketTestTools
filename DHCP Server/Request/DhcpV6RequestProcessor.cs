using System;
using System.Net;
using PIXIS.DHCP.Message;
using PIXIS.DHCP.Option.V6;

using PIXIS.DHCP.Utility;
using System.Collections.Generic;
using PIXIS.DHCP.Request.Bind;
using PIXIS.DHCP.DB;

namespace PIXIS.DHCP.Request
{
    public class DhcpV6RequestProcessor : BaseDhcpV6Processor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IPAddress _clientV4IP;
        public DhcpV6RequestProcessor(DhcpV6Message dhcpMessage, IPAddress linkAddress, IPAddress clientV4IP) :
            base(dhcpMessage, linkAddress)
        {
            _clientV4IP = clientV4IP;
        }
        public override bool PreProcess()
        {
            if (!base.PreProcess())
            {
                return false;
            }

            DhcpV6ServerIdOption requestedServerIdOption = requestMsg.GetDhcpServerIdOption();
            if (requestedServerIdOption == null)
            {
                log.Warn("Ignoring Request message: " +
                        "Requested ServerId option is null");
                return false;
            }

            if (!(dhcpServerIdOption.Equals(requestedServerIdOption)))
            {
                log.Warn("Ignoring Request message: " +
                         "Requested ServerId: " + requestedServerIdOption +
                         "My ServerId: " + dhcpServerIdOption);
                return false;
            }

            if (requestMsg.GetDhcpClientIdOption() == null)
            {
                log.Warn("Ignoring Request message: " +
                        "ClientId option is null");
                return false;
            }

            return true;
        }
        public override bool Process()
        {
            //       When the server receives a Request message via unicast from a client
            //       to which the server has not sent a unicast option, the server
            //       discards the Request message and responds with a Reply message
            //       containing a Status Code option with the value UseMulticast, a Server
            //       Identifier option containing the server's DUID, the Client Identifier
            //       option from the client message, and no other options.
            if (ShouldMulticast())
            {
                replyMsg.SetMessageType(DhcpConstants.V6MESSAGE_TYPE_REPLY);
                SetReplyStatus(DhcpConstants.V6STATUS_CODE_USEMULTICAST);
                return true;
            }

            //            If the server finds that the prefix on one or more IP addresses in
            //            any IA in the message from the client is not appropriate for the link
            //            to which the client is connected, the server MUST return the IA to
            //            the client with a Status Code option with the value NotOnLink.
            // 
            //            If the server cannot assign any addresses to an IA in the message
            //            from the client, the server MUST include the IA in the Reply message
            //            with no addresses in the IA and a Status Code option in the IA
            //            containing status code NoAddrsAvail.
            bool sendReply = true;
            DhcpV6ClientIdOption clientIdOption = requestMsg.GetDhcpClientIdOption();
            List<DhcpV6IaNaOption> iaNaOptions = requestMsg.GetIaNaOptions();
            if (iaNaOptions != null && iaNaOptions.Count > 0)
            {
                V6NaAddrBindingManager bindingMgr = dhcpServerConfig.GetNaAddrBindingMgr();
                if ((bindingMgr != null))
                {
                    foreach (DhcpV6IaNaOption dhcpIaNaOption in iaNaOptions)
                    {
                        log.Info(("Processing IA_NA Request: " + dhcpIaNaOption.ToString()));
                        if (!AllIaAddrsOnLink(dhcpIaNaOption, clientLink))
                        {
                            AddIaNaOptionStatusToReply(dhcpIaNaOption, DhcpConstants.V6STATUS_CODE_NOTONLINK);
                        }
                        else
                        {
                            Binding binding = bindingMgr.FindCurrentBinding(clientLink, clientIdOption, dhcpIaNaOption, requestMsg);
                            if ((binding != null))
                            {
                                binding = bindingMgr.UpdateBinding(binding, clientLink, clientIdOption, dhcpIaNaOption, requestMsg, IdentityAssoc.COMMITTED, _clientV4IP);
                                if ((binding != null))
                                {
                                    AddBindingToReply(clientLink, binding);
                                    bindings.Add(binding);
                                }
                                else
                                {
                                    AddIaNaOptionStatusToReply(dhcpIaNaOption, DhcpConstants.V6STATUS_CODE_NOADDRSAVAIL);
                                }
                            }
                            else
                            {
                                // TODO: what is the right thing to do here - we have
                                //         a request, but the solicit failed somehow?
                                //                             addIaNaOptionStatusToReply(dhcpIaNaOption,
                                //                                     DhcpConstants.STATUS_CODE_NOBINDING);
                                //  assume that if we have no binding, then there were
                                //  no addresses available to be given out on solicit
                                AddIaNaOptionStatusToReply(dhcpIaNaOption, DhcpConstants.V6STATUS_CODE_NOADDRSAVAIL);
                            }

                        }

                    }

                }
                else
                {
                    log.Error(("Unable to process IA_NA Request:" + " No NaAddrBindingManager available"));
                }

            }
            #region
            //List<DhcpV6IaTaOption> iaTaOptions = requestMsg.GetIaTaOptions();
            //if (((iaTaOptions != null)
            //            && iaTaOptions.Count > 0))
            //{
            //    V6TaAddrBindingManager bindingMgr = dhcpServerConfig.GetTaAddrBindingMgr();
            //    if ((bindingMgr != null))
            //    {
            //        foreach (DhcpV6IaTaOption dhcpIaTaOption in iaTaOptions)
            //        {
            //            log.Info(("Processing IA_TA Request: " + dhcpIaTaOption.ToString()));
            //            if (!AllIaAddrsOnLink(dhcpIaTaOption, clientLink))
            //            {
            //                AddIaTaOptionStatusToReply(dhcpIaTaOption, DhcpConstants.V6STATUS_CODE_NOTONLINK);
            //            }
            //            else
            //            {
            //                Binding binding = bindingMgr.FindCurrentBinding(clientLink, clientIdOption, dhcpIaTaOption, requestMsg);
            //                if ((binding != null))
            //                {
            //                    binding = bindingMgr.UpdateBinding(binding, clientLink, clientIdOption, dhcpIaTaOption, requestMsg, IdentityAssoc.COMMITTED, _clientV4IP);
            //                    if ((binding != null))
            //                    {
            //                        AddBindingToReply(clientLink, binding);
            //                        bindings.Add(binding);
            //                    }
            //                    else
            //                    {
            //                        AddIaTaOptionStatusToReply(dhcpIaTaOption, DhcpConstants.V6STATUS_CODE_NOADDRSAVAIL);
            //                    }

            //                }
            //                else
            //                {
            //                    // TODO: what is the right thing to do here - we have
            //                    //         a request, but the solicit failed somehow?
            //                    //                             addIaTaOptionStatusToReply(dhcpIaTaOption,
            //                    //                                     DhcpConstants.STATUS_CODE_NOBINDING);
            //                    //  assume that if we have no binding, then there were
            //                    //  no addresses available to be given out on solicit
            //                    AddIaTaOptionStatusToReply(dhcpIaTaOption, DhcpConstants.V6STATUS_CODE_NOADDRSAVAIL);
            //                }

            //            }

            //        }

            //    }
            //    else
            //    {
            //        log.Error(("Unable to process IA_TA Request:" + " No TaAddrBindingManager available"));
            //    }

            //}

            //List<DhcpV6IaPdOption> iaPdOptions = requestMsg.GetIaPdOptions();
            //if (((iaPdOptions != null)
            //            && iaPdOptions.Count > 0))
            //{
            //    V6PrefixBindingManager bindingMgr = dhcpServerConfig.GetPrefixBindingMgr();
            //    if ((bindingMgr != null))
            //    {
            //        foreach (DhcpV6IaPdOption dhcpIaPdOption in iaPdOptions)
            //        {
            //            log.Info(("Processing IA_PD Request: " + dhcpIaPdOption.ToString()));
            //            if (!AllIaPrefixesOnLink(dhcpIaPdOption, clientLink))
            //            {
            //                //  for PD return NoPrefixAvail instead of NotOnLink
            //                AddIaPdOptionStatusToReply(dhcpIaPdOption, DhcpConstants.V6STATUS_CODE_NOPREFIXAVAIL);
            //            }
            //            else
            //            {
            //                Binding binding = bindingMgr.FindCurrentBinding(clientLink, clientIdOption, dhcpIaPdOption, requestMsg);
            //                if ((binding != null))
            //                {
            //                    binding = bindingMgr.UpdateBinding(binding, clientLink, clientIdOption, dhcpIaPdOption, requestMsg, IdentityAssoc.COMMITTED, _clientV4IP);
            //                    if ((binding != null))
            //                    {
            //                        AddBindingToReply(clientLink, binding);
            //                        bindings.Add(binding);
            //                    }
            //                    else
            //                    {
            //                        //  for PD return NoPrefixAvail instead of NotOnLink
            //                        AddIaPdOptionStatusToReply(dhcpIaPdOption, DhcpConstants.V6STATUS_CODE_NOPREFIXAVAIL);
            //                    }

            //                }
            //                else
            //                {
            //                    // TODO: what is the right thing to do here - we have
            //                    //         a request, but the solicit failed somehow?
            //                    //                             addIaPdOptionStatusToReply(dhcpIaPdOption,
            //                    //                                     DhcpConstants.STATUS_CODE_NOBINDING);
            //                    //  assume that if we have no binding, then there were
            //                    //  no prefixes available to be given out on solicit
            //                    AddIaPdOptionStatusToReply(dhcpIaPdOption, DhcpConstants.V6STATUS_CODE_NOPREFIXAVAIL);
            //                }

            //            }

            //        }

            //    }
            //    else
            //    {
            //        log.Error(("Unable to process IA_PD Request:" + " No PrefixBindingManager available"));
            //    }

            //}
            #endregion
            if (sendReply)
            {
                replyMsg.SetMessageType(DhcpConstants.V6MESSAGE_TYPE_REPLY);
                if (bindings.Count > 0)
                {
                    PopulateReplyMsgOptions(clientLink);
                    ProcessDdnsUpdates(true);
                }

            }

            return sendReply;
        }
    }
}