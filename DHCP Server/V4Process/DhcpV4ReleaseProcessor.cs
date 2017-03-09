using PIXIS.DHCP.Request.Bind;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.V4Process
{
    public class DhcpV4ReleaseProcessor : BaseDhcpV4Processor
    {
        /** The log. */
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly static object _lock = new object();
        /**
    * Construct an DhcpV4ReleaseProcessor processor.
    * 
    * @param requestMsg the Release message
    * @param clientLinkAddress the client link address
    */
        public DhcpV4ReleaseProcessor(DhcpV4Message requestMsg, IPAddress clientLinkAddress)
            : base(requestMsg, clientLinkAddress)
        {
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.server.request.BaseDhcpProcessor#preProcess()
         */
        public override bool PreProcess()
        {
            if (!base.PreProcess())
            {
                return false;
            }

            if (_requestMsg.GetCiAddr() == null)
            {
                log.Warn("Ignoring Release message: " +
                        "ciAddr is null");
                return false;
            }

            if (_requestMsg.GetCiAddr().Equals(DhcpConstants.ZEROADDR_V4))
            {
                log.Warn("Ignoring Release message: " +
                        "ciAddr is zero");
            }

            return true;
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.server.request.BaseDhcpProcessor#process()
         */
        public override bool Process()
        {
            byte[] chAddr = _requestMsg.GetChAddr();
            lock (_lock)
            {
                V4AddrBindingManager bindingMgr = _dhcpServerConfig.GetV4AddrBindingMgr();
                if (bindingMgr != null)
                {
                    log.Info("Processing Release" +
                             " from chAddr=" + Util.ToHexString(chAddr) +
                             " ciAddr=" + _requestMsg.GetCiAddr().ToString());
                    Binding binding = bindingMgr.FindCurrentBinding(_clientLink,
                                                                    chAddr, _requestMsg);
                    if (binding != null)
                    {
                        HashSet<BindingObject> bindingObjs = binding.GetBindingObjects();
                        if ((bindingObjs != null) && bindingObjs.Count > 0)
                        {
                            V4BindingAddress bindingAddr = (V4BindingAddress)bindingObjs.First();
                            bindingMgr.ReleaseIaAddress(binding, bindingAddr);
                        }
                        else
                        {
                            log.Error("No binding addresses in binding for client: " +
                                    Util.ToHexString(chAddr));
                        }
                    }
                    else
                    {
                        log.Error("No Binding available for client: " +
                                Util.ToHexString(chAddr));
                    }
                }
                else
                {
                    log.Error("Unable to process V4 Release:" +
                            " No V4AddrBindingManager available");
                }
            }
            return false;   // no reply for v4 release
        }
    }
}
