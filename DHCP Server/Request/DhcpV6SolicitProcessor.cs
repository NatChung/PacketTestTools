using System;
using System.Net;
using PIXIS.DHCP.Message;

using PIXIS.DHCP.Option.V6;
using PIXIS.DHCP.DB;
using System.Collections.Generic;
using PIXIS.DHCP.Request.Bind;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Config;
using static PIXIS.DHCP.Config.DhcpServerPolicies;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Request
{
    public class DhcpV6SolicitProcessor : BaseDhcpV6Processor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IPAddress _clientV4IP;

        public DhcpV6SolicitProcessor(DhcpV6Message requestMsg, IPAddress clientLinkAddress, IPAddress clientV4IP) :
            base(requestMsg, clientLinkAddress)
        {
            _clientV4IP = clientV4IP;
        }

        public override bool PreProcess()
        {
            if (!base.PreProcess())
            {
                return false;
            }

            // this check enforced by TAHI DHCP server tests
            if (requestMsg.IsUnicast())
            {
                log.Warn("Ignoring unicast Solicit Message");
                return false;
            }

            DhcpV6ClientIdOption dhcpClientId = requestMsg.GetDhcpClientIdOption();
            if ((dhcpClientId == null) || (dhcpClientId.GetOpaqueData() == null) || (dhcpClientId.GetOpaqueData().GetAscii() == null))
            {
                log.Warn("Ignoring Solicit message: " + "ClientId option is null");
                return false;
            }

            if (requestMsg.GetDhcpServerIdOption() != null)
            {
                log.Warn("Ignoring Solicit message: " + "ServerId option is not null");
                return false;
            }

            return true;
        }


        public override bool Process()
        {
            //If the Solicit message from the client included one or more IA
            //options, the server MUST include IA options in the Advertise message
            //containing any addresses that would be assigned to IAs contained in
            //the Solicit message from the client.  If the client has included
            //addresses in the IAs in the Solicit message, the server uses those
            //addresses as hints about the addresses the client would like to
            //receive.

            //If the server will not assign any addresses to any IAs in a
            //subsequent Request from the client, the server MUST send an Advertise
            //message to the client that includes only a Status Code option with
            //code NoAddrsAvail and a status message for the user, a Server
            //Identifier option with the server's DUID, and a Client Identifier
            //option with the client's DUID.
            bool sendReply = true;
            bool rapidCommit = IsRapidCommit(requestMsg, clientLink.GetLink());
            byte state = rapidCommit ? IaAddress.COMMITTED : IaAddress.ADVERTISED;

            // TODO: Warning!!!, inline IF is not supported ?
            DhcpV6ClientIdOption clientIdOption = requestMsg.GetDhcpClientIdOption();
            List<DhcpV6IaNaOption> iaNaOptions = requestMsg.GetIaNaOptions();
            if (iaNaOptions != null && iaNaOptions.Count > 0)
            {
                V6NaAddrBindingManager bindingMgr = dhcpServerConfig.GetNaAddrBindingMgr();
                if (bindingMgr != null)
                {
                    foreach (DhcpV6IaNaOption dhcpIaNaOption in iaNaOptions)
                    {
                        log.Info(("Processing IA_NA Solicit: " + dhcpIaNaOption.ToString()));
                        Binding binding = bindingMgr.FindCurrentBinding(clientLink, clientIdOption, dhcpIaNaOption, requestMsg);
                        if (binding == null)
                        {
                            //  no current binding for this IA_NA, create a new one
                            binding = bindingMgr.CreateSolicitBinding(clientLink, clientIdOption, dhcpIaNaOption, requestMsg, state, _clientV4IP);
                        }
                        else
                        {
                            binding = bindingMgr.UpdateBinding(binding, clientLink, clientIdOption, dhcpIaNaOption, requestMsg, state, _clientV4IP);
                        }

                        if (binding != null)
                        {
                            //  have a good binding, put it in the reply with options
                            AddBindingToReply(clientLink, binding);
                            bindings.Add(binding);
                        }
                        else
                        {
                            //  something went wrong, report NoAddrsAvail status for IA_NA
                            //  TAHI tests want this status at the message level
                            //                         addIaNaOptionStatusToReply(dhcpIaNaOption, 
                            //                                 DhcpConstants.STATUS_CODE_NOADDRSAVAIL);
                            SetReplyStatus(DhcpConstants.V6STATUS_CODE_NOADDRSAVAIL);
                        }
                    }
                }
                else
                {
                    log.Error(("Unable to process IA_NA Solicit:" + " No NaAddrBindingManager available"));
                }
            }
            #region
            //List<DhcpV6IaTaOption> iaTaOptions = requestMsg.GetIaTaOptions();
            //if (iaTaOptions != null && iaTaOptions.Count > 0)
            //{
            //    V6TaAddrBindingManager bindingMgr = dhcpServerConfig.GetTaAddrBindingMgr();
            //    if (bindingMgr != null)
            //    {
            //        foreach (DhcpV6IaTaOption dhcpIaTaOption in iaTaOptions)
            //        {
            //            log.Info(("Processing IA_TA Solicit: " + dhcpIaTaOption.ToString()));
            //            Binding binding = bindingMgr.FindCurrentBinding(clientLink, clientIdOption, dhcpIaTaOption, requestMsg);
            //            if ((binding == null))
            //            {
            //                //  no current binding for this IA_TA, create a new one
            //                binding = bindingMgr.CreateSolicitBinding(clientLink, clientIdOption, dhcpIaTaOption, requestMsg, state, _clientV4IP);
            //            }
            //            else
            //            {
            //                binding = bindingMgr.UpdateBinding(binding, clientLink, clientIdOption, dhcpIaTaOption, requestMsg, state, _clientV4IP);
            //            }

            //            if ((binding != null))
            //            {
            //                //  have a good binding, put it in the reply with options
            //                AddBindingToReply(clientLink, binding);
            //                bindings.Add(binding);
            //            }
            //            else
            //            {
            //                //  something went wrong, report NoAddrsAvail status for IA_TA
            //                //  TAHI tests want this status at the message level
            //                //                         addIaTaOptionStatusToReply(dhcpIaTaOption, 
            //                //                                 DhcpConstants.STATUS_CODE_NOADDRSAVAIL);
            //                SetReplyStatus(DhcpConstants.V6STATUS_CODE_NOADDRSAVAIL);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        log.Error(("Unable to process IA_TA Solicit:" + " No TaAddrBindingManager available"));
            //    }
            //}

            //List<DhcpV6IaPdOption> iaPdOptions = requestMsg.GetIaPdOptions();
            //if (((iaPdOptions != null) && iaPdOptions.Count > 0))
            //{
            //    V6PrefixBindingManager bindingMgr = dhcpServerConfig.GetPrefixBindingMgr();
            //    if ((bindingMgr != null))
            //    {
            //        foreach (DhcpV6IaPdOption dhcpIaPdOption in iaPdOptions)
            //        {
            //            log.Info(("Processing IA_PD Solicit: " + dhcpIaPdOption.ToString()));
            //            Binding binding = bindingMgr.FindCurrentBinding(clientLink, clientIdOption, dhcpIaPdOption, requestMsg);
            //            if ((binding == null))
            //            {
            //                //  no current binding for this IA_PD, create a new one
            //                binding = bindingMgr.CreateSolicitBinding(clientLink, clientIdOption, dhcpIaPdOption, requestMsg, state, _clientV4IP);
            //            }
            //            else
            //            {
            //                binding = bindingMgr.UpdateBinding(binding, clientLink, clientIdOption, dhcpIaPdOption, requestMsg, state, _clientV4IP);
            //            }

            //            if ((binding != null))
            //            {
            //                //  have a good binding, put it in the reply with options
            //                AddBindingToReply(clientLink, binding);
            //                bindings.Add(binding);
            //            }
            //            else
            //            {
            //                //  something went wrong, report NoPrefixAvail status for IA_PD
            //                //  TAHI tests want this status at the message level
            //                //                         addIaPdOptionStatusToReply(dhcpIaPdOption, 
            //                //                                 DhcpConstants.STATUS_CODE_NOPREFIXAVAIL);
            //                SetReplyStatus(DhcpConstants.V6STATUS_CODE_NOPREFIXAVAIL);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        log.Error(("Unable to process IA_PD Solicit:" + " No PrefixBindingManager available"));
            //    }
            //}
            #endregion
            if (sendReply)
            {
                if (rapidCommit)
                {
                    replyMsg.SetMessageType(DhcpConstants.V6MESSAGE_TYPE_REPLY);
                }
                else
                {
                    replyMsg.SetMessageType(DhcpConstants.V6MESSAGE_TYPE_ADVERTISE);
                }

                if (bindings.Count > 0)
                {
                    PopulateReplyMsgOptions(clientLink);
                    if (rapidCommit)
                    {
                        ProcessDdnsUpdates(true);
                    }
                    else
                    {
                        ProcessDdnsUpdates(false);
                    }
                }
            }
            return sendReply;
        }

        /// <summary>
        /// 是否開啟快速提交模式
        /// </summary>
        /// <param name="requestMsg"></param>
        /// <param name="clientLink"></param>
        /// <returns></returns>
        private bool IsRapidCommit(DhcpV6Message requestMsg, link clientLink)
        {
            if ((requestMsg.HasOption(DhcpConstants.V6OPTION_RAPID_COMMIT) && DhcpServerPolicies.EffectivePolicyAsBoolean(requestMsg, clientLink, Property.SUPPORT_RAPID_COMMIT)))
            {
                return true;
            }

            return false;
        }
    }
}