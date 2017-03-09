using PIXIS.DHCP.Config;
using PIXIS.DHCP.DB;
using PIXIS.DHCP.Request.Bind;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static PIXIS.DHCP.Config.DhcpServerPolicies;
using System.Threading;

namespace PIXIS.DHCP.V4Process
{
    public class DhcpV4DiscoverProcessor : BaseDhcpV4Processor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly static object _lock = new object();

        public DhcpV4DiscoverProcessor(DhcpV4Message requestMsg, IPAddress clientLinkAddress) : base(requestMsg, clientLinkAddress)
        {
        }

        public override bool PreProcess()
        {
            if (!base.PreProcess())
            {
                return false;
            }
            IPAddress ciAddr = _requestMsg.GetCiAddr();
            if (!ciAddr.Equals(DhcpConstants.ZEROADDR_V4))
            {
                log.Warn("Ignoring Discover message: " + "ciAddr field is non-zero: " + ciAddr);
                return false;
            }
            if (_requestMsg.GetDhcpV4ServerIdOption() != null)
            {
                log.Warn("Ignoring Discover message: " + "ServerId option is not null");
                return false;
            }
            return true;
        }
        public override bool Process()
        {
            bool sendReply = true;
            lock (_lock)
            {
                bool rapidCommit = IsRapidCommit(_requestMsg, _clientLink.GetLink());
                byte state = rapidCommit ? IaAddress.COMMITTED : IaAddress.ADVERTISED;
                byte[] chAddr = _requestMsg.GetChAddr();

                V4AddrBindingManager bindingMgr = _dhcpServerConfig.GetV4AddrBindingMgr();
                if (bindingMgr != null)
                {
                    log.Info("Processing Discover from: chAddr=" + Util.ToHexString(chAddr));
                    Binding binding = bindingMgr.FindCurrentBinding(_clientLink, chAddr, _requestMsg);
                    if (binding == null)
                    {
                        // no current binding for this MAC, create a new one 
                        binding = bindingMgr.CreateDiscoverBinding(_clientLink, chAddr, _requestMsg, state);
                    }
                    else
                    {
                        binding = bindingMgr.UpdateBinding(binding, _clientLink, chAddr, _requestMsg, state);
                    }
                    if (binding != null)
                    {
                        // have a good binding, put it in the reply with options 
                        AddBindingToReply(_clientLink, binding);
                        _bindings.Add(binding);
                    }
                    else
                    {
                        log.Error("Failed to create binding for Discover from: " + Util.ToHexString(chAddr));
                        sendReply = false;
                    }
                }
                else
                {
                    log.Error("Unable to process V4 Discover:" + " No V4AddrBindingManager available");
                    sendReply = false;
                }

                if (sendReply)
                {
                    if (rapidCommit)
                    {
                        _replyMsg.SetMessageType((short)DhcpConstants.V4MESSAGE_TYPE_ACK);
                    }
                    else
                    {
                        _replyMsg.SetMessageType((short)DhcpConstants.V4MESSAGE_TYPE_OFFER);
                    }
                    if (_bindings.Count != 0)
                    {
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
            }
            return sendReply;
        }

        private bool IsRapidCommit(DhcpV4Message requestMsg, link clientLink)
        {
            if (_requestMsg.HasOption(DhcpConstants.V4OPTION_RAPID_COMMIT) &&
                DhcpServerPolicies.EffectivePolicyAsBoolean(requestMsg, clientLink,
                Property.SUPPORT_RAPID_COMMIT))
            {
                return true;
            }
            return false;
        }
    }
}
