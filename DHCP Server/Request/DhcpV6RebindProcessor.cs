using System;
using System.Net;
using PIXIS.DHCP.Message;

using PIXIS.DHCP.Option.V6;
using System.Collections.Generic;
using PIXIS.DHCP.Request.Bind;
using PIXIS.DHCP.DB;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Config;
using static PIXIS.DHCP.Config.DhcpServerPolicies;

namespace PIXIS.DHCP.Request
{
    public class DhcpV6RebindProcessor : BaseDhcpV6Processor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IPAddress _clientV4IP;
        public DhcpV6RebindProcessor(DhcpV6Message requestMsg, IPAddress clientLinkAddress, IPAddress clientV4IP)
            : base(requestMsg, clientLinkAddress)
        {
            this._clientV4IP = clientV4IP;
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
                log.Warn("Ignoring unicast Rebind Message");
                return false;
            }

            if ((requestMsg.GetDhcpClientIdOption() == null))
            {
                log.Warn(("Ignoring Rebind message: " + "ClientId option is null"));
                return false;
            }

            if ((requestMsg.GetDhcpServerIdOption() != null))
            {
                log.Warn(("Ignoring Rebind message: " + "ServerId option is not null"));
                return false;
            }

            return true;
        }
        public override bool Process()
        {
            //            If the server cannot find a client entry for the IA and the server
            //            determines that the addresses in the IA are not appropriate for the
            //            link to which the client's interface is attached according to the
            //            server's explicit configuration information, the server MAY send a
            //            Reply message to the client containing the client's IA, with the
            //            lifetimes for the addresses in the IA set to zero.  This Reply
            //            constitutes an explicit notification to the client that the addresses
            //            in the IA are no longer valid.  In this situation, if the server does
            //            not send a Reply message it silently discards the Rebind message.
            // 
            //            If the server finds that any of the addresses are no longer
            //            appropriate for the link to which the client is attached, the server
            //            returns the address to the client with lifetimes of 0.
            // 
            //            If the server finds the addresses in the IA for the client then the
            //            server SHOULD send back the IA to the client with new lifetimes and
            //            T1/T2 times.
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
                        log.Info(("Processing IA_NA Rebind: " + dhcpIaNaOption.ToString()));
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
                        else if (DhcpServerPolicies.EffectivePolicyAsBoolean(requestMsg, clientLink.GetLink(), Property.VERIFY_UNKNOWN_REBIND))
                        {
                            //  zero out the lifetimes of any invalid addresses
                            AllIaAddrsOnLink(dhcpIaNaOption, clientLink);
                            replyMsg.AddIaNaOption(dhcpIaNaOption);
                        }

                    }

                }
                else
                {
                    log.Error(("Unable to process IA_NA Rebind:" + " No NaAddrBindingManager available"));
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
                        log.Info(("Processing IA_TA Rebind: " + dhcpIaTaOption.ToString()));
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
                        else if (DhcpServerPolicies.EffectivePolicyAsBoolean(requestMsg, clientLink.GetLink(), Property.VERIFY_UNKNOWN_REBIND))
                        {
                            //  zero out the lifetimes of any invalid addresses
                            AllIaAddrsOnLink(dhcpIaTaOption, clientLink);
                            replyMsg.AddIaTaOption(dhcpIaTaOption);
                        }

                    }

                }
                else
                {
                    log.Error(("Unable to process IA_TA Rebind:" + " No TaAddrBindingManager available"));
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
                        log.Info(("Processing IA_PD Rebind: " + dhcpIaPdOption.ToString()));
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
                        else if (DhcpServerPolicies.EffectivePolicyAsBoolean(requestMsg, clientLink.GetLink(), Property.VERIFY_UNKNOWN_REBIND))
                        {
                            //  zero out the lifetimes of any invalid addresses
                            AllIaPrefixesOnLink(dhcpIaPdOption, clientLink);
                            replyMsg.AddIaPdOption(dhcpIaPdOption);
                        }

                    }

                }
                else
                {
                    log.Error(("Unable to process IA_PD Rebind:" + " No PrefixBindingManager available"));
                }

            }

            if ((bindings.Count == 0
                        && !DhcpServerPolicies.EffectivePolicyAsBoolean(requestMsg, clientLink.GetLink(), Property.VERIFY_UNKNOWN_REBIND)))
            {
                sendReply = false;
            }

            if (sendReply)
            {
                replyMsg.SetMessageType(DhcpConstants.V6MESSAGE_TYPE_REPLY);
                if (bindings.Count == 0)
                {
                    PopulateReplyMsgOptions(clientLink);
                    ProcessDdnsUpdates(true);
                }

            }

            return sendReply;
        }
    }
}