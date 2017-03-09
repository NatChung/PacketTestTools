using PIXIS.DHCP.Config;
using PIXIS.DHCP.Message;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Option.V6;
using PIXIS.DHCP.Request.Bind;
using PIXIS.DHCP.Request.Dns;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using static PIXIS.DHCP.Config.DhcpServerPolicies;

namespace PIXIS.DHCP.Request
{
    public abstract class BaseDhcpV6Processor : DhcpV6MessageProcessor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected static DhcpServerConfiguration dhcpServerConfig = new DhcpServerConfiguration();

        //  wrap the configured ServerId option in a DhcpOption for the wire
        protected static DhcpV6ServerIdOption dhcpServerIdOption = 
            new DhcpV6ServerIdOption(dhcpServerConfig.GetDhcpServerConfig().v6ServerIdOption);

        protected DhcpV6Message requestMsg;

        protected DhcpV6Message replyMsg;

        protected IPAddress clientLinkAddress;

        protected DhcpLink clientLink;

        protected List<Binding> bindings = new List<Binding>();

        //protected static Set<DhcpV6Message> recentMsgs = Collections.synchronizedSet(new HashSet<DhcpV6Message>());

        //protected static Timer recentMsgPruner = new Timer("RecentMsgPruner");

        protected BaseDhcpV6Processor(DhcpV6Message requestMsg, IPAddress clientLinkAddress)
        {
            this.requestMsg = requestMsg;
            this.clientLinkAddress = clientLinkAddress;
        }

        protected Dictionary<int, DhcpOption> RequestedOptions(Dictionary<int, DhcpOption> optionMap, DhcpV6Message requestMsg)
        {
            if (((optionMap != null)
                        && optionMap.Count() > 0))
            {
                List<int> requestedCodes = this.requestMsg.GetRequestedOptionCodes();
                if (((requestedCodes != null)
                            && requestedCodes.Count() > 0))
                {
                    Dictionary<int, DhcpOption> _optionMap = new Dictionary<int, DhcpOption>();
                    foreach (var option in optionMap)
                    {
                        if (requestedCodes.Contains(option.Key))
                        {
                            _optionMap.Add(option.Key, option.Value);
                        }

                    }

                    optionMap = _optionMap;
                }

            }

            return optionMap;
        }

        protected void PopulateReplyMsgOptions()
        {
            Dictionary<int, DhcpOption> optionMap = dhcpServerConfig.EffectiveMsgOptions(this.requestMsg);
            if (DhcpServerPolicies.GlobalPolicyAsBoolean(Property.SEND_REQUESTED_OPTIONS_ONLY))
            {
                optionMap = this.RequestedOptions(optionMap, this.requestMsg);
            }

            this.replyMsg.PutAllDhcpOptions(optionMap);
        }

        /// <summary>
        /// Populate reply msg options.
        /// </summary>
        /// <param name="dhcpLink">the link</param>
        protected void PopulateReplyMsgOptions(DhcpLink dhcpLink)
        {
            Dictionary<int, DhcpOption> optionMap = dhcpServerConfig.EffectiveMsgOptions(this.requestMsg, dhcpLink);
            if (DhcpServerPolicies.EffectivePolicyAsBoolean(dhcpLink.GetLink(), Property.SEND_REQUESTED_OPTIONS_ONLY))
            {
                optionMap = this.RequestedOptions(optionMap, this.requestMsg);
            }

            replyMsg.PutAllDhcpOptions(optionMap);
        }

        protected void PopulateIaNaOptions(DhcpV6IaNaOption iaNaOption)
        {
            Dictionary<int, DhcpOption> optionMap = dhcpServerConfig.EffectiveIaNaOptions(this.requestMsg);
            if (DhcpServerPolicies.GlobalPolicyAsBoolean(Property.SEND_REQUESTED_OPTIONS_ONLY))
            {
                optionMap = this.RequestedOptions(optionMap, this.requestMsg);
            }

            iaNaOption.PutAllDhcpOptions(optionMap);
        }

        protected void PopulateIaTaOptions(DhcpV6IaTaOption iaTaOption)
        {
            Dictionary<int, DhcpOption> optionMap = dhcpServerConfig.EffectiveIaTaOptions(this.requestMsg);
            if (DhcpServerPolicies.GlobalPolicyAsBoolean(Property.SEND_REQUESTED_OPTIONS_ONLY))
            {
                optionMap = this.RequestedOptions(optionMap, this.requestMsg);
            }

            iaTaOption.PutAllDhcpOptions(optionMap);
        }

        protected void PopulateIaPdOptions(DhcpV6IaPdOption iaPdOption)
        {
            Dictionary<int, DhcpOption> optionMap = dhcpServerConfig.EffectiveIaPdOptions(this.requestMsg);
            if (DhcpServerPolicies.GlobalPolicyAsBoolean(Property.SEND_REQUESTED_OPTIONS_ONLY))
            {
                optionMap = this.RequestedOptions(optionMap, this.requestMsg);
            }

            iaPdOption.PutAllDhcpOptions(optionMap);
        }

        protected void PopulateIaNaOptions(DhcpV6IaNaOption iaNaOption, DhcpLink dhcpLink)
        {
            Dictionary<int, DhcpOption> optionMap = dhcpServerConfig.EffectiveIaNaOptions(this.requestMsg, dhcpLink);
            if (DhcpServerPolicies.EffectivePolicyAsBoolean(dhcpLink.GetLink(), Property.SEND_REQUESTED_OPTIONS_ONLY))
            {
                optionMap = this.RequestedOptions(optionMap, this.requestMsg);
            }

            iaNaOption.PutAllDhcpOptions(optionMap);
        }

