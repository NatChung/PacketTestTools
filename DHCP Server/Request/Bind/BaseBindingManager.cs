using PIXIS.DHCP.Config;
using PIXIS.DHCP.DB;
using PIXIS.DHCP.Message;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Request.Bind
{
    public abstract class BaseBindingManager
    {
        /** The log. */
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DhcpServerConfiguration serverConfig = new DhcpServerConfiguration();

        protected IaManager iaMgr;

        private readonly static object _lock = new object();
        /// <summary>
        /// DHCP 派發IP前，確認IP是否已經使用
        /// </summary>
        /// <param name="ip">檢查IP</param>
        /// <param name="wait">等待時間(毫秒)</param>
        /// <returns></returns>
        public Func<IPAddress, int, bool> CheckIPIsUsed { get; set; }
        public void Init()
        {

            iaMgr = new LeaseManager();

            InitPoolMap();

            InitStaticBindings();

            StartReaper();
        }
        public void UpdatePool()
        {
            InitPoolMap();
        }
        /// <summary>
        ///  Start a reaper thread to check for expired bindings.
        /// </summary>
        protected abstract void StartReaper();

        /// <summary>
        /// Initialize the static bindings.  Read through the link map from the server's
        /// configuration and build the binding map keyed by link address with a
        /// value of the list of (na/ta/v4 address or prefix) bindings for the link.
        /// </summary>
        protected void InitStaticBindings()
        {
            Dictionary<Subnet, DhcpLink> linkMap = serverConfig.GetLinkMap();
            if (linkMap != null)
            {
                staticBindingMap = new Dictionary<string, List<StaticBinding>>();
                foreach (DhcpLink dhcpLink in linkMap.Values)
                {
                    List<StaticBinding> staticBindings = BuildStaticBindings(dhcpLink.GetLink());
                    if ((staticBindings != null) && staticBindings.Count > 0)
                    {
                        staticBindingMap[dhcpLink.GetLinkAddress()] = staticBindings;
                    }
                }
            }
        }

        protected void AddStaticBindings(DhcpLink dhcpLink, v4AddressBinding binding)
        {
            if (staticBindingMap.ContainsKey(dhcpLink.GetLinkAddress()))
            {
                V4StaticAddressBinding sb = new V4StaticAddressBinding(binding);
                staticBindingMap[dhcpLink.GetLinkAddress()].Add(sb);
            }

        }

        /// <summary>
        /// Build the list of BindingPools for the given DhcpLink.  The BindingPools
        /// are either V6AddressBindingPools (NA and TA) or V6PrefixBindingPools
        /// or V4AddressBindingPools.
        /// </summary>
        /// <param name="link">configured DhcpLink</param>
        /// <returns>BindingPools for the link</returns>
        protected abstract List<BindingPool> BuildBindingPools(link link);

        /// <summary>
        /// Initialize the pool map.  Read through the link map from the server's
        /// configuration and build the pool map keyed by link address with a
        /// value of the list of (na/ta/v4 address or prefix) bindings for the link.
        /// </summary>
        private void InitPoolMap()
        {
            Dictionary<Subnet, DhcpLink> linkMap = serverConfig.GetLinkMap();
            if ((linkMap != null) && linkMap.Count > 0)
            {
                bindingPoolMap = new Dictionary<String, List<BindingPool>>();
                foreach (DhcpLink dhcpLink in linkMap.Values)
                {
                    List<BindingPool> bindingPools = BuildBindingPools(dhcpLink.GetLink());
                    if ((bindingPools != null) && bindingPools.Count > 0)
                    {
                        bindingPoolMap[dhcpLink.GetLinkAddress()] = bindingPools;
                    }
                }
            }
            else
            {
                _log.Error("LinkMap is null for DhcpServerConfiguration");
            }
        }

        /// <summary>
        /// Find a static binding, if any, for the given client identity association (IA).
        /// </summary>
        /// <param name="clientLink">link for the client request message</param>
        /// <param name="duid">DUID of the client</param>
        /// <param name="iatype">IA type of the client request</param>
        /// <param name="iaid">IAID of the client request</param>
        /// <param name="requestMsg">client request message</param>
        /// <returns>the existing StaticBinding for this client request</returns>
        public StaticBinding FindStaticBinding(link clientLink, byte[] duid, byte iatype, long iaid, DhcpMessage requestMsg)
        {
            try
            {
                List<StaticBinding> staticBindings = staticBindingMap.ContainsKey(clientLink.Address) ? staticBindingMap[clientLink.Address] : null;
                if ((staticBindings != null) && staticBindings.Count > 0)
                {
                    foreach (StaticBinding staticBinding in staticBindings)
                    {
                        if (staticBinding.Matches(duid, iatype, iaid, requestMsg))
                        {
                            _log.Info("Found static binding: " + staticBinding);
                            return staticBinding;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Exception in findStaticBinding");
            }
            return null;
        }

        /// <summary>
        /// Create a binding in from a StaticBinding
        /// </summary>
        /// <param name="clientLink">link for the client request message</param>
        /// <param name="duid">DUID of the client</param>
        /// <param name="iatype">IA type of the client request</param>
        /// <param name="iaid">IAID of the client request</param>
        /// <param name="staticBinding">static binding</param>
        /// <param name="requestMsg">client request message</param>
        /// <returns>created Binding</returns>
        protected Binding CreateStaticBinding(DhcpLink clientLink, byte[] duid, byte iatype, long iaid,
                StaticBinding staticBinding, DhcpMessage requestMsg)
        {
            Binding binding = null;
            if (staticBinding != null)
            {
                binding = BuildBinding(clientLink, duid, iatype, iaid, IaAddress.STATIC);
                IPAddress inetAddr = staticBinding.GetInetAddress();
                if (inetAddr != null)
                {
                    IaAddress iaAddr = new IaAddress();
                    iaAddr.SetIpAddress(inetAddr);
                    iaAddr.SetState(IaAddress.STATIC);
                    V6BindingAddress bindingAddr = new V6BindingAddress(iaAddr, staticBinding);
                    SetBindingObjectTimes(bindingAddr,
                            staticBinding.GetPreferredLifetimeMs(),
                            staticBinding.GetPreferredLifetimeMs());
                    // TODO store the configured options in the persisted binding?
                    // bindingAddr.setDhcpOptions(bp.getDhcpOptions());
                    HashSet<BindingObject> bindingObjs = new HashSet<BindingObject>();
                    bindingObjs.Add(bindingAddr);
                    binding.SetBindingObjects(bindingObjs);
                    try
                    {
                        iaMgr.CreateIA(binding);
                    }
                    catch (Exception ex)
                    {
                        _log.Error("Failed to create persistent binding");
                        return null;
                    }
                }
                else
                {
                    _log.Error("Failed to build binding object(s)");
                    return null;
                }
            }
            else
            {
                _log.Error("StaticBinding object is null");
            }

            String bindingType = (iatype == IdentityAssoc.V4_TYPE) ? "discover" : "solicit";
            if (binding != null)
            {
                _log.Info("Created static " + bindingType + " binding: " + binding.ToString());
            }
            else
            {
                _log.Warn("Failed to create static " + bindingType + " binding");
            }
            return binding;
        }

        /// <summary>
        ///  Builds a new Binding.Create a new IdentityAssoc from the given
        ///  tuple and wrap that IdentityAssoc in a Binding.
        /// </summary>
        /// <param name="clientLink">client link</param>
        /// <param name="duid">DUID</param>
        /// <param name="iatype">IA type</param>
        /// <param name="iaid">IAID</param>
        /// <param name="state"></param>
        /// <returns> binding (a wrapped IdentityAssoc)</returns>
        private Binding BuildBinding(DhcpLink clientLink, byte[] duid, byte iatype, long iaid,
                byte state)
        {
            IdentityAssoc ia = new IdentityAssoc();
            ia.SetDuid(duid);
            ia.SetIatype(iatype);
            ia.SetIaid(iaid);
            ia.SetState(state);
            return new Binding(ia, clientLink);
        }

        /// <summary>
        /// Sets the lifetimes of the given binding object.
        /// </summary>
        /// <param name="bindingObj">binding object</param>
        /// <param name="preferred">preferred lifetime in ms</param>
        /// <param name="valid">valid lifetime in ms</param>
        protected void SetBindingObjectTimes(BindingObject bindingObj, long preferred, long valid)
        {
            if (_log.IsDebugEnabled)
                _log.Debug("Updating binding times for address: " +
                        bindingObj.GetIpAddress().ToString() +
                        " preferred=" + preferred + ", valid=" + valid);
            DateTime now = DateTime.Now;
            bindingObj.SetStartTime(now);
            if (preferred < 0)
            {
                // infinite lease
                bindingObj.SetPreferredEndTime(new DateTime(-1));
            }
            else
            {
                bindingObj.SetPreferredEndTime(now.AddMilliseconds(preferred));
            }
            if (valid < 0)
            {
                // infinite lease
                bindingObj.SetValidEndTime(now.AddDays(-1));
            }
            else
            {
                bindingObj.SetValidEndTime(now.AddMilliseconds(valid));
            }
        }

        /// <summary>
        /// Create a binding in response to a Solicit/Discover request for the given client IA.
        /// </summary>
        /// <param name="clientLink">link for the client request message</param>
        /// <param name="duid">DUID of the client</param>
        /// <param name="iatype"> IA type of the client request</param>
        /// <param name="iaid">IAID of the client request</param>
        /// <param name="requestAddrs"></param>
        /// <param name="requestMsg"></param>
        /// <param name="state"></param>
        /// <returns>created Binding</returns>
        protected Binding CreateBinding(DhcpLink clientLink, byte[] duid, byte iatype, long iaid, List<IPAddress> requestAddrs, DhcpMessage requestMsg, byte state, IPAddress clientV4IPAddress)
        {
            Binding binding = null;
            lock (_lock)
            {
                try
                {
                    _log.Debug("Getting addresses for new binding");
                    List<IPAddress> inetAddrs = GetInetAddrs(clientLink, duid, iatype, iaid, requestAddrs, requestMsg, clientV4IPAddress);
                    if ((inetAddrs != null) && inetAddrs.Count > 0)
                    {
                        _log.Debug("Got " + inetAddrs.Count + " addresses, building binding");
                        binding = BuildBinding(clientLink, duid, iatype, iaid, state);
                        _log.Debug("Building binding objects");
                        HashSet<BindingObject> bindingObjs = BuildBindingObjects(clientLink, inetAddrs, requestMsg, state);
                        if ((bindingObjs != null) && bindingObjs.Count > 0)
                        {
                            binding.SetBindingObjects(bindingObjs);
                            _log.Info("Creating new binding");
                            try
                            {
                                iaMgr.CreateIA(binding);
                            }
                            catch (Exception ex)
                            {
                                _log.Error("Failed to create persistent binding");
                                return null;
                            }
                        }
                        else
                        {
                            _log.Error("Failed to build binding object(s)");
                            return null;
                        }
                    }

                    string bindingType = (iatype == IdentityAssoc.V4_TYPE) ? "discover" : "solicit";
                    if (binding != null)
                    {
                        _log.Info("Created " + bindingType + " binding: " + binding.ToString());
                    }
                    else
                    {
                        _log.Warn("Failed to create " + bindingType + " binding");
                    }
                }
                catch (Exception ex)
                {
                    _log.WarnFormat("BaseDhcpV4Processor ProcessMessage Faile. exMessage:{0} exStackTrace:{1}", ex.Message, ex.StackTrace);
                    return null;
                }
                return binding;
            }
        }

        /// <summary>
        /// Get list of IP addresses for the given client IA request.
        /// </summary>
        /// <param name="clientLink">link for the client request message</param>
        /// <param name="duid">DUID of the client</param>
        /// <param name="iatype">IA type of the client request</param>
        /// <param name="iaid">IAID of the client request</param>
        /// <param name="requestAddrs">list of requested IP addresses, if any</param>
        /// <param name="requestMsg">client request message</param>
        /// <returns>list of IPAddress</returns>
        protected List<IPAddress> GetInetAddrs(DhcpLink clientLink, byte[] duid, byte iatype, long iaid, List<IPAddress> requestAddrs, DhcpMessage requestMsg, IPAddress clientV4IPAddress)
        {
            List<IPAddress> inetAddrs = new List<IPAddress>();

            if ((requestAddrs != null) && requestAddrs.Count > 0)
            {
                foreach (IPAddress reqAddr in requestAddrs)
                {
                    IPAddress addr = reqAddr;
                    if (!addr.Equals(DhcpConstants.ZEROADDR_V6))
                    {
                        BindingPool bp = FindBindingPool(clientLink.GetLink(), addr, requestMsg);
                        if (bp.IsFree(new BigInteger(addr.GetAddressBytes())) == false) continue;
                        if (bp == null)
                        {

                            _log.Warn("No BindingPool found for requested client address: " + addr.ToString());
                            if (iatype == IdentityAssoc.PD_TYPE)
                            {
                                // TAHI tests want NoPrefixAvail in this case
                                _log.Warn("Requested prefix is not available, returning");
                                return inetAddrs;
                            }
                            // if there is no pool for the requested address, then skip it
                            // because that address is either off-link or no longer valid
                            continue;
                        }
                        _log.Info("Searching existing bindings for requested IP=" + addr.ToString());
                        IdentityAssoc ia = null;
                        try
                        {
                            ia = iaMgr.FindIA(addr);
                            if (ia != null)
                            {
                                // the address is assigned to an IA, which we
                                // don't expect to be this IA, because we would
                                // have found it using findCurrentBinding...
                                // but, perhaps another thread just created it?
                                if (IsMyIa(duid, iatype, iaid, ia))
                                {
                                    _log.Warn("Requested IP=" + addr.ToString() + " is already held by THIS client " + IdentityAssoc.KeyToString(duid, iatype, iaid) + ".  Allowing this requested IP.");
                                }
                                else
                                {
                                    _log.Info("Requested IP=" + addr.ToString() + " is held by ANOTHER client " + IdentityAssoc.KeyToString(duid, iatype, iaid));
                                    // the address is held by another IA, so get a new one
                                    addr = GetNextFreeAddress(clientLink, requestMsg, clientV4IPAddress);
                                }
                            }
                            if (addr != null)
                            {
                                inetAddrs.Add(addr);
                            }
                        }
                        catch
                        {
                            _log.Error("Failure finding IA for address");
                        }
                    }
                }
            }

            if (inetAddrs == null || inetAddrs.Count == 0)
            {
                // the client did not request any valid addresses, so get the next one
                IPAddress inetAddr = GetNextFreeAddress(clientLink, requestMsg, clientV4IPAddress);
                if (inetAddr != null)
                {
                    inetAddrs.Add(inetAddr);
                }
            }
            return inetAddrs;
        }

        /// <summary>
        /// Gets the next free address from the pool(s) configured for the client link.
        /// </summary>
        /// <param name="clientLink">client link</param>
        /// <param name="requestMsg">request message</param>
        /// <returns></returns>
        protected IPAddress GetNextFreeAddress(DhcpLink clientLink, DhcpMessage requestMsg, IPAddress clientV4IPAddress)
        {
            if (clientLink != null)
            {
                List<BindingPool> pools = bindingPoolMap.ContainsKey(clientLink.GetLinkAddress()) ? bindingPoolMap[clientLink.GetLinkAddress()] : null;
                if ((pools != null) && pools.Count > 0)
                {
                    foreach (BindingPool bp in pools)
                    {
                        linkFilter filter = bp.GetLinkFilter();
                        if ((requestMsg != null) && (filter != null))
                        {
                            if (!DhcpServerConfiguration.MsgMatchesFilter(requestMsg, filter))
                            {
                                _log.Info("Client request does not match filter, skipping pool: " + bp.ToString());
                                continue;
                            }
                        }
                        _log.Debug("Getting next available address from pool: " + bp.ToString());

                        if (clientV4IPAddress != null)
                        {
                            //if (bp.GetV6AssignRule() != AssignDhcpV6Rule.Noen && bp.GetV6AssignRule() != AssignDhcpV6Rule.AssignV6IPAndDNSByV6Pool)
                            //{
                            //    string[] clientV4IPArray = clientV4IPAddress.ToString().Split('.');
                            //    IPAddress clientV6Addr = null;
                            //    if (bp.GetV6AssignRule() == AssignDhcpV6Rule.AssignV6IPAndDNSByTransferV4IPLast2)
                            //    {
                            //        clientV6Addr = bp.GetNextAvailableAddress(clientV4IPArray[2], clientV4IPArray[3]);
                            //    }
                            //    else
                            //    {
                            //        clientV6Addr = bp.GetNextAvailableAddress("", clientV4IPArray[3]);
                            //    }
                            //    if (clientV6Addr != null)
                            //    {
                            //        return clientV6Addr;
                            //    }
                            //    // Callback sreach V4 IP Using MAC
                            //    // 依照回傳的 IPv4 IP 決定回傳的 IPv6 IP
                            //    // Return new IPAddress(clientLink.GetLinkAddress() + "IPv4 尾 1 碼或 2 碼")
                            //    // byte[] v4TransferV6 = new byte[] { 32, 1, 176, 48, 17, 40, 1, 96, 0, 0, 0, 0, 0, 97, 1, 151 };
                            //    // return new IPAddress(v4TransferV6);
                            //    // 若無 v4 IP 則依照既有的邏輯進行派發，派發的 IP 從::255:255 以後開始派發
                            //}
                        }
                        IPAddress free = bp.GetNextAvailableAddress();
                        if (free != null)
                        {
                            _log.Debug("Found next available address: " + free.ToString());
                            return free;
                        }
                        else
                        {
                            // warning here, b/c there may be more pools
                            _log.Warn("No free addresses available in pool: " + bp.ToString());
                        }
                    }
                    // if we get this far, then we did not find any free(virgin) addresses
                    // so we must start searching for any that can be used in the pools
                    foreach (BindingPool bp in pools)
                    {
                        linkFilter filter = bp.GetLinkFilter();
                        if ((requestMsg != null) && (filter != null))
                        {
                            if (!DhcpServerConfiguration.MsgMatchesFilter(requestMsg, filter))
                            {
                                _log.Info("Client request does not match filter, skipping pool: " + bp.ToString());
                                continue;
                            }
                        }
                        IPAddress reused = ReuseAvailableAddress(bp);
                        if (reused != null)
                        {
                            return reused;
                        }
                    }
                }
                else
                {
                    _log.Error("No Pools defined in server configuration for Link: " + clientLink.GetLinkAddress());
                }
            }
            else
            {
                throw new Exception("ClientLink is null");
            }
            return null;
        }

        /// <summary>
        /// Find an address that can be reused.  This method is invoked only
        /// when no "virgin" leases can be found for a new client request.
        /// </summary>
        /// <param name="bp"> binding pool</param>
        /// <returns>the oldest available address, if any</returns>
        protected IPAddress ReuseAvailableAddress(BindingPool bp)
        {
            Monitor.Enter(_lock);
            try
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("Finding available addresses in pool: " +
                            bp.ToString());
                List<IaAddress> iaAddrs =
                    iaMgr.FindUnusedIaAddresses(bp.GetStartAddress(), bp.GetEndAddress());
                if ((iaAddrs != null) && iaAddrs.Count > 0)
                {
                    if (_log.IsDebugEnabled)
                    {
                        foreach (IaAddress iaAddre in iaAddrs)
                        {
                            _log.Debug("Found available address: " + iaAddre.ToString());
                        }
                    }
                    // list is ordered by validendtime
                    // so the first one is the oldest one
                    IaAddress iaAddr = iaAddrs.First();
                    _log.Info("Deleting oldest available address: " + iaAddr.ToString());
                    // delete the oldest one and return the IP
                    // allowing that IP to be used again
                    iaMgr.DeleteIaAddr(iaAddr);
                    return iaAddr.GetIpAddress();
                    // TODO: should we clear the rest of unused IPs
                    // now, or wait for them to expire or be needed
                    //				for (int i=1; i<iaAddrs.size(); i++) {
                    //					
                    //				}
                }
            }
            finally
            {
                Monitor.Exit(_lock);
            }

            return null;
        }

        /// <summary>
        /// Checks if the duid-iatype-iaid tuple matches the given IA.
        /// </summary>
        /// <param name="duid">DUID</param>
        /// <param name="iatype">IA type</param>
        /// <param name="iaid">IAID</param>
        /// <param name="ia">IA</param>
        /// <returns>true, if is my ia</returns>
        protected bool IsMyIa(byte[] duid, byte iatype, long iaid, IdentityAssoc ia)
        {
            bool rc = false;
            if (duid != null)
            {
                if (Array.Equals(ia.GetDuid(), duid) &&
                    (ia.GetIatype() == iatype) &&
                    (ia.GetIaid() == iaid))
                {
                    rc = true;
                }
            }
            return rc;
        }

        protected BindingPool FindBindingPool(link link, IPAddress inetAddr, DhcpMessage requestMsg)
        {
            List<BindingPool> bps = bindingPoolMap.ContainsKey(link.Address) ? bindingPoolMap[link.Address] : null;
            if ((bps != null) && bps.Count > 0)
            {
                foreach (BindingPool bindingPool in bps)
                {
                    //				if (log.isDebugEnabled()) {
                    //					if (bindingPool instanceof AddressBindingPool) {
                    //						AddressBindingPool abp = (AddressBindingPool) bindingPool;
                    //						log.Debug("AddressBindingPool: " + abp.toString());
                    //						log.Debug("FreeList: " + abp.freeListToString());
                    //					}
                    //				}
                    if (bindingPool.Contains(inetAddr))
                    {
                        if ((requestMsg != null) && (bindingPool.GetLinkFilter() != null))
                        {
                            if (DhcpServerConfiguration.MsgMatchesFilter(requestMsg,
                                    bindingPool.GetLinkFilter()))
                            {
                                _log.Info("Found filter binding pool: " + bindingPool);
                                return bindingPool;
                            }
                        }
                        else
                        {
                            _log.Info("Found binding pool: " + bindingPool);
                            return bindingPool;
                        }
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// Builds the set of binding objects for the client request.
        /// </summary>
        /// <param name="clientLink">client link</param>
        /// <param name="inetAddrs">the list of IP addresses for the client binding</param>
        /// <param name="requestMsg">request message</param>
        /// <param name="state">binding state</param>
        /// <returns>the HashSet<binding object></returns>
        private HashSet<BindingObject> BuildBindingObjects(DhcpLink clientLink,
                List<IPAddress> inetAddrs, DhcpMessage requestMsg,
                byte state)
        {
            HashSet<BindingObject> bindingObjs = new HashSet<BindingObject>();
            foreach (IPAddress inetAddr in inetAddrs)
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("Building BindingObject for IP=" + inetAddr.ToString());
                BindingObject bindingObj = BuildBindingObject(inetAddr, clientLink, requestMsg);
                if (bindingObj != null)
                {
                    bindingObj.SetState(state);
                    bindingObjs.Add(bindingObj);
                }
                else
                {
                    _log.Warn("Failed to build BindingObject for IP=" + inetAddr.ToString());
                }
            }
            return bindingObjs;
        }

        /// <summary>
        /// Initialize the static bindings for the given DhcpLink.  The StaticBindings
        /// are either StaticAddressBindings (NA and TA) or StaticPrefixBindings
        /// or StaticV4AddressBindings.
        /// </summary>
        /// <param name="link">configured DhcpLink</param>
        /// <returns>the list of StaticBindings for the link</returns>
        protected abstract List<StaticBinding> BuildStaticBindings(link link);

        /// <summary>
        /// The map of binding binding pools for this manager.  The key is the link address
        ///  and the value is the list of configured BindingPools for the link. 
        /// </summary>
        protected Dictionary<string, List<BindingPool>> bindingPoolMap;

        /// <summary>
        /// The map of static bindings for this manager.  The key is the link address
        /// and the value is the list of configured StaticBindings for the link. 
        /// </summary>
        protected Dictionary<string, List<StaticBinding>> staticBindingMap;


        /// <summary>
        /// Build the appropriate type of BindingObject.  This method is implemented by the
        /// subclasses to create an NA/TA BindingAddress object or a BindingPrefix object
        /// or a v4AddressBinding object.
        /// </summary>
        /// <param name="inetAddr">IP address for this binding object</param>
        /// <param name="clientLink">client link</param>
        /// <param name="requestMsg">request message</param>
        /// <returns>binding object</returns>
        protected abstract BindingObject BuildBindingObject(IPAddress inetAddr,
                DhcpLink clientLink, DhcpMessage requestMsg);


        /// <summary>
        ///  Create a Binding given an IdentityAssoc loaded from the database.
        /// </summary>
        /// <param name="ia">IA</param>
        /// <param name="clientLink">client link</param>
        /// <param name="requestMsg">request message</param>
        /// <returns>binding</returns>
        protected abstract Binding BuildBindingFromIa(IdentityAssoc ia,
                DhcpLink clientLink, DhcpMessage requestMsg);

        /// <summary>
        ///  Find the current binding, if any, for the given client identity association (IA).
        /// </summary>
        /// <param name="clientLink">link for the client request message</param>
        /// <param name="duid">DUID of the client</param>
        /// <param name="iatype">the IAID of the client request</param>
        /// <param name="iaid"> IAID of the client request</param>
        /// <param name="requestMsg">requestMsg the client request message</param>
        /// <returns>return the existing Binding for this client request</returns>
        protected Binding FindCurrentBinding(DhcpLink clientLink, byte[] duid, byte iatype, long iaid, DhcpMessage requestMsg)
        {
            Binding binding = null;
            lock (_lock)
            {
                try
                {
                    IdentityAssoc ia = iaMgr.FindIA(duid, iatype, iaid);
                    if (ia != null)
                    {
                        _log.Info("Found current binding for " + IdentityAssoc.KeyToString(duid, iatype, iaid) + " state=" + ia.GetState());
                        binding = BuildBindingFromIa(ia, clientLink, requestMsg);
                        if (binding != null)
                        {
                            _log.Info("Successfully built Binding object: " + binding);
                        }
                        else
                        {
                            _log.Error("Failed to build Binding object");
                        }
                    }
                    else
                    {
                        _log.Info("No current binding found for " + IdentityAssoc.KeyToString(duid, iatype, iaid));
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Failed to find current binding");
                }
            }
            return binding;
        }
        /**
        * Sets and IP address as free in it's binding pool.
        * 
        * @param inetAddr the IP address to free
*/
        protected void FreeAddress(IPAddress inetAddr)
        {
            BindingPool bp = FindBindingPool(inetAddr);
            if (bp != null)
            {
                bp.SetFree(inetAddr);
            }
            else
            {
                _log.Error("Failed to free address: no BindingPool found for address: " +
                        inetAddr.ToString());
            }
        }

        private BindingPool FindBindingPool(IPAddress inetAddr)
        {
            Dictionary<string, List<BindingPool>> allPools = bindingPoolMap;
            if ((allPools != null) && allPools.Count > 0)
            {
                foreach (var bps in allPools)
                {
                    foreach (BindingPool bindingPool in bps.Value)
                    {
                        if (bindingPool.Contains(inetAddr))
                        {
                            _log.Info("Found binding pool for address=" +
                                    inetAddr.ToString() +
                                    ": " + bindingPool);
                            return bindingPool;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Update an existing client binding.  Addresses in the current binding that are appropriate
        /// for the client's link are simply updated with new lifetimes.  If no current bindings are
        /// appropriate for the client link, new binding addresses will be created and added to the
        /// existing binding, leaving any other addresses alone.
        /// </summary>
        /// <param name="binding">existing client binding</param>
        /// <param name="clientLink">link for the client request message</param>
        /// <param name="duid">DUID of the client</param>
        /// <param name="iatype">IA type of the client request</param>
        /// <param name="iaid">IAID of the client request</param>
        /// <param name="requestAddrs">list of requested IP addresses, if any</param>
        /// <param name="requestMsg">client request message</param>
        /// <param name="state">the new state for the binding</param>
        /// <returns>updated Binding</returns>
        protected Binding UpdateBinding(Binding binding, DhcpLink clientLink, byte[] duid, byte iatype, long iaid, List<IPAddress> requestAddrs, DhcpMessage requestMsg, byte state, IPAddress clientV4IPAddress)
        {
            List<IaAddress> addIaAddresses = null;
            List<IaAddress> updateIaAddresses = null;
            List<IaAddress> delIaAddresses = null;    // not used currently

            //		log.info("Updating dynamic binding: " + binding);

            HashSet<BindingObject> bindingObjs = binding.GetBindingObjects();
            if ((bindingObjs != null) && bindingObjs.Count > 0)
            {
                // current binding has addresses, so update times
                SetBindingObjsTimes(bindingObjs);
                // the existing IaAddress binding objects will be updated
                updateIaAddresses = binding.GetIaAddresses();
            }
            else
            {
                _log.Warn("Existing binding has no on-link addresses, allocating new address(es)");
                // current binding has no addresses, add new one(s)
                List<IPAddress> inetAddrs = GetInetAddrs(clientLink, duid, iatype, iaid, requestAddrs, requestMsg, clientV4IPAddress);
                if ((inetAddrs == null) || inetAddrs.Count == 0)
                {
                    _log.Error("Failed to update binding, no addresses available");
                    return null;
                }
                bindingObjs = BuildBindingObjects(clientLink, inetAddrs, requestMsg, state);
                binding.SetBindingObjects(bindingObjs);
                // these new IaAddress binding objects will be added
                addIaAddresses = binding.GetIaAddresses();
            }
            binding.SetState(state);
            try
            {
                _log.Info("Updating binding");
                iaMgr.UpdateIA(binding, addIaAddresses, updateIaAddresses, delIaAddresses);
                _log.Info("Binding updated: " + binding.ToString());
                return binding; // if we get here, it worked
            }
            catch (Exception ex)
            {
                _log.Error("Failed to update binding");
                return null;
            }
        }

        /// <summary>
        /// Update an existing static binding.
        /// </summary>
        /// <param name="binding">existing client binding</param>
        /// <param name="clientLink">link for the client request message</param>
        /// <param name="duid">DUID of the client</param>
        /// <param name="iatype">IA type of the client request</param>
        /// <param name="iaid">IAID of the client request</param>
        /// <param name="staticBinding">static binding</param>
        /// <param name="requestMsg">client request message</param>
        /// <returns>updated Binding</returns>
        protected Binding UpdateStaticBinding(Binding binding, DhcpLink clientLink,
                byte[] duid, byte iatype, long iaid, StaticBinding staticBinding,
                DhcpMessage requestMsg)
        {
            List<IaAddress> addIaAddresses = null;
            List<IaAddress> updateIaAddresses = null;
            List<IaAddress> delIaAddresses = null;    // not used currently

            if (staticBinding != null)
            {
                _log.Info("Updating static binding: " + binding);
                HashSet<BindingObject> bindingObjs = binding.GetBindingObjects();
                if ((bindingObjs != null) && bindingObjs.Count > 0)
                {
                    foreach (BindingObject bindingObj in bindingObjs)
                    {
                        if (bindingObj.GetIpAddress().Equals(staticBinding.GetIpAddress()))
                        {
                            SetBindingObjectTimes(bindingObj,
                                    staticBinding.GetPreferredLifetimeMs(),
                                    staticBinding.GetPreferredLifetimeMs());
                            break;
                        }
                        //TODO: what about bindingObjs that do NOT match the static binding?
                    }
                    // the existing IaAddress binding objects will be updated
                    updateIaAddresses = binding.GetIaAddresses();
                }
                else
                {
                    IPAddress inetAddr = staticBinding.GetInetAddress();
                    if (inetAddr != null)
                    {
                        BindingObject bindingObj = BuildStaticBindingObject(inetAddr, staticBinding);
                        bindingObjs = new HashSet<BindingObject>();
                        bindingObjs.Add(bindingObj);
                        binding.SetBindingObjects(bindingObjs);
                        // this new IaAddress binding object will be added
                        addIaAddresses = binding.GetIaAddresses();
                    }
                }
            }
            else
            {
                _log.Error("StaticBindingObject is null");
            }

            binding.SetState(IaAddress.STATIC);
            try
            {
                iaMgr.UpdateIA(binding, addIaAddresses, updateIaAddresses, delIaAddresses);
                return binding; // if we get here, it worked
            }
            catch (Exception ex)
            {
                _log.Error("Failed to update binding");
                return null;
            }
        }

        /// <summary>
        /// Sets the lifetimes of the given collection of binding objects.
        /// </summary>
        /// <param name="bindingObjs">Sets the collection of binding objects to set lifetimes in</param>
        protected void SetBindingObjsTimes(HashSet<BindingObject> bindingObjs)
        {
            if ((bindingObjs != null) && bindingObjs.Count > 0)
            {
                foreach (BindingObject bindingObj in bindingObjs)
                {
                    DhcpConfigObject configObj = bindingObj.GetConfigObj();
                    SetBindingObjectTimes(bindingObj,
                            configObj.GetPreferredLifetimeMs(),
                            configObj.GetValidLifetimeMs());
                    //TODO: if we store the options, and they have changed,
                    // 		then we must update those options here somehow
                }
            }
        }

        /// <summary>
        /// Build a BindingObject from a static binding.  Because the StaticBinding
        /// implements the DhcpConfigObject interface, it can be used directly in
        /// creating the BindingObject, unlike buildBindingObject above which is
        /// abstract because each implementation must lookup the object in that
        /// binding manager's map of binding pools.
        /// </summary>
        /// <param name="inetAddr">IP address for this binding object</param>
        /// <param name="staticBinding">static binding</param>
        /// <returns>binding object</returns>
        protected BindingObject BuildStaticBindingObject(IPAddress inetAddr,
                PIXIS.DHCP.Request.Bind.StaticBinding staticBinding)
        {
            IaAddress iaAddr = new IaAddress();
            iaAddr.SetIpAddress(inetAddr);
            V6BindingAddress bindingAddr = new V6BindingAddress(iaAddr, staticBinding);
            SetBindingObjectTimes(bindingAddr,
                    staticBinding.GetPreferredLifetimeMs(),
                    staticBinding.GetPreferredLifetimeMs());
            return bindingAddr;
        }

        /// <summary>
        /// Sets an IP address as in-use in it's binding pool.
        /// </summary>
        /// <param name="link">link for the message</param>
        /// <param name="inetAddr">IP address to set used</param>
        protected void SetIpAsUsed(link link, IPAddress inetAddr)
        {
            BindingPool bindingPool = FindBindingPool(link, inetAddr, null);
            if (bindingPool != null)
            {
                bindingPool.SetUsed(inetAddr);
            }
            else
            {
                _log.Warn("Unable to set address used: No BindingPool found for IP=" +
                        inetAddr.ToString());
            }
        }

        public IaManager GetIaMgr()
        {
            return iaMgr;
        }

        public void SetIaMgr(IaManager iaMgr)
        {
            this.iaMgr = iaMgr;
        }
    }
}
