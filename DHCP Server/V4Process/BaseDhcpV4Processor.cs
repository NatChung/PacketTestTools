using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using PIXIS.DHCP.Message;
using System.Net.NetworkInformation;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Option.V4;
using PIXIS.DHCP.Config;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Request.Bind;
using System.Collections.ObjectModel;
using PIXIS.DHCP.Request.Dns;
using PIXIS.DHCP.Option;
using static PIXIS.DHCP.Config.DhcpServerPolicies;
using System.Threading;

namespace PIXIS.DHCP.V4Process
{
    public abstract class BaseDhcpV4Processor : DhcpV4MessageProcessor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected static DhcpServerConfiguration _dhcpServerConfig = new  DhcpServerConfiguration();
        // wrap the configured V4ServerId option in a DhcpOption for the wire
        protected static DhcpV4ServerIdOption _dhcpV4ServerIdOption = new DhcpV4ServerIdOption(_dhcpServerConfig.GetDhcpServerConfig().v4ServerIdOption);
        protected DhcpV4Message _requestMsg;
        protected DhcpV4Message _replyMsg;
        protected IPAddress _clientLinkAddress;
        protected DhcpLink _clientLink;
        protected List<Binding> _bindings = new List<Binding>();
        private readonly static object _lock = new object();

        protected BaseDhcpV4Processor(DhcpV4Message requestMsg, IPAddress clientLinkAddress)
        {
            _requestMsg = requestMsg;
            _clientLinkAddress = clientLinkAddress;
        }
        private Dictionary<int, DhcpOption> RequestedOptions(Dictionary<int, DhcpOption> optionDictionary, DhcpV4Message requestMsg)
        {
            if (optionDictionary != null && optionDictionary.Count > 0)
            {
                List<int> requestedCodes = requestMsg.GetRequestedOptionCodes();
                if (requestedCodes != null && requestedCodes.Count > 0)
                {
                    Dictionary<int, DhcpOption> newOptionDictionary = new Dictionary<int, DhcpOption>();
                    foreach (var req in requestedCodes)
                    {
                        if (optionDictionary.ContainsKey(req))
                        {
                            newOptionDictionary[req] = optionDictionary[req];
                        }
                    }
                    optionDictionary = newOptionDictionary;
                }
            }
            return optionDictionary;
        }

        //Populate v4 options.
        //@param link the link
        //@param configObj the config object or null if none
        protected void PopulateV4Reply(DhcpLink dhcpLink, DhcpV4OptionConfigObject configObj)
        {
            string sname = DhcpServerPolicies.EffectivePolicy(_requestMsg, configObj,
                dhcpLink.GetLink(), Property.V4_HEADER_SNAME);
            if (!String.IsNullOrEmpty(sname))
            {
                _replyMsg.SetsName(sname);
            }

            string filename = DhcpServerPolicies.EffectivePolicy(_requestMsg, configObj,
                    dhcpLink.GetLink(), Property.V4_HEADER_FILENAME);

            if (!String.IsNullOrEmpty(filename))
            {
                _replyMsg.SetFile(filename);
            }

            Dictionary<int, DhcpOption> optionMap =
                _dhcpServerConfig.EffectiveV4AddrOptions(_requestMsg, dhcpLink, configObj);
            if (DhcpServerPolicies.EffectivePolicyAsBoolean(configObj,
                    dhcpLink.GetLink(), Property.SEND_REQUESTED_OPTIONS_ONLY))
            {
                optionMap = RequestedOptions(optionMap, _requestMsg);
            }
            _replyMsg.PutAllDhcpOptions(optionMap);

            // copy the relay agent info option from request to reply 
            // in order to echo option back to router as required
            if (_requestMsg.HasOption(DhcpConstants.V4OPTION_RELAY_INFO))
            {
                _requestMsg.PutDhcpOption(_requestMsg.GetDhcpOption(DhcpConstants.V4OPTION_RELAY_INFO));
            }
        }

        //Process the client request.  Find appropriate configuration based on any
        //criteria in the request message that can be matched against the server's
        //configuration, then formulate a response message containing the options
        //to be sent to the client.
        //@return a Reply DhcpMessage
        public DhcpV4Message ProcessMessage(IPAddress localAddress)
        {
            Monitor.Enter(_lock);
            try
            {
                //設定ServerId為管理IP
                _dhcpV4ServerIdOption.SetIpAddress(localAddress.ToString());

                if (!PreProcess())
                    return null;
                // build a reply message using the local and remote sockets from the request
                _replyMsg = new DhcpV4Message(_requestMsg.GetLocalAddress(), _requestMsg.GetRemoteAddress());

                _replyMsg.SetOp(DhcpConstants.V4_OP_REPLY);
                // copy fields from request to reply
                _replyMsg.SetHtype(_requestMsg.GetHtype());
                _replyMsg.SetHlen(_requestMsg.GetHlen());
                _replyMsg.SetTransactionId(_requestMsg.GetTransactionId());
                _replyMsg.SetFlags(_requestMsg.GetFlags());
                _replyMsg.SetGiAddr(_requestMsg.GetGiAddr());
                _replyMsg.SetChAddr(_requestMsg.GetChAddr());
                // MUST put Server Identifier in REPLY message
                _replyMsg.PutDhcpOption(_dhcpV4ServerIdOption);

                if (!Process())
                {
                    log.Warn("Message dropped by processor");
                    return null;
                }
            }
            catch (Exception ex)
            {
                log.WarnFormat("BaseDhcpV4Processor ProcessMessage Faile. exMessage:{0} exStackTrace:{1}", ex.Message, ex.StackTrace);
                return null;
            }
            finally
            {
                if (!PostProcess())
                {
                    log.Warn("Message dropped by postProcess");
                    _replyMsg = null;
                }
                Monitor.Exit(_lock);
            }
            return _replyMsg;
        }

        public virtual bool PreProcess()
        {
            IPAddress localSocketAddr = _requestMsg.GetLocalAddress();

            byte[] chAddr = _requestMsg.GetChAddr();

            //判斷是否有MAC
            if ((chAddr == null) || (chAddr.Length == 0) || IsIgnoredMac(chAddr))
            {
                //log.Warn("Ignorning request message from client: mac=" + Util.toHexString(chAddr));
                return false;
            }

            _clientLink = _dhcpServerConfig.FindDhcpLinkV4(localSocketAddr, (IPAddress)_clientLinkAddress);
            if (_clientLink == null)
            {
                //log.Error("No Link configured for DHCPv4 client request: " + " localAddress=" + localSocketAddr.GetAddress().GetHostAddress() +" clientLinkAddress=" + clientLinkAddress.getHostAddress());
                return false;   // must configure link for server to reply
            }

            /* TODO: check if this DOS mitigation is useful
             * 
                    boolean isNew = recentMsgs.add(requestMsg);
                    if (!isNew) {
                        if (log.isDebugEnabled())
                            log.debug("Dropping recent message");
                        return false;	// don't process
                    }

                    if (log.isDebugEnabled())
                        log.debug("Processing new message");

                    long timer = DhcpServerPolicies.effectivePolicyAsLong(clientLink.getLink(),
                            Property.DHCP_PROCESSOR_RECENT_MESSAGE_TIMER);
                    if (timer > 0) {
                        recentMsgPruner.schedule(new RecentMsgTimerTask(requestMsg), timer);
                    }
            */
            return true;	// ok to process
        }

        /**
        * Process.
        * 
        * @return true if a reply should be sent
        */
        public abstract bool Process();

        /**
         * Post process.
         * 
         * @return true if a reply should be sent
         */
        public bool PostProcess()
        {
            //TODO consider the implications of always removing the
            //     recently processed message b/c we could just keep
            //     getting blasted by an attempted DOS attack?
            // Exactly!?... the comment above says it all
            //    		if (recentMsgs.remove(requestMsg)) {
            //    			if (log.isDebugEnabled())
            //    				log.debug("Removed recent message: " + requestMsg.toString());
            //    		}
            return true;
        }
        private bool IsIgnoredMac(byte[] chAddr)
        {
            string ignoredMacPolicy = DhcpServerPolicies.GlobalPolicy(Property.V4_IGNORED_MACS);
            if (ignoredMacPolicy != null)
            {
                string[] ignoredMacs = ignoredMacPolicy.Split(',');
                if (ignoredMacs != null)
                {
                    foreach (var ignoredMac in ignoredMacs)
                    {
                        if (ignoredMac.Trim().ToUpper().Equals(Util.ToHexString(chAddr).ToUpper()))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /**
	     * Adds the v4 binding to reply.
	     * 
	     * @param clientLink the client link
         * @param binding the binding
         */
        protected void AddBindingToReply(DhcpLink clientLink, Binding binding)
        {
            HashSet<BindingObject> bindingObjs = binding.GetBindingObjects();
            if ((bindingObjs != null) && bindingObjs.Count > 0)
            {
                if (bindingObjs.Count == 1)
                {
                    BindingObject bindingObj = bindingObjs.First();
                    IPAddress inetAddr = bindingObj.GetIpAddress();
                    if (inetAddr != null)
                    {
                        _replyMsg.SetYiAddr(inetAddr);
                        // must be an DhcpV4OptionConfigObject for v4 binding
                        DhcpV4OptionConfigObject configObj =
                            (DhcpV4OptionConfigObject)bindingObj.GetConfigObj();
                        if (configObj != null)
                        {
                            long preferred = configObj.GetPreferredLifetime();
                            DhcpV4LeaseTimeOption dhcpV4LeaseTimeOption = new DhcpV4LeaseTimeOption();
                            dhcpV4LeaseTimeOption.SetUnsignedInt(preferred);
                            _replyMsg.PutDhcpOption(dhcpV4LeaseTimeOption);
                            PopulateV4Reply(clientLink, configObj);
                            //TODO when do actually start the timer?  currently, two get
                            //     created - one during advertise, one during reply
                            //     policy to allow real-time expiration?
                            //					bp.startExpireTimerTask(bindingAddr, iaAddrOption.getValidLifetime());
                        }
                        else
                        {
                            log.Error("Null binding pool in binding: " + binding.ToString());
                        }
                    }
                    else
                    {
                        log.Error("Null address in binding: " + binding.ToString());
                    }
                }
                else
                {
                    log.Error("Expected only one bindingObject in v4 Binding, but found " +
                            bindingObjs.Count + "bindingObjects");
                }
            }
            else
            {
                log.Error("No V4 bindings in binding object!");
            }
        }

        protected void ProcessDdnsUpdates(bool sendUpdates)
        {
            bool doForwardUpdate = true;

            DhcpV4ClientFqdnOption clientFqdnOption = (DhcpV4ClientFqdnOption)_requestMsg.GetDhcpOption(DhcpConstants.V4OPTION_CLIENT_FQDN);
            DhcpV4HostnameOption hostnameOption = (DhcpV4HostnameOption)_requestMsg.GetDhcpOption(DhcpConstants.V4OPTION_HOSTNAME);

            if (clientFqdnOption == null && hostnameOption == null)
            {
                //TODO allow name generation?
                log.Debug("No Client FQDN nor hostname option in request.  Skipping DDNS update processing.");
                return;
            }

            string fqdn = "";
            string domain = DhcpServerPolicies.DffectivePolicy(_clientLink.GetLink(), Property.DDNS_DOMAIN);
            DhcpV4ClientFqdnOption replyFqdnOption = null;

            if (clientFqdnOption != null)
            {
                replyFqdnOption = new DhcpV4ClientFqdnOption();
                replyFqdnOption.SetDomainName(clientFqdnOption.GetDomainName());
                replyFqdnOption.SetUpdateABit(false);
                replyFqdnOption.SetOverrideBit(false);
                replyFqdnOption.SetNoUpdateBit(false);
                replyFqdnOption.SetEncodingBit(clientFqdnOption.GetEncodingBit());
                replyFqdnOption.SetRcode1((short)0xff);     // RFC 4702 says server should set to 255
                replyFqdnOption.SetRcode2((short)0xff);     // RFC 4702 says server should set to 255

                fqdn = clientFqdnOption.GetDomainName();
                if ((fqdn == null) || (fqdn.Length <= 0))
                {
                    log.Error("Client FQDN option domain name is null/empty.  No DDNS udpates performed.");
                    replyFqdnOption.SetNoUpdateBit(true);   // tell client that server did no updates
                    _replyMsg.PutDhcpOption(replyFqdnOption);
                    return;
                }

                string policy = DhcpServerPolicies.EffectivePolicy(_requestMsg, _clientLink.GetLink(), Property.DDNS_UPDATE);
                log.Info("Server configuration for ddns.update policy: " + policy);
                if ((policy == null) || policy.Contains("none"))
                {
                    log.Info("Server configuration for ddns.update policy is null or 'none'." + "  No DDNS updates performed.");
                    replyFqdnOption.SetNoUpdateBit(true);   // tell client that server did no updates
                    _replyMsg.PutDhcpOption(replyFqdnOption);
                    return;
                }

                if (clientFqdnOption.GetNoUpdateBit() && policy.Contains("honorNoUpdate"))
                {
                    log.Info("Client FQDN NoUpdate flag set.  Server configured to honor request." + "  No DDNS updates performed.");
                    replyFqdnOption.SetNoUpdateBit(true);   // tell client that server did no updates
                    _replyMsg.PutDhcpOption(replyFqdnOption);
                    //TODO: RFC 4704 Section 6.1
                    //		...the server SHOULD delete any RRs that it previously added 
                    //		via DNS updates for the client.
                    return;
                }

                if (!clientFqdnOption.GetUpdateABit() && policy.Contains("honorNoA"))
                {
                    log.Info("Client FQDN NoA flag set.  Server configured to honor request." + "  No FORWARD DDNS updates performed.");
                    doForwardUpdate = false;
                }
                else
                {
                    replyFqdnOption.SetUpdateABit(true);    // server will do update
                    if (!clientFqdnOption.GetUpdateABit())
                    {
                        replyFqdnOption.SetOverrideBit(true);   // tell client that we overrode request flag
                    }
                }

                if (!String.IsNullOrEmpty(domain))
                {
                    log.Info("Server configuration for domain policy: " + domain);
                    // if there is a configured domain, then replace the domain provide by the client
                    int dot = fqdn.IndexOf('.');
                    if (dot > 0)
                    {
                        fqdn = fqdn.Substring(0, dot + 1) + domain;
                    }
                    else
                    {
                        fqdn = fqdn + "." + domain;
                    }
                    replyFqdnOption.SetDomainName(fqdn);
                }
                // since the client DID send option 81, return it in the reply
                _replyMsg.PutDhcpOption(replyFqdnOption);
            }
            else
            {
                // The client did not send an FQDN option, so we'll try to formulate the FQDN
                // from the hostname option combined with the DDNS_DOMAIN policy setting.
                // A replyFqdnOption is fabricated to be stored with the binding for use
                // with the release/expire binding processing to remove the DDNS entry.
                replyFqdnOption = new DhcpV4ClientFqdnOption();
                fqdn = hostnameOption.GetString();
                if (!String.IsNullOrEmpty(domain))
                {
                    log.Info("Server configuration for domain policy: " + domain);
                    fqdn = fqdn + "." + domain;
                    // since the client did NOT send option 81, do not put
                    // the fabricated fqdnOption into the reply packet
                    // but set the option so that is can be used below
                    // when storing the fqdnOption to the database, so 
                    // that it can be used if/when the lease expires
                    replyFqdnOption.SetDomainName(fqdn);
                    // server will do the A record update, so set the flag
                    // for the option stored in the database, so server will
                    // remove the A record when the lease expires
                    replyFqdnOption.SetUpdateABit(true);
                }
                else
                {
                    log.Error("No DDNS domain configured.  No DDNS udpates performed.");
                    replyFqdnOption.SetNoUpdateBit(true);   // tell client that server did no updates
                    _replyMsg.PutDhcpOption(replyFqdnOption);
                    return;
                }
            }

            if (sendUpdates)
            {
                foreach (Binding binding in _bindings)
                {
                    if (binding.GetState() == Binding.COMMITTED)
                    {
                        HashSet<BindingObject> bindingObjs = binding.GetBindingObjects();
                        if (bindingObjs != null)
                        {
                            foreach (var bindingObj in bindingObjs)
                            {
                                V4BindingAddress bindingAddr = (V4BindingAddress)bindingObj;
                                DhcpConfigObject configObj = bindingAddr.GetConfigObj();
                                DdnsCallback ddnsComplete = new DhcpV4DdnsComplete(bindingAddr, replyFqdnOption);
                                DdnsUpdater ddns = new DdnsUpdater(_requestMsg, _clientLink.GetLink(), configObj, bindingAddr.GetIpAddress(), fqdn, _requestMsg.GetChAddr(), configObj.GetValidLifetime(), doForwardUpdate, false, ddnsComplete);
                                ddns.ProcessUpdates();
                            }
                        }
                    }
                }
            }
        }

        public bool AddrOnLink(DhcpV4RequestedIpAddressOption requestedIpOption, DhcpLink clientLink)
        {
            bool onLink = true;
            if (requestedIpOption != null)
            {
                IPAddress requestedIp = IPAddress.Parse(requestedIpOption.GetIpAddress());
                if (!clientLink.GetSubnet().Contains(requestedIp))
                {
                    onLink = false;
                }
            }
            return onLink;
        }
    }
}
