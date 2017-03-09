using System;
using System.Net;
using PIXIS.DHCP.Message;
using PIXIS.DHCP.Option.V6;

using PIXIS.DHCP.Utility;
using System.Collections.Generic;
using PIXIS.DHCP.Request.Bind;

namespace PIXIS.DHCP.Request
{
    internal class DhcpV6ReleaseProcessor : BaseDhcpV6Processor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DhcpV6ReleaseProcessor(DhcpV6Message requestMsg, IPAddress clientLinkAddress) :
            base(requestMsg, clientLinkAddress)
        {
        }

        public override bool PreProcess()
        {
            if (!base.PreProcess())
            {
                return false;
            }

            DhcpV6ServerIdOption requestedServerIdOption = requestMsg.GetDhcpServerIdOption();
            if ((requestedServerIdOption == null))
            {
                log.Warn(("Ignoring Release message: " + "Requested ServerId option is null"));
                return false;
            }

            if (!dhcpServerIdOption.Equals(requestedServerIdOption))
            {
                log.Warn(("Ignoring Release message: " + ("Requested ServerId: "
                                + (requestedServerIdOption + ("My ServerId: " + dhcpServerIdOption)))));
                return false;
            }

            if ((requestMsg.GetDhcpClientIdOption() == null))
            {
                log.Warn(("Ignoring Release message: " + "ClientId option is null"));
                return false;
            }

            return true;
        }

