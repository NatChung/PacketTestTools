using PIXIS.DHCP.Config;
using PIXIS.DHCP.DB;
using PIXIS.DHCP.Message;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Option.V4;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static PIXIS.DHCP.Config.DhcpServerPolicies;

namespace PIXIS.DHCP.Request.Bind
{
    public class V4AddrBindingManagerImpl : BaseAddrBindingManager, V4AddrBindingManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly static object _lock = new object();
        /// <summary>
        /// DHCP 派發IP前，確認IP是否已經使用
        /// </summary>
        /// <param name="ip">檢查IP</param>
        /// <param name="wait">等待時間(毫秒)</param>
        /// <returns></returns>
        /**
         * Instantiates a new v4 addr binding manager impl.
         * 
         * @throws DhcpServerConfigException the dhcp server config exception
         */
        public V4AddrBindingManagerImpl() : base()
        {
        }

        /**
         * Build the list of V4AddressBindingPools from the list of configured V4AddressPools
         * for the given configuration link container object. The list of V4AddressBindingPools
         * starts with the filtered V4AddressPools followed by non-filtered V4AddressPools.
         * 
         * @param link the configuration link object
         * 
         * @return the list of V4AddressBindingPools (<? extends BindingPool>)
         * 
         * @throws DhcpServerConfigException if there is a problem parsing a configured range
         */
        protected override List<BindingPool> BuildBindingPools(link link)
        {
            Debug.Assert(CheckIPIsUsed != null, "V4AddrBindingManagerImpl --BuildBindingPools-- CheckIPIsUsed = null");
            List<BindingPool> bindingPools = new List<BindingPool>();
            // Put the filtered pools first in the list of pools on this link
            List<linkFilter> linkFilters = link.linkFilters;
            if ((linkFilters != null) && linkFilters.Count > 0)
            {
                foreach (linkFilter linkFilter in linkFilters)
                {
                    List<v4AddressPool> pools = linkFilter.v4AddrPools;

                    if ((pools != null) && pools.Count > 0)
                    {
                        foreach (v4AddressPool pool in pools)
                        {
                            V4AddressBindingPool abp = BuildV4BindingPool(pool, link, linkFilter);
                            abp.CheckIPIsUsed = CheckIPIsUsed;
                            bindingPools.Add(abp);
                        }
                    }
                    else
                    {
                        log.Error("PoolList is null for PoolsType");
                    }
                }
            }
            // add the unfiltered pools to the mapped list
            List<v4AddressPool> unpools = link.v4AddrPools;
            if ((unpools != null) && unpools.Count > 0)
            {
                foreach (v4AddressPool pool in unpools)
                {
                    V4AddressBindingPool abp = BuildV4BindingPool(pool, link);
                    abp.CheckIPIsUsed = CheckIPIsUsed;
                    bindingPools.Add(abp);
                }
            }
            else
            {
                log.Error("PoolList is null for PoolsType");
            }


            ReconcilePools(bindingPools);

            return bindingPools;
        }

        /**
         * Reconcile pools.  Delete any IaAddress objects not contained
         * within the given list of V4AddressBindingPools.
         * 
         * @param bindingPools the list of V4AddressBindingPools
         */
        protected void ReconcilePools(List<BindingPool> bindingPools)
        {
            if ((bindingPools != null) && bindingPools.Count > 0)
            {
                List<Range> ranges = new List<Range>();
                foreach (V4AddressBindingPool bp in bindingPools)
                {
                    Range range = new Range(bp.GetStartAddress(), bp.GetEndAddress());
                    ranges.Add(range);
                }
                iaMgr.ReconcileIaAddresses(ranges);
            }
        }

        /**
         * Builds a binding pool from an V4AddressPool using the given link.
         * 
         * @param pool the V4AddressPool to wrap as an V4AddressBindingPool
         * @param link the link
         * 
         * @return the binding pool
         * 
         * @throws DhcpServerConfigException if there is a problem parsing the configured range
         */
        protected V4AddressBindingPool BuildV4BindingPool(v4AddressPool pool, link link)
        {
            return BuildV4BindingPool(pool, link, null);
        }

        /**
         * Builds a binding pool from an V4AddressPool using the given link and filter.
         * 
         * @param pool the V4AddressPool to wrap as an V4AddressBindingPool
         * @param link the link
         * @param linkFilter the link filter
         * 
         * @return the binding pool
         * 
         * @throws DhcpServerConfigException if there is a problem parsing the configured range
         */
        protected V4AddressBindingPool BuildV4BindingPool(v4AddressPool pool, link link,
                linkFilter linkFilter)
        {
            V4AddressBindingPool bp = new V4AddressBindingPool(pool);
            long leasetime =
                DhcpServerPolicies.EffectivePolicyAsLong(bp, link, Property.V4_DEFAULT_LEASETIME);
            bp.SetLeasetime(leasetime);
            bp.SetLinkFilter(linkFilter);

            List<IPAddress> usedIps = iaMgr.FindExistingIPs(bp.GetStartAddress(), bp.GetEndAddress());
            if ((usedIps != null) && usedIps.Count > 0)
            {
                foreach (IPAddress ip in usedIps)
                {
                    //TODO: for the quickest startup?...
                    // set IP as used without checking if the binding has expired
                    // let the reaper thread deal with all binding cleanup activity
                    bp.SetUsed(ip);
                }
            }
            log.Info("Built v4 address binding pool: " + bp.GetStartAddress().ToString() + "-" +
                            bp.GetEndAddress().ToString() + " size=" + bp.GetSize());
            return bp;
        }

        protected override List<StaticBinding> BuildStaticBindings(link link)
        {
            List<StaticBinding> staticBindings = new List<StaticBinding>();
            List<v4AddressBinding> bindings = link.v4AddrBindings;

            if ((bindings != null) && bindings.Count > 0)
            {
                foreach (v4AddressBinding binding in bindings)
                {
                    V4StaticAddressBinding sab = BuildV4StaticBinding(binding, link);
                    staticBindings.Add(sab);
                }
            }

            return staticBindings;
        }

        protected V4StaticAddressBinding BuildV4StaticBinding(v4AddressBinding binding, link link)
        {
            try
            {
                IPAddress inetAddr = IPAddress.Parse(binding.ipAddress);
                V4StaticAddressBinding sb = new V4StaticAddressBinding(binding);
                SetIpAsUsed(link, inetAddr);
                return sb;
            }
            catch (Exception ex)
            {
                log.Error("Invalid static binding address");
                throw new Exception("Invalid static binding address", ex);
            }
        }

        public Binding FindCurrentBinding(DhcpLink clientLink, byte[] macAddr,
                DhcpMessage requestMsg)
        {
            lock (_lock)
            {
                return base.FindCurrentBinding(clientLink, macAddr, IdentityAssoc.V4_TYPE,
                    0, requestMsg);
            }
        }

        public Binding CreateDiscoverBinding(DhcpLink clientLink, byte[] macAddr,
                DhcpMessage requestMsg, byte state)
        {
            lock (_lock)
            {
                StaticBinding staticBinding =
                FindStaticBinding(clientLink.GetLink(), macAddr, IdentityAssoc.V4_TYPE, 0, requestMsg);

                if (staticBinding != null)
                {
                    return base.CreateStaticBinding(clientLink, macAddr, IdentityAssoc.V4_TYPE,
                            0, staticBinding, requestMsg);
                }
                else
                {
                    return base.CreateBinding(clientLink, macAddr, IdentityAssoc.V4_TYPE,
                            0, GetInetAddrs(requestMsg), requestMsg, state, null);
                }
            }
        }

        public Binding UpdateBinding(Binding binding, DhcpLink clientLink,
                byte[] macAddr, DhcpMessage requestMsg, byte state)
        {

            StaticBinding staticBinding =
                FindStaticBinding(clientLink.GetLink(), macAddr, IdentityAssoc.V4_TYPE, 0, requestMsg);

            if (staticBinding != null)
            {
                return base.UpdateStaticBinding(binding, clientLink, macAddr, IdentityAssoc.V4_TYPE,
                        0, staticBinding, requestMsg);
            }
            else
            {
                return base.UpdateBinding(binding, clientLink, macAddr, IdentityAssoc.V4_TYPE,
                        0, GetInetAddrs(requestMsg), requestMsg, state, null);
            }
        }

        /**
         * Get the Requested IP addresses from the client message, if any was provided.
         * 
         * @param requestMsg the request msg
         * 
         * @return a list of InetAddresses containing the requested IP, or null if none requested or
         *         if the requested IP is bogus
         */
        private List<IPAddress> GetInetAddrs(DhcpMessage requestMsg)
        {
            List<IPAddress> inetAddrs = new List<IPAddress>();
            DhcpV4RequestedIpAddressOption reqIpOption = (DhcpV4RequestedIpAddressOption)
                requestMsg.GetDhcpOption(DhcpConstants.V4OPTION_REQUESTED_IP);
            if (reqIpOption != null)
            {
                IPAddress inetAddr =
                       IPAddress.Parse(reqIpOption.GetIpAddress());
                inetAddrs = new List<IPAddress>();
                inetAddrs.Add(inetAddr);
            }
            return inetAddrs;
        }

        /**
         * Create a Binding given an IdentityAssoc loaded from the database.
         * 
         * @param ia the ia
         * @param clientLink the client link
         * @param requestMsg the request msg
         * 
         * @return the binding
         */
        protected override Binding BuildBindingFromIa(IdentityAssoc ia, DhcpLink clientLink,
                DhcpMessage requestMsg)
        {
            Binding binding = new Binding(ia, clientLink);
            List<IaAddress> iaAddrs = ia.GetIaAddresses();
            if ((iaAddrs != null) && iaAddrs.Count > 0)
            {
                List<IaAddress> bindingAddrs = new List<IaAddress>();
                foreach (IaAddress iaAddr in iaAddrs)
                {
                    if (!clientLink.GetSubnet().Contains(iaAddr.GetIpAddress()))
                    {
                        log.Info("Ignoring off-link binding address: " +
                                iaAddr.GetIpAddress().ToString());
                        continue;
                    }
                    V4BindingAddress bindingAddr = null;
                    StaticBinding staticBinding =
                        FindStaticBinding(clientLink.GetLink(), ia.GetDuid(),
                                ia.GetIatype(), ia.GetIaid(), requestMsg);
                    if (staticBinding != null)
                    {
                        bindingAddr =
                            BuildV4StaticBindingFromIaAddr(iaAddr, staticBinding);
                    }
                    else
                    {
                        bindingAddr =
                            BuildV4BindingAddressFromIaAddr(iaAddr, clientLink.GetLink(), requestMsg);
                    }
                    if (bindingAddr != null)
                        bindingAddrs.Add(bindingAddr);
                }
                // replace the collection of IaAddresses with BindingAddresses
                binding.SetIaAddresses(bindingAddrs);
            }
            else
            {
                log.Warn("IA has no addresses, binding is empty.");
            }
            return binding;
        }

        /**
         * Create a V4BindingAddress given an IaAddress loaded from the database.
         * 
         * @param iaAddr the ia addr
         * @param clientLink the client link
         * @param requestMsg the request msg
         * 
         * @return the binding address
         */
        private V4BindingAddress BuildV4BindingAddressFromIaAddr(IaAddress iaAddr,
                link clientLink, DhcpMessage requestMsg)
        {
            IPAddress inetAddr = iaAddr.GetIpAddress();
            BindingPool bp = FindBindingPool(clientLink, inetAddr, requestMsg);
            if (bp != null)
            {
                // TODO store the configured options in the persisted binding?
                // ipAddr.setDhcpOptions(bp.getDhcpOptions());
                return new V4BindingAddress(iaAddr, (V4AddressBindingPool)bp);
            }
            else
            {
                log.Error("Failed to create V4BindingAddress: No V4BindingPool found for IP=" +
                        inetAddr.ToString());
            }
            // MUST have a BindingPool, otherwise something's broke
            return null;
        }

        private V4BindingAddress BuildV4StaticBindingFromIaAddr(IaAddress iaAddr,
                StaticBinding staticBinding)
        {
            V4BindingAddress bindingAddr = new V4BindingAddress(iaAddr, staticBinding);
            return bindingAddr;
        }

        protected override BindingObject BuildBindingObject(IPAddress inetAddr,
                DhcpLink clientLink, DhcpMessage requestMsg)
        {
            V4AddressBindingPool bp =
                (V4AddressBindingPool)FindBindingPool(clientLink.GetLink(), inetAddr, requestMsg);
            if (bp != null)
            {
                bp.SetUsed(inetAddr);   // TODO check if this is necessary
                IaAddress iaAddr = new IaAddress();
                iaAddr.SetIpAddress(inetAddr);
                V4BindingAddress bindingAddr = new V4BindingAddress(iaAddr, bp);
                SetBindingObjectTimes(bindingAddr,
                        bp.GetPreferredLifetimeMs(), bp.GetPreferredLifetimeMs());
                // TODO store the configured options in the persisted binding?
                // bindingAddr.setDhcpOptions(bp.getDhcpOptions());
                return bindingAddr;
            }
            else
            {
                log.Error("Failed to create V4BindingAddress: No V4BindingPool found for IP=" +
                        inetAddr.ToString());
            }
            // MUST have a BindingPool, otherwise something's broke
            return null;
        }


        /// <summary>
        /// Perform the DDNS delete processing when a lease is released or expired.
        /// </summary>
        /// <param name="ia">iaAddr the released or expired IaAddress </param>
        /// <param name="iaAddr">iaAddr the released or expired IaAddress </param>
        protected override void DdnsDelete(IdentityAssoc ia, IaAddress iaAddr)
        {
            DhcpV4ClientFqdnOption clientFqdnOption = null;
            try
            {
                if ((ia != null) && (iaAddr != null))
                {
                    List<DhcpOption> opts = iaAddr.GetDhcpOptions();
                    if (opts != null)
                    {
                        foreach (DhcpOption opt in opts)
                        {
                            if (opt.GetCode() == DhcpConstants.V4OPTION_CLIENT_FQDN)
                            {
                                clientFqdnOption = new DhcpV4ClientFqdnOption();
                                //clientFqdnOption.Decode(ByteBuffer.wrap(opt.GetValue()));
                                break;
                            }
                        }
                    }
                    if (clientFqdnOption != null)
                    {
                        string fqdn = clientFqdnOption.GetDomainName();
                        if ((fqdn != null) && fqdn.Count() > 0)
                        {
                            DhcpLink link = serverConfig.FindLinkForAddress(iaAddr.GetIpAddress());
                            if (link != null)
                            {
                                V4BindingAddress bindingAddr = null;
                                StaticBinding staticBinding =
                                    FindStaticBinding(link.GetLink(), ia.GetDuid(),
                                            ia.GetIatype(), ia.GetIaid(), null);
                                if (staticBinding != null)
                                {
                                    bindingAddr =
                                        BuildV4StaticBindingFromIaAddr(iaAddr, staticBinding);
                                }
                                else
                                {
                                    bindingAddr =
                                        BuildV4BindingAddressFromIaAddr(iaAddr, link.GetLink(), null);  // safe to send null requestMsg
                                }
                                if (bindingAddr != null)
                                {

                                    //DdnsCallback ddnsComplete =
                                    //    new DhcpV4DdnsComplete(bindingAddr, clientFqdnOption);

                                    //DhcpConfigObject configObj = bindingAddr.getConfigObj();

                                    //DdnsUpdater ddns =
                                    //    new DdnsUpdater(link.getLink(), configObj,
                                    //            bindingAddr.getIpAddress(), fqdn, ia.getDuid(),
                                    //            configObj.getValidLifetime(),
                                    //            clientFqdnOption.getUpdateABit(), true,
                                    //            ddnsComplete);

                                    //ddns.processUpdates();
                                }
                                else
                                {
                                    log.Error("Failed to find binding for address: " +
                                            iaAddr.GetIpAddress().ToString());
                                }
                            }
                            else
                            {
                                log.Error("Failed to find link for binding address: " +
                                        iaAddr.GetIpAddress().ToString());
                            }
                        }
                        else
                        {
                            log.Error("FQDN is null or empty.  No DDNS deletes performed.");
                        }
                    }
                    else
                    {
                        log.Warn("No Client FQDN option in current binding.  No DDNS deletes performed.");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Failed to perform DDNS delete");
            }
        }

        protected override byte GetIaType()
        {
            return IdentityAssoc.V4_TYPE;
        }
    }
}