        protected void PopulateIaTaOptions(DhcpV6IaTaOption iaTaOption, DhcpLink dhcpLink)
        {
            Dictionary<int, DhcpOption> optionMap = dhcpServerConfig.EffectiveIaTaOptions(this.requestMsg, dhcpLink);
            if (DhcpServerPolicies.EffectivePolicyAsBoolean(dhcpLink.GetLink(), Property.SEND_REQUESTED_OPTIONS_ONLY))
            {
                optionMap = this.RequestedOptions(optionMap, this.requestMsg);
            }

            iaTaOption.PutAllDhcpOptions(optionMap);
        }

        protected void PopulateIaPdOptions(DhcpV6IaPdOption iaPdOption, DhcpLink dhcpLink)
        {
            Dictionary<int, DhcpOption> optionMap = dhcpServerConfig.EffectiveIaPdOptions(this.requestMsg, dhcpLink);
            if (DhcpServerPolicies.EffectivePolicyAsBoolean(dhcpLink.GetLink(), Property.SEND_REQUESTED_OPTIONS_ONLY))
            {
                optionMap = this.RequestedOptions(optionMap, this.requestMsg);
            }

            iaPdOption.PutAllDhcpOptions(optionMap);
        }

        protected void PopulateNaAddrOptions(DhcpV6IaAddrOption iaAddrOption)
        {
            Dictionary<int, DhcpOption> optionMap = dhcpServerConfig.EffectiveNaAddrOptions(this.requestMsg);
            if (DhcpServerPolicies.GlobalPolicyAsBoolean(Property.SEND_REQUESTED_OPTIONS_ONLY))
            {
                optionMap = this.RequestedOptions(optionMap, this.requestMsg);
            }

            iaAddrOption.PutAllDhcpOptions(optionMap);
        }

        protected void PopulateTaAddrOptions(DhcpV6IaAddrOption iaAddrOption)
        {
            Dictionary<int, DhcpOption> optionMap = dhcpServerConfig.EffectiveTaAddrOptions(this.requestMsg);
            if (DhcpServerPolicies.GlobalPolicyAsBoolean(Property.SEND_REQUESTED_OPTIONS_ONLY))
            {
                optionMap = this.RequestedOptions(optionMap, this.requestMsg);
            }

            iaAddrOption.PutAllDhcpOptions(optionMap);
        }

        protected void PopulatePrefixOptions(DhcpV6IaPrefixOption iaPrefixOption)
        {
            Dictionary<int, DhcpOption> optionMap = dhcpServerConfig.EffectivePrefixOptions(this.requestMsg);
            if (DhcpServerPolicies.GlobalPolicyAsBoolean(Property.SEND_REQUESTED_OPTIONS_ONLY))
            {
                optionMap = this.RequestedOptions(optionMap, this.requestMsg);
            }

            iaPrefixOption.PutAllDhcpOptions(optionMap);
        }

        protected void PopulateNaAddrOptions(DhcpV6IaAddrOption iaAddrOption, DhcpLink dhcpLink)
        {
            Dictionary<int, DhcpOption> optionMap = dhcpServerConfig.EffectiveNaAddrOptions(this.requestMsg, dhcpLink);
            if (DhcpServerPolicies.EffectivePolicyAsBoolean(dhcpLink.GetLink(), Property.SEND_REQUESTED_OPTIONS_ONLY))
            {
                optionMap = this.RequestedOptions(optionMap, this.requestMsg);
            }

            iaAddrOption.PutAllDhcpOptions(optionMap);
        }

        protected void PopulateTaAddrOptions(DhcpV6IaAddrOption iaAddrOption, DhcpLink dhcpLink)
        {
            Dictionary<int, DhcpOption> optionMap = dhcpServerConfig.EffectiveTaAddrOptions(this.requestMsg, dhcpLink);
            if (DhcpServerPolicies.EffectivePolicyAsBoolean(dhcpLink.GetLink(), Property.SEND_REQUESTED_OPTIONS_ONLY))
            {
                optionMap = this.RequestedOptions(optionMap, this.requestMsg);
            }

            iaAddrOption.PutAllDhcpOptions(optionMap);
        }

        protected void PopulatePrefixOptions(DhcpV6IaPrefixOption iaPrefixOption, DhcpLink dhcpLink)
        {
            Dictionary<int, DhcpOption> optionMap = dhcpServerConfig.EffectivePrefixOptions(this.requestMsg, dhcpLink);
            if (DhcpServerPolicies.EffectivePolicyAsBoolean(dhcpLink.GetLink(), Property.SEND_REQUESTED_OPTIONS_ONLY))
            {
                optionMap = this.RequestedOptions(optionMap, this.requestMsg);
            }

            iaPrefixOption.PutAllDhcpOptions(optionMap);
        }

        protected void PopulateNaAddrOptions(DhcpV6IaAddrOption iaAddrOption, DhcpLink dhcpLink, DhcpV6OptionConfigObject configObj)
        {
            Dictionary<int, DhcpOption> optionMap = dhcpServerConfig.EffectiveNaAddrOptions(this.requestMsg, dhcpLink, configObj);
            if (DhcpServerPolicies.EffectivePolicyAsBoolean(configObj, dhcpLink.GetLink(), Property.SEND_REQUESTED_OPTIONS_ONLY))
            {
                optionMap = this.RequestedOptions(optionMap, this.requestMsg);
            }

            iaAddrOption.PutAllDhcpOptions(optionMap);
        }

        protected void PopulateTaAddrOptions(DhcpV6IaAddrOption iaAddrOption, DhcpLink dhcpLink, DhcpV6OptionConfigObject configObj)
        {
            Dictionary<int, DhcpOption> optionMap = dhcpServerConfig.EffectiveTaAddrOptions(this.requestMsg, dhcpLink, configObj);
            if (DhcpServerPolicies.EffectivePolicyAsBoolean(configObj, dhcpLink.GetLink(), Property.SEND_REQUESTED_OPTIONS_ONLY))
            {
                optionMap = this.RequestedOptions(optionMap, this.requestMsg);
            }

            iaAddrOption.PutAllDhcpOptions(optionMap);
        }

        protected void PopulatePrefixOptions(DhcpV6IaPrefixOption iaPrefixOption, DhcpLink dhcpLink, DhcpV6OptionConfigObject configObj)
        {
            Dictionary<int, DhcpOption> optionMap = dhcpServerConfig.EffectivePrefixOptions(this.requestMsg, dhcpLink, configObj);
            if (DhcpServerPolicies.EffectivePolicyAsBoolean(configObj, dhcpLink.GetLink(), Property.SEND_REQUESTED_OPTIONS_ONLY))
            {
                optionMap = this.RequestedOptions(optionMap, this.requestMsg);
            }

            iaPrefixOption.PutAllDhcpOptions(optionMap);
        }

        public DhcpV6Message ProcessMessage()
        {
            try
            {
                if (!PreProcess())
                {
                    log.Warn("Message dropped by preProcess");
                    return null;
                }
                if (log.IsDebugEnabled)
                {
                    log.Debug(("Processing: " + this.requestMsg.ToStringWithOptions()));
                }
                else if (log.IsInfoEnabled)
                {
                    log.Info(("Processing: " + this.requestMsg.ToString()));
                }
                //  build a reply message using the local and remote sockets from the request
                this.replyMsg = new DhcpV6Message(this.requestMsg.GetLocalAddress(), this.requestMsg.GetRemoteAddress());
                //  copy the transaction ID into the reply
                replyMsg.SetTransactionId(this.requestMsg.GetTransactionId());
                //  MUST put Server Identifier DUID in ADVERTISE or REPLY message
                replyMsg.PutDhcpOption(dhcpServerIdOption);
                //  copy Client Identifier DUID if given in client request message
                replyMsg.PutDhcpOption(this.requestMsg.GetDhcpClientIdOption());
                if (!Process())
                {
                    log.Warn("Message dropped by processor");
                    return null;
                }
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Returning: {0}", this.replyMsg.ToStringWithOptions());
                }
                else if (log.IsInfoEnabled)
                {
                    log.InfoFormat("Returning: {0}", this.replyMsg.ToString());
                }
            }
            finally
            {
                if (!this.PostProcess())
                {
                    log.Warn("Message dropped by postProcess");
                    this.replyMsg = null;
                }
            }
            return this.replyMsg;
        }

        public virtual bool PreProcess()
        {
            IPAddress localSocketAddr = this.requestMsg.GetLocalAddress();
            this.clientLink = dhcpServerConfig.FindDhcpLinkV6(localSocketAddr, this.clientLinkAddress);
            if (this.clientLink == null)
            {
                log.ErrorFormat("No Link configured for DHCPv6 client request: localAddress={0} clientLinkAddress={1}"
                    , localSocketAddr.ToString(), this.clientLinkAddress.ToString());
                return false;
                //  must configure link for server to reply
            }
            return true;
            //  ok to process
        }

        public abstract bool Process();

        public bool PostProcess()
        {
            // TODO consider the implications of always removing the
            //      recently processed message b/c we could just keep
            //      getting blasted by an attempted DOS attack?
            //  Exactly!?... the comment above says it all
            //             if (recentMsgs.remove(requestMsg)) {
            //                 if (log.IsDebugEnabled)
            //                     log.Debug("Removed recent message: " + requestMsg.toString());
            //             }
            return true;
        }

        protected bool ShouldMulticast()
        {
            if (this.requestMsg.IsUnicast())
            {
                Dictionary<int, DhcpOption> effectiveMsgOptions = dhcpServerConfig.EffectiveMsgOptions(this.requestMsg, this.clientLink);
                if (((effectiveMsgOptions == null)
                            || !effectiveMsgOptions.ContainsKey(DhcpConstants.V6OPTION_UNICAST)))
                {
                    //  if the server has not explicitly told the client to unicast,
                    //  then tell the client that it should send multicast packets
                    return true;
                }

            }

            return false;
        }

        protected void SetReplyStatus(int statusCode)
        {
            DhcpV6StatusCodeOption statusOption = new DhcpV6StatusCodeOption();
            statusOption.SetStatusCode(statusCode);
            this.replyMsg.PutDhcpOption(statusOption);
        }

        protected void AddIaNaOptionStatusToReply(DhcpV6IaNaOption iaNaOption, int statusCode)
        {
            DhcpV6IaNaOption replyIaNaOption = new DhcpV6IaNaOption();
            replyIaNaOption.SetIaId(iaNaOption.GetIaId());
            DhcpV6StatusCodeOption status = new DhcpV6StatusCodeOption();
            status.SetStatusCode(statusCode);
            replyIaNaOption.PutDhcpOption(status);
            this.replyMsg.AddIaNaOption(replyIaNaOption);
        }

        protected void AddIaTaOptionStatusToReply(DhcpV6IaTaOption iaTaOption, int statusCode)
        {
            DhcpV6IaTaOption replyIaTaOption = new DhcpV6IaTaOption();
            replyIaTaOption.SetIaId(iaTaOption.GetIaId());
            DhcpV6StatusCodeOption status = new DhcpV6StatusCodeOption();
            status.SetStatusCode(statusCode);
            replyIaTaOption.PutDhcpOption(status);
            this.replyMsg.AddIaTaOption(replyIaTaOption);
        }

        protected void AddIaPdOptionStatusToReply(DhcpV6IaPdOption iaPdOption, int statusCode)
        {
            DhcpV6IaPdOption replyIaPdOption = new DhcpV6IaPdOption();
            replyIaPdOption.SetIaId(iaPdOption.GetIaId());
            DhcpV6StatusCodeOption status = new DhcpV6StatusCodeOption();
            status.SetStatusCode(statusCode);
            replyIaPdOption.PutDhcpOption(status);
            replyMsg.AddIaPdOption(replyIaPdOption);
        }

        protected void AddBindingToReply(DhcpLink clientLink, Binding binding)
        {
            if ((binding.GetIatype() == Binding.NA_TYPE))
            {
                this.AddNaBindingToReply(this.clientLink, binding);
            }
            else if ((binding.GetIatype() == Binding.TA_TYPE))
            {
                this.AddTaBindingToReply(this.clientLink, binding);
            }
            else if ((binding.GetIatype() == Binding.PD_TYPE))
            {
                this.AddPdBindingToReply(this.clientLink, binding);
            }

        }

        protected void AddNaBindingToReply(DhcpLink clientLink, Binding binding)
        {
            DhcpV6IaNaOption dhcpIaNaOption = new DhcpV6IaNaOption();
            dhcpIaNaOption.SetIaId(binding.GetIaid());
            long minPreferredLifetime = 0;
            HashSet<BindingObject> bindingObjs = binding.GetBindingObjects();
            if (bindingObjs != null && bindingObjs.Count > 0)
            {
                minPreferredLifetime = 4294967295;
                List<DhcpV6IaAddrOption> dhcpIaAddrOptions = new List<DhcpV6IaAddrOption>();
                foreach (BindingObject bindingObj in bindingObjs)
                {
                    DhcpV6IaAddrOption dhcpIaAddrOption = new DhcpV6IaAddrOption();
                    IPAddress inetAddr = bindingObj.GetIpAddress();
                    if ((inetAddr != null))
                    {
                        dhcpIaAddrOption.SetIpAddress(inetAddr);
                        //  must be an DhcpOptionConfigObject for IA_NA binding
                        DhcpV6OptionConfigObject configObj = ((DhcpV6OptionConfigObject)(bindingObj.GetConfigObj()));
                        if ((configObj != null))
                        {
                            long preferred = configObj.GetPreferredLifetime();
                            if (minPreferredLifetime == 4294967295 || preferred < minPreferredLifetime)
                            {
                                minPreferredLifetime = preferred;
                            }

                            dhcpIaAddrOption.SetPreferredLifetime(preferred);
                            dhcpIaAddrOption.SetValidLifetime(configObj.GetValidLifetime());
                            this.PopulateNaAddrOptions(dhcpIaAddrOption, this.clientLink, configObj);
                            dhcpIaAddrOptions.Add(dhcpIaAddrOption);
                            // TODO when do actually start the timer?  currently, two get
                            //      created - one during advertise, one during reply
                            //      policy to allow real-time expiration?
                            //                     bp.startExpireTimerTask(bindingAddr, iaAddrOption.getValidLifetime());
                        }
                        else
                        {
                            log.Error(("Null binding pool in binding: " + binding.ToString()));
                        }
                    }
                    else
                    {
                        log.Error(("Null address in binding: " + binding.ToString()));
                    }
                }

                dhcpIaNaOption.SetIaAddrOptions(dhcpIaAddrOptions);
            }
            else
            {
                log.Error("No IA_NA bindings in binding object!");
            }

            this.SetIaNaT1(this.clientLink, dhcpIaNaOption, minPreferredLifetime);
            this.SetIaNaT2(this.clientLink, dhcpIaNaOption, minPreferredLifetime);
            this.PopulateIaNaOptions(dhcpIaNaOption, this.clientLink);
            this.replyMsg.AddIaNaOption(dhcpIaNaOption);
        }

        protected void AddTaBindingToReply(DhcpLink clientLink, Binding binding)
        {
            DhcpV6IaTaOption dhcpIaTaOption = new DhcpV6IaTaOption();
            dhcpIaTaOption.SetIaId(binding.GetIaid());
            HashSet<BindingObject> bindingObjs = binding.GetBindingObjects();
            if (((bindingObjs != null)
                        && bindingObjs.Count > 0))
            {
                List<DhcpV6IaAddrOption> dhcpIaAddrOptions = new List<DhcpV6IaAddrOption>();
                foreach (BindingObject bindingObj in bindingObjs)
                {
                    DhcpV6IaAddrOption dhcpIaAddrOption = new DhcpV6IaAddrOption();
                    IPAddress inetAddr = bindingObj.GetIpAddress();
                    if ((inetAddr != null))
                    {
                        dhcpIaAddrOption.SetIpAddress(inetAddr);
                        //  must be an DhcpOptionConfigObject for IA_TA binding
                        DhcpV6OptionConfigObject configObj = ((DhcpV6OptionConfigObject)(bindingObj.GetConfigObj()));
                        if ((configObj != null))
                        {
                            dhcpIaAddrOption.SetPreferredLifetime(configObj.GetPreferredLifetime());
                            dhcpIaAddrOption.SetValidLifetime(configObj.GetValidLifetime());
                            this.PopulateTaAddrOptions(dhcpIaAddrOption, this.clientLink, configObj);
                            dhcpIaAddrOptions.Add(dhcpIaAddrOption);
                            // TODO when do actually start the timer?  currently, two get
                            //      created - one during advertise, one during reply
                            //      policy to allow real-time expiration?
                            //                     bp.startExpireTimerTask(bindingAddr, iaAddrOption.getValidLifetime());
                        }
                        else
                        {
                            log.Error(("Null binding pool in binding: " + binding.ToString()));
                        }

                    }
                    else
                    {
                        log.Error(("Null address in binding: " + binding.ToString()));
                    }

                }

                dhcpIaTaOption.SetIaAddrOptions(dhcpIaAddrOptions);
            }
            else
            {
                log.Error("No IA_TA bindings in binding object!");
            }

            this.PopulateIaTaOptions(dhcpIaTaOption, this.clientLink);
            this.replyMsg.AddIaTaOption(dhcpIaTaOption);
        }

        protected void AddPdBindingToReply(DhcpLink clientLink, Binding binding)
        {
            DhcpV6IaPdOption dhcpIaPdOption = new DhcpV6IaPdOption();
            dhcpIaPdOption.SetIaId(binding.GetIaid());
            long minPreferredLifetime = 0;
            HashSet<BindingObject> bindingObjs = binding.GetBindingObjects();
            if (((bindingObjs != null)
                        && bindingObjs.Count > 0))
            {
                minPreferredLifetime = 4294967295;
                List<DhcpV6IaPrefixOption> dhcpIaPrefixOptions = new List<DhcpV6IaPrefixOption>();
                foreach (BindingObject bindingObj in bindingObjs)
                {
                    //  must be a Binding Prefix for IaPd binding
                    V6BindingPrefix bindingPrefix = ((V6BindingPrefix)(bindingObj));
                    DhcpV6IaPrefixOption dhcpIaPrefixOption = new DhcpV6IaPrefixOption();
                    IPAddress inetAddr = bindingObj.GetIpAddress();
                    if ((inetAddr != null))
                    {
                        dhcpIaPrefixOption.SetIpAddress(inetAddr);
                        dhcpIaPrefixOption.SetPrefixLength(bindingPrefix.GetPrefixLength());
                        //  must be an DhcpOptionConfigObject for IA_PD binding
                        DhcpV6OptionConfigObject configObj = ((DhcpV6OptionConfigObject)(bindingObj.GetConfigObj()));
                        if ((configObj != null))
                        {
                            long preferred = configObj.GetPreferredLifetime();
                            if (((minPreferredLifetime == 4294967295)
                                        || (preferred < minPreferredLifetime)))
                            {
                                minPreferredLifetime = preferred;
                            }

                            dhcpIaPrefixOption.SetPreferredLifetime(preferred);
                            dhcpIaPrefixOption.SetValidLifetime(configObj.GetValidLifetime());
                            this.PopulatePrefixOptions(dhcpIaPrefixOption, this.clientLink, configObj);
                            dhcpIaPrefixOptions.Add(dhcpIaPrefixOption);
                            // TODO when do actually start the timer?  currently, two get
                            //      created - one during advertise, one during reply
                            //      policy to allow real-time expiration?
                            //                     bp.startExpireTimerTask(bindingAddr, iaAddrOption.getValidLifetime());
                        }
                        else
                        {
                            log.Error(("Null binding pool in binding: " + binding.ToString()));
                        }

                    }
                    else
                    {
                        log.Error(("Null address in binding: " + binding.ToString()));
                    }

                }

                dhcpIaPdOption.SetIaPrefixOptions(dhcpIaPrefixOptions);
            }
            else
            {
                log.Error("No IA_PD bindings in binding object!");
            }

            SetIaPdT1(this.clientLink, dhcpIaPdOption, minPreferredLifetime);
            SetIaPdT2(this.clientLink, dhcpIaPdOption, minPreferredLifetime);
            PopulateIaPdOptions(dhcpIaPdOption, this.clientLink);
            replyMsg.AddIaPdOption(dhcpIaPdOption);
        }

        private void SetIaNaT1(DhcpLink clientLink, DhcpV6IaNaOption iaNaOption, long minPreferredLifetime)
        {
            float t1 = DhcpServerPolicies.EffectivePolicyAsFloat(this.clientLink.GetLink(), Property.IA_NA_T1);
            if ((t1 > 1))
            {
                log.Debug(("Setting IA_NA T1 to configured number of seconds: " + t1));
                //  if T1 is greater than one, then treat it as an
                //  absolute value which specifies the number of seconds
                iaNaOption.SetT1(((long)(t1)));
            }
            else
            {
                //  if T1 is less than one and greater than or equal to zero,
                //  then treat is as a percentage of the minimum preferred lifetime
                //  unless the minimum preferred lifetime is infinity (0xffffffff)
                if ((minPreferredLifetime == 4294967295))
                {
                    log.Debug(("Setting IA_NA T1 to minPreferredLifetime of infinity: " + minPreferredLifetime));
                    iaNaOption.SetT1(minPreferredLifetime);
                }
                else if ((t1 >= 0))
                {
                    //  zero means let the client decide
                    log.Debug(("Setting IA_NA T1 to configured ratio="
                                    + (t1 + (" of minPreferredLifetime=" + minPreferredLifetime))));
                    iaNaOption.SetT1(((long)((t1 * minPreferredLifetime))));
                }
                else
                {
                    log.Debug(("Setting IA_NA T1 to standard ratio=0.5" + (" of minPreferredLifetime=" + minPreferredLifetime)));
                    iaNaOption.SetT1(((long)((0.5 * minPreferredLifetime))));
                }

            }

        }

        private void SetIaNaT2(DhcpLink clientLink, DhcpV6IaNaOption iaNaOption, long minPreferredLifetime)
        {
            float t2 = DhcpServerPolicies.EffectivePolicyAsFloat(this.clientLink.GetLink(), Property.IA_NA_T2);
            if ((t2 > 1))
            {
                log.Debug(("Setting IA_NA T2 to configured number of seconds: " + t2));
                iaNaOption.SetT2(((long)(t2)));
            }
            else
            {
                //  if T2 is less than one and greater than or equal to zero,
                //  then treat is as a percentage of the minimum preferred lifetime
                //  unless the minimum preferred lifetime is infinity (0xffffffff)
                if ((minPreferredLifetime == 4294967295))
                {
                    log.Debug(("Setting IA_NA T2 to minPreferredLifetime of infinity: " + minPreferredLifetime));
                    iaNaOption.SetT2(minPreferredLifetime);
                }
                else if ((t2 >= 0))
                {
                    //  zero means let the client decide
                    log.Debug(("Setting IA_NA T2 to configured ratio="
                                    + (t2 + (" of minPreferredLifetime=" + minPreferredLifetime))));
                    iaNaOption.SetT2(((long)((t2 * minPreferredLifetime))));
                }
                else
                {
                    log.Debug(("Setting IA_NA T2 to standard ratio=0.8" + (" of minPreferredLifetime=" + minPreferredLifetime)));
                    iaNaOption.SetT2(((long)((0.8 * minPreferredLifetime))));
                }

            }

            //  ensure that T2 >= T1
            if ((iaNaOption.GetT2() < iaNaOption.GetT1()))
            {
                log.Warn(("IA_NA T2("
                                + (iaNaOption.GetT2() + (")" + (" < IA_NA T1("
                                + (iaNaOption.GetT1() + "): setting T2=T1"))))));
                iaNaOption.SetT2(iaNaOption.GetT1());
            }

        }

        private void SetIaPdT1(DhcpLink clientLink, DhcpV6IaPdOption iaPdOption, long minPreferredLifetime)
        {
            float t1 = DhcpServerPolicies.EffectivePolicyAsFloat(this.clientLink.GetLink(), Property.IA_PD_T1);
            if ((t1 > 1))
            {
                log.Debug(("Setting IA_PD T1 to configured number of seconds: " + t1));
                //  if T1 is greater than one, then treat it as an
                //  absolute value which specifies the number of seconds
                iaPdOption.SetT1(((long)(t1)));
            }
            else
            {
                //  if T1 is less than one and greater than or equal to zero,
                //  then treat is as a percentage of the minimum preferred lifetime
                //  unless the minimum preferred lifetime is infinity (0xffffffff)
                if ((minPreferredLifetime == 4294967295))
                {
                    log.Debug(("Setting IA_PD T1 to minPreferredLifetime of infinity: " + minPreferredLifetime));
                    iaPdOption.SetT1(minPreferredLifetime);
                }
                else if ((t1 >= 0))
                {
                    //  zero means let the client decide
                    log.Debug(("Setting IA_PD T1 to configured ratio="
                                    + (t1 + (" of minPreferredLifetime=" + minPreferredLifetime))));
                    iaPdOption.SetT1(((long)((t1 * minPreferredLifetime))));
                }
                else
                {
                    log.Debug(("Setting IA_PD T1 to standard ratio=0.5" + (" of minPreferredLifetime=" + minPreferredLifetime)));
                    iaPdOption.SetT1(((long)((0.5 * minPreferredLifetime))));
                }

            }

        }

        private void SetIaPdT2(DhcpLink clientLink, DhcpV6IaPdOption iaPdOption, long minPreferredLifetime)
        {
            float t2 = DhcpServerPolicies.EffectivePolicyAsFloat(this.clientLink.GetLink(), Property.IA_PD_T2);
            if ((t2 > 1))
            {
                log.Debug(("Setting IA_PD T2 to configured number of seconds: " + t2));
                iaPdOption.SetT2(((long)(t2)));
            }
            else
            {
                //  if T2 is less than one and greater than or equal to zero,
                //  then treat is as a percentage of the minimum preferred lifetime
                //  unless the minimum preferred lifetime is infinity (0xffffffff)
                if ((minPreferredLifetime == 4294967295))
                {
                    log.Debug(("Setting IA_PD T2 to minPreferredLifetime of infinity: " + minPreferredLifetime));
                    iaPdOption.SetT2(minPreferredLifetime);
                }
                else if ((t2 >= 0))
                {
                    //  zero means let the client decide
                    log.Debug(("Setting IA_PD T2 to configured ratio="
                                    + (t2 + (" of minPreferredLifetime=" + minPreferredLifetime))));
                    iaPdOption.SetT2(((long)((t2 * minPreferredLifetime))));
                }
                else
                {
                    log.Debug(("Setting IA_PD T2 to standard ratio=0.8" + (" of minPreferredLifetime=" + minPreferredLifetime)));
                    iaPdOption.SetT2(((long)((0.8 * minPreferredLifetime))));
                }

            }

            //  ensure that T2 >= T1
            if ((iaPdOption.GetT2() < iaPdOption.GetT1()))
            {
                log.Warn((" IA_PD T2("
                                + (iaPdOption.GetT2() + (")" + (" <  IA_PD T1("
                                + (iaPdOption.GetT1() + "): setting T2=T1"))))));
                iaPdOption.SetT2(iaPdOption.GetT1());
            }

        }

        protected void ProcessDdnsUpdates(bool sendUpdates)
        {
            DhcpV6ClientFqdnOption clientFqdnOption = ((DhcpV6ClientFqdnOption)(this.requestMsg.GetDhcpOption(DhcpConstants.V6OPTION_CLIENT_FQDN)));
            if ((clientFqdnOption == null))
            {
                // TODO allow name generation?
                log.Debug("No Client FQDN option in request.  Skipping DDNS update processing.");
                return;
            }

            bool includeFqdnOptionInReply = false;
            if (((this.requestMsg.GetRequestedOptionCodes() != null)
                        && this.requestMsg.GetRequestedOptionCodes().Contains(DhcpConstants.V6OPTION_CLIENT_FQDN)))
            {
                //  RFC 4704 section 6 says:
                //    Servers MUST only include a Client FQDN option in ADVERTISE and REPLY
                //    messages if the client included a Client FQDN option and the Client
                //    FQDN option is requested by the Option Request option in the client's
                //    message to which the server is responding.
                includeFqdnOptionInReply = true;
            }

            DhcpV6ClientFqdnOption replyFqdnOption = new DhcpV6ClientFqdnOption();
            replyFqdnOption.SetDomainName(clientFqdnOption.GetDomainName());
            replyFqdnOption.SetUpdateAaaaBit(false);
            replyFqdnOption.SetOverrideBit(false);
            replyFqdnOption.SetNoUpdateBit(false);
            string fqdn = clientFqdnOption.GetDomainName();
            if (((fqdn == null)
                        || (fqdn.Length <= 0)))
            {
                log.Error("Client FQDN option domain name is null/empty.  No DDNS udpates performed.");
                if (includeFqdnOptionInReply)
                {
                    replyFqdnOption.SetNoUpdateBit(true);
                    //  tell client that server did no updates
                    this.replyMsg.PutDhcpOption(replyFqdnOption);
                }

                return;
            }

            String policy = DhcpServerPolicies.EffectivePolicy(this.requestMsg, this.clientLink.GetLink(), Property.DDNS_UPDATE);
            log.Info(("Server configuration for ddns.update policy: " + policy));
            if (((policy == null)
                        || policy.ToLower() == "none"))
            {
                log.Info(("Server configuration for ddns.update policy is null or \'none\'." + "  No DDNS updates performed."));
                if (includeFqdnOptionInReply)
                {
                    replyFqdnOption.SetNoUpdateBit(true);
                    //  tell client that server did no updates
                    this.replyMsg.PutDhcpOption(replyFqdnOption);
                }

                return;
            }

            if ((clientFqdnOption.GetNoUpdateBit() && policy.ToLower() == "honorNoUpdate".ToLower()))
            {
                log.Info(("Client FQDN NoUpdate flag set.  Server configured to honor request." + "  No DDNS updates performed."));
                if (includeFqdnOptionInReply)
                {
                    replyFqdnOption.SetNoUpdateBit(true);
                    //  tell client that server did no updates
                    this.replyMsg.PutDhcpOption(replyFqdnOption);
                }

                // TODO: RFC 4704 Section 6.1
                //         ...the server SHOULD delete any RRs that it previously added 
                //         via DNS updates for the client.
                return;
            }

            bool doForwardUpdate = true;
            if ((!clientFqdnOption.GetUpdateAaaaBit()
                        && policy.ToLower() == "honorNoAAAA".ToLower()))
            {
                log.Info(("Client FQDN NoAAAA flag set.  Server configured to honor request." + "  No FORWARD DDNS updates performed."));
                doForwardUpdate = false;
            }
            else
            {
                replyFqdnOption.SetUpdateAaaaBit(true);
                //  server will do update
                if (!clientFqdnOption.GetUpdateAaaaBit())
                {
                    replyFqdnOption.SetOverrideBit(true);
                }

                //  tell client that we overrode request flag
            }

            string domain = DhcpServerPolicies.EffectivePolicy(this.clientLink.GetLink(), Property.DDNS_DOMAIN);
            if (((domain != null)
                        && domain.Count() > 0))
            {
                log.Info(("Server configuration for domain policy: " + domain));
                //  if there is a configured domain, then replace the domain provide by the client
                int dot = fqdn.IndexOf('.');
                if ((dot > 0))
                {
                    fqdn = (fqdn.Substring(0, (dot + 1)) + domain);
                }
                else
                {
                    fqdn = (fqdn + ("." + domain));
                }

                replyFqdnOption.SetDomainName(fqdn);
            }

            if (includeFqdnOptionInReply)
            {
                this.replyMsg.PutDhcpOption(replyFqdnOption);
            }

            if (sendUpdates)
            {
                foreach (Binding binding in this.bindings)
                {
                    if ((binding.GetState() == Binding.COMMITTED))
                    {
                        HashSet<BindingObject> bindingObjs = binding.GetBindingObjects();
                        if ((bindingObjs != null))
                        {
                            foreach (BindingObject bindingObj in bindingObjs)
                            {
                                V6BindingAddress bindingAddr = ((V6BindingAddress)(bindingObj));
                                DhcpConfigObject configObj = bindingAddr.GetConfigObj();
                                DdnsCallback ddnsComplete = new DhcpV6DdnsComplete(bindingAddr, replyFqdnOption);
                                DdnsUpdater ddns = new DdnsUpdater(this.requestMsg, this.clientLink.GetLink(), configObj, bindingAddr.GetIpAddress(), fqdn, this.requestMsg.GetDhcpClientIdOption().GetDuid(), configObj.GetValidLifetime(), doForwardUpdate, false, ddnsComplete);
                                ddns.ProcessUpdates();
                            }

                        }

                    }

                }

            }

        }

        protected bool AllIaAddrsOnLink(DhcpV6IaNaOption dhcpIaNaOption, DhcpLink clientLink)
        {
            bool onLink = true;
            //  assume all IPs are on link
            if ((dhcpIaNaOption != null))
            {
                List<DhcpV6IaAddrOption> iaAddrOpts = dhcpIaNaOption.GetIaAddrOptions();
                if ((iaAddrOpts != null))
                {
                    foreach (DhcpV6IaAddrOption iaAddrOpt in iaAddrOpts)
                    {
                        if (this.clientLink.GetSubnet().GetSubnetAddress().IsIPv6LinkLocal)
                        {
                            //  if the Link address is link-local, then check if the
                            //  address is within one of the pools configured for this
                            //  local Link, which automatically makes this server
                            //  "authoritative" (in ISC parlance) for this local net
                            v6AddressPool p = DhcpServerConfiguration.FindNaAddrPool(this.clientLink.GetLink(), iaAddrOpt.GetInetAddress());
                            if ((p == null))
                            {
                                log.Info(("No local address pool found for requested IA_NA: " + (iaAddrOpt.ToString() + " - considered to be off link")));
                                iaAddrOpt.SetPreferredLifetime(0);
                                iaAddrOpt.SetValidLifetime(0);
                                onLink = false;
                            }

                        }
                        else
                        {
                            //  it the Link address is remote, then check 
                            //  if the address is valid for that link
                            if (!this.clientLink.GetSubnet().Contains(iaAddrOpt.GetInetAddress()))
                            {
                                log.Info("Setting zero(0) lifetimes for off link address: " + iaAddrOpt.GetIpAddress());
                                iaAddrOpt.SetPreferredLifetime(0);
                                iaAddrOpt.SetValidLifetime(0);
                                onLink = false;
                            }
                        }
                    }
                }
            }

            return onLink;
        }

        protected bool AllIaAddrsOnLink(DhcpV6IaTaOption dhcpIaTaOption, DhcpLink clientLink)
        {
            bool onLink = true;
            //  assume all IPs are on link
            if ((dhcpIaTaOption != null))
            {
                List<DhcpV6IaAddrOption> iaAddrOpts = dhcpIaTaOption.GetIaAddrOptions();
                if ((iaAddrOpts != null))
                {
                    foreach (DhcpV6IaAddrOption iaAddrOpt in iaAddrOpts)
                    {
                        if (this.clientLink.GetSubnet().GetSubnetAddress().IsIPv6LinkLocal)
                        {
                            //  if the Link address is link-local, then check if the
                            //  address is within one of the pools configured for this
                            //  local Link, which automatically makes this server
                            //  "authoritative" (in ISC parlance) for this local net
                            v6AddressPool p = DhcpServerConfiguration.FindTaAddrPool(this.clientLink.GetLink(), iaAddrOpt.GetInetAddress());
                            if ((p == null))
                            {
                                log.Info(("No local address pool found for requested IA_TA: "
                                                + (iaAddrOpt.GetInetAddress().ToString() + " - considered to be off link")));
                                iaAddrOpt.SetPreferredLifetime(0);
                                iaAddrOpt.SetValidLifetime(0);
                                onLink = false;
                            }

                        }
                        else if (!this.clientLink.GetSubnet().Contains(iaAddrOpt.GetInetAddress()))
                        {
                            log.Info(("Setting zero(0) lifetimes for off link address: " + iaAddrOpt.GetInetAddress().ToString()));
                            iaAddrOpt.SetPreferredLifetime(0);
                            iaAddrOpt.SetValidLifetime(0);
                            onLink = false;
                        }

                    }

                }

            }

            return onLink;
        }

        protected bool AllIaPrefixesOnLink(DhcpV6IaPdOption dhcpIaPdOption, DhcpLink clientLink)
        {
            bool onLink = true;
            //  assume all IPs are on link
            if ((dhcpIaPdOption != null))
            {
                List<DhcpV6IaPrefixOption> iaPrefixOpts = dhcpIaPdOption.GetIaPrefixOptions();
                if ((iaPrefixOpts != null))
                {
                    foreach (DhcpV6IaPrefixOption iaPrefixOpt in iaPrefixOpts)
                    {
                        if (this.clientLink.GetSubnet().GetSubnetAddress().IsIPv6LinkLocal)
                        {
                            //  if the Link address is link-local, then check if the
                            //  address is within one of the pools configured for this
                            //  local Link, which automatically makes this server
                            //  "authoritative" (in ISC parlance) for this local net
                            v6PrefixPool p = DhcpServerConfiguration.FindPrefixPool(this.clientLink.GetLink(), iaPrefixOpt.GetInetAddress());
                            if ((p == null))
                            {
                                log.Info(("No local prefix pool found for requested IA_PD: "
                                                + (iaPrefixOpt.GetInetAddress().ToString() + " - considered to be off link")));
                                iaPrefixOpt.SetPreferredLifetime(0);
                                iaPrefixOpt.SetValidLifetime(0);
                                onLink = false;
                            }

                        }
                        else if (!this.clientLink.GetSubnet().Contains(iaPrefixOpt.GetInetAddress()))
                        {
                            log.Info(("Setting zero(0) lifetimes for off link prefix: " + iaPrefixOpt.GetInetAddress().ToString()));
                            iaPrefixOpt.SetPreferredLifetime(0);
                            iaPrefixOpt.SetValidLifetime(0);
                            onLink = false;
                        }

                    }

                }

            }

            return onLink;
        }

        //class RecentMsgTimerTask : TimerTask
        //{

        //    private DhcpV6Message dhcpMsg;

        //    public RecentMsgTimerTask(DhcpV6Message dhcpMsg)
        //    {
        //        this.dhcpMsg = this.dhcpMsg;
        //    }

        //    [Override()]
        //    public void run()
        //    {
        //        if (recentMsgs.remove(this.dhcpMsg))
        //        {
        //            if (log.IsDebugEnabled)
        //            {
        //                log.Debug(("Pruned recent message: " + this.dhcpMsg.toString()));
        //            }

        //        }

        //    }
        //}
    }
}