        public override bool Process()
        {
            //            When the server receives a Release message via unicast from a client
            //            to which the server has not sent a unicast option, the server
            //            discards the Release message and responds with a Reply message
            //            containing a Status Code option with value UseMulticast, a Server
            //            Identifier option containing the server's DUID, the Client Identifier
            //            option from the client message, and no other options.
            if (ShouldMulticast())
            {
                replyMsg.SetMessageType(DhcpConstants.V6MESSAGE_TYPE_REPLY);
                SetReplyStatus(DhcpConstants.V6STATUS_CODE_USEMULTICAST);
                return true;
            }

            //            Upon the receipt of a valid Release message, the server examines the
            //            IAs and the addresses in the IAs for validity.  If the IAs in the
            //            message are in a binding for the client, and the addresses in the IAs
            //            have been assigned by the server to those IAs, the server deletes the
            //            addresses from the IAs and makes the addresses available for
            //            assignment to other clients.  The server ignores addresses not
            //            assigned to the IA, although it may choose to log an error.
            // 
            //            After all the addresses have been processed, the server generates a
            //            Reply message and includes a Status Code option with value Success, a
            //            Server Identifier option with the server's DUID, and a Client
            //            Identifier option with the client's DUID.  For each IA in the Release
            //            message for which the server has no binding information, the server
            //            adds an IA option using the IAID from the Release message, and
            //            includes a Status Code option with the value NoBinding in the IA
            //            option.  No other options are included in the IA option.
            // 
            //            A server may choose to retain a record of assigned addresses and IAs
            //            after the lifetimes on the addresses have expired to allow the server
            //            to reassign the previously assigned addresses to a client.
            bool sendReply = true;
            DhcpV6ClientIdOption clientIdOption = requestMsg.GetDhcpClientIdOption();
            List<DhcpV6IaNaOption> iaNaOptions = requestMsg.GetIaNaOptions();
            if (((iaNaOptions != null)
                        && iaNaOptions.Count > 0))
            {
                V6NaAddrBindingManager bindingMgr = dhcpServerConfig.GetNaAddrBindingMgr();
                if ((bindingMgr != null))
                {
                    foreach (DhcpV6IaNaOption dhcpIaNaOption in iaNaOptions)
                    {
                        log.Info(("Processing IA_NA Release: " + dhcpIaNaOption.ToString()));
                        Binding binding = bindingMgr.FindCurrentBinding(clientLink, clientIdOption, dhcpIaNaOption, requestMsg);
                        if ((binding != null))
                        {
                            HashSet<BindingObject> bindingObjs = binding.GetBindingObjects();
                            if (((bindingObjs != null)
                                        && bindingObjs.Count > 0))
                            {
                                foreach (BindingObject bindingObj in bindingObjs)
                                {
                                    bindingMgr.ReleaseIaAddress(binding, ((V6BindingAddress)(bindingObj)));
                                }

                            }

                        }
                        else
                        {
                            AddIaNaOptionStatusToReply(dhcpIaNaOption, DhcpConstants.V6STATUS_CODE_NOBINDING);
                        }

                    }

                }
                else
                {
                    log.Error(("Unable to process IA_NA Release:" + " No NaAddrBindingManager available"));
                }

            }

            List<DhcpV6IaTaOption> iaTaOptions = requestMsg.GetIaTaOptions();
            if (((iaTaOptions != null)
                        && iaTaOptions.Count > 0))
            {
                V6TaAddrBindingManager bindingMgr = dhcpServerConfig.GetTaAddrBindingMgr();
                if ((bindingMgr != null))
                {
                    foreach (DhcpV6IaTaOption dhcpIaTaOption in iaTaOptions)
                    {
                        log.Info(("Processing IA_TA Release: " + dhcpIaTaOption.ToString()));
                        Binding binding = bindingMgr.FindCurrentBinding(clientLink, clientIdOption, dhcpIaTaOption, requestMsg);
                        if ((binding != null))
                        {
                            HashSet<BindingObject> bindingObjs = binding.GetBindingObjects();
                            if (((bindingObjs != null)
                                        && bindingObjs.Count > 0))
                            {
                                foreach (BindingObject bindingObj in bindingObjs)
                                {
                                    bindingMgr.ReleaseIaAddress(binding, ((V6BindingAddress)(bindingObj)));
                                }

                            }

                        }
                        else
                        {
                            AddIaTaOptionStatusToReply(dhcpIaTaOption, DhcpConstants.V6STATUS_CODE_NOBINDING);
                        }

                    }

                }
                else
                {
                    log.Error(("Unable to process IA_TA Release:" + " No TaAddrBindingManager available"));
                }

            }

            List<DhcpV6IaPdOption> iaPdOptions = requestMsg.GetIaPdOptions();
            if (((iaPdOptions != null)
                        && iaPdOptions.Count > 0))
            {
                V6PrefixBindingManager bindingMgr = dhcpServerConfig.GetPrefixBindingMgr();
                if ((bindingMgr != null))
                {
                    foreach (DhcpV6IaPdOption dhcpIaPdOption in iaPdOptions)
                    {
                        log.Info(("Processing IA_PD Release: " + dhcpIaPdOption.ToString()));
                        Binding binding = bindingMgr.FindCurrentBinding(clientLink, clientIdOption, dhcpIaPdOption, requestMsg);
                        if ((binding != null))
                        {
                            HashSet<BindingObject> bindingObjs = binding.GetBindingObjects();
                            if (((bindingObjs != null)
                                        && bindingObjs.Count > 0))
                            {
                                foreach (BindingObject bindingObj in bindingObjs)
                                {
                                    bindingMgr.ReleaseIaPrefix(((V6BindingPrefix)(bindingObj)));
                                }

                            }

                        }
                        else
                        {
                            AddIaPdOptionStatusToReply(dhcpIaPdOption, DhcpConstants.V6STATUS_CODE_NOBINDING);
                        }

                    }

                }
                else
                {
                    log.Error(("Unable to process IA_PD Release:" + " No PrefixBindingManager available"));
                }

            }

            if (sendReply)
            {
                replyMsg.SetMessageType(DhcpConstants.V6MESSAGE_TYPE_REPLY);
                SetReplyStatus(DhcpConstants.V6STATUS_CODE_SUCCESS);
            }

            return sendReply;
        }
    }
}