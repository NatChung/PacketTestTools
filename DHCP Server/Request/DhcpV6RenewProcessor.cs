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
    internal class DhcpV6RenewProcessor : BaseDhcpV6Processor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IPAddress _clientV4IP;
        public DhcpV6RenewProcessor(DhcpV6Message requestMsg, IPAddress clientLinkAddres, IPAddress clientV4IP) :
             base(requestMsg, clientLinkAddres)
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
            if ((requestedServerIdOption == null))
            {
                log.Warn(("Ignoring Renew message: " + "Requested ServerId option is null"));
                return false;
            }

            if (!dhcpServerIdOption.Equals(requestedServerIdOption))
            {
                log.Warn(("Ignoring Renew message: " + ("Requested ServerId: "
                                + (requestedServerIdOption + ("My ServerId: " + dhcpServerIdOption)))));
                return false;
            }

            if ((requestMsg.GetDhcpClientIdOption() == null))
            {
                log.Warn(("Ignoring Renew message: " + "ClientId option is null"));
                return false;
            }

            return true;
        }

        public override bool Process()
        {
            //       When the server receives a Renew message via unicast from a client
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

            //         If the server cannot find a client entry for the IA the server
            //         returns the IA containing no addresses with a Status Code option set
            //         to NoBinding in the Reply message.
            // 
            //         If the server finds that any of the addresses are not appropriate for
            //         the link to which the client is attached, the server returns the
            //         address to the client with lifetimes of 0.
            // 
            //         If the server finds the addresses in the IA for the client then the
            //         server sends back the IA to the client with new lifetimes and T1/T2
            //         times.  The server may choose to change the list of addresses and the
            //         lifetimes of addresses in IAs that are returned to the client.
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
                        log.Info(("Processing IA_NA Renew: " + dhcpIaNaOption.ToString()));
                        Binding binding = bindingMgr.FindCurrentBinding(clientLink, clientIdOption, dhcpIaNaOption, requestMsg);
                        if ((binding != null))
                        {
                            //  zero out the lifetimes of any invalid addresses
                            if (!AllIaAddrsOnLink(dhcpIaNaOption, clientLink))
                            {
                                replyMsg.AddIaNaOption(dhcpIaNaOption);
                            }
                            else
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

                        }
                        else
                        {
                            AddIaNaOptionStatusToReply(dhcpIaNaOption, DhcpConstants.V6STATUS_CODE_NOBINDING);
                        }

                    }

                }
                else
                {
                    log.Error(("Unable to process IA_NA Renew:" + " No NaAddrBindingManager available"));
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
                        log.Info(("Processing IA_TA Renew: " + dhcpIaTaOption.ToString()));
                        Binding binding = bindingMgr.FindCurrentBinding(clientLink, clientIdOption, dhcpIaTaOption, requestMsg);
                        if ((binding != null))
                        {
                            //  zero out the lifetimes of any invalid addresses
                            if (!AllIaAddrsOnLink(dhcpIaTaOption, clientLink))
                            {
                                replyMsg.AddIaTaOption(dhcpIaTaOption);
                            }
                            else
                            {
                                binding = bindingMgr.UpdateBinding(binding, clientLink, clientIdOption, dhcpIaTaOption, requestMsg, IdentityAssoc.COMMITTED, _clientV4IP);
                                if ((binding != null))
                                {
                                    AddBindingToReply(clientLink, binding);
                                    bindings.Add(binding);
                                }
                                else
                                {
                                    AddIaTaOptionStatusToReply(dhcpIaTaOption, DhcpConstants.V6STATUS_CODE_NOADDRSAVAIL);
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
                    log.Error(("Unable to process IA_TA Renew:" + " No TaAddrBindingManager available"));
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
                        log.Info(("Processing IA_PD Renew: " + dhcpIaPdOption.ToString()));
                        Binding binding = bindingMgr.FindCurrentBinding(clientLink, clientIdOption, dhcpIaPdOption, requestMsg);
                        if ((binding != null))
                        {
                            //  zero out the lifetimes of any invalid addresses
                            if (!AllIaPrefixesOnLink(dhcpIaPdOption, clientLink))
                            {
                                replyMsg.AddIaPdOption(dhcpIaPdOption);
                            }
                            else
                            {
                                binding = bindingMgr.UpdateBinding(binding, clientLink, clientIdOption, dhcpIaPdOption, requestMsg, IdentityAssoc.COMMITTED, _clientV4IP);
                                if ((binding != null))
                                {
                                    AddBindingToReply(clientLink, binding);
                                    bindings.Add(binding);
                                }
                                else
                                {
                                    AddIaPdOptionStatusToReply(dhcpIaPdOption, DhcpConstants.V6STATUS_CODE_NOPREFIXAVAIL);
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
                    log.Error(("Unable to process IA_PD Renew:" + " No PrefixBindingManager available"));
                }

            }

            if (sendReply)
            {
                replyMsg.SetMessageType(DhcpConstants.V6MESSAGE_TYPE_REPLY);
                if (bindings.Count > 0)
                {
                    PopulateReplyMsgOptions(clientLink);
                    ProcessDdnsUpdates(true);
                }
                else
                {
                    log.Warn("Reply message has no bindings");
                }

            }

            return sendReply;
        }
    }
}