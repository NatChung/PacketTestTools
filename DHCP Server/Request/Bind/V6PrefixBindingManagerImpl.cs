using PIXIS.DHCP.Config;
using PIXIS.DHCP.DB;
using PIXIS.DHCP.Message;
using PIXIS.DHCP.Option.V6;
using PIXIS.DHCP.Xml;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static PIXIS.DHCP.Config.DhcpServerPolicies;

namespace PIXIS.DHCP.Request.Bind
{
    public class V6PrefixBindingManagerImpl : BaseBindingManager, V6PrefixBindingManager
    {
        /** The log. */
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public V6PrefixBindingManagerImpl() : base()
        {
        }

        protected override void StartReaper()
        {
            //TODO: separate properties for address/prefix binding managers?
            long reaperStartupDelay =
                DhcpServerPolicies.GlobalPolicyAsLong(Property.BINDING_MANAGER_REAPER_STARTUP_DELAY);
            long reaperRunPeriod =
                DhcpServerPolicies.GlobalPolicyAsLong(Property.BINDING_MANAGER_REAPER_RUN_PERIOD);

            //reaper = new Timer("BindingReaper");
            //reaper.schedule(new ReaperTimerTask(), reaperStartupDelay, reaperRunPeriod);
        }


        /**
         * Build the list of PrefixBindingPools from the list of configured PrefixPools
         * for the given configuration Link container object. The list of PrefixBindingPools
         * starts with the filtered PrefixPools followed by non-filtered PrefixPools.
         * 
         * @param link the configuration Link object
         * 
         * @return the list of PrefixBindingPools (<? extends BindingPool>)
         * 
         * @throws DhcpServerConfigException if there is a problem parsing a configured range
         */
        protected override List<BindingPool> BuildBindingPools(link link)
        {
            List<BindingPool> bindingPools = new List<BindingPool>();
            // Put the filtered pools first in the list of pools on this link
            List<linkFilter> linkFilters = link.linkFilters;

            foreach (linkFilter linkFilter in linkFilters)
            {
                List<v6PrefixPool> poolsType = linkFilter.v6PrefixPools;
                if (poolsType != null)
                {
                    // add the filtered pools to the mapped list
                    List<v6PrefixPool> pools = poolsType;
                    if ((pools != null) && pools.Count > 0)
                    {
                        foreach (v6PrefixPool pool in pools)
                        {
                            V6PrefixBindingPool abp = BuildV6BindingPool(pool, link, linkFilter);
                            bindingPools.Add(abp);
                        }
                    }
                    else
                    {
                        log.Error("PoolList is null for PoolsType: " + poolsType);
                    }
                }
                else
                {
                    log.Info("PoolsType is null for LinkFilter: " + linkFilter.name);
                }
            }

            // add the unfiltered pools to the mapped list
            List<v6PrefixPool> unpools = link.v6PrefixPools;
            if ((unpools != null) && unpools.Count > 0)
            {
                foreach (v6PrefixPool pool in unpools)
                {
                    V6PrefixBindingPool abp = BuildV6BindingPool(pool, link);
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
         * within the given list of PrefixBindingPools.
         * 
         * @param bindingPools the list of PrefixBindingPools
         */
        protected void ReconcilePools(List<BindingPool> bindingPools)
        {
            if ((bindingPools != null) && bindingPools.Count > 0)
            {
                List<Range> ranges = new List<Range>();
                foreach (V6PrefixBindingPool bp in bindingPools)
                {
                    Range range = new Range(bp.GetStartAddress(), bp.GetEndAddress());
                    ranges.Add(range);
                }
                iaMgr.ReconcileIaAddresses(ranges);
            }
        }

        /**
         * Builds a binding pool from an PrefixPool using the given link.
         * 
         * @param pool the PrefixPool to wrap as an PrefixBindingPool
         * @param link the link
         * 
         * @return the binding pool
         * 
         * @throws DhcpServerConfigException if there is a problem parsing the configured range
         */
        protected V6PrefixBindingPool BuildV6BindingPool(v6PrefixPool pool, link link)
        {
            return BuildV6BindingPool(pool, link, null);
        }

        /**
         * Builds a binding pool from an PrefixPool using the given link and filter.
         * 
         * @param pool the AddressPool to wrap as an PrefixBindingPool
         * @param link the link
         * @param linkFilter the link filter
         * 
         * @return the binding pool
         * 
         * @throws DhcpServerConfigException if there is a problem parsing the configured range
         */
        protected V6PrefixBindingPool BuildV6BindingPool(v6PrefixPool pool, link link,
                linkFilter linkFilter)
        {
            V6PrefixBindingPool bp = new V6PrefixBindingPool(pool);
            long pLifetime =
                DhcpServerPolicies.EffectivePolicyAsLong(bp, link, Property.PREFERRED_LIFETIME);
            bp.SetPreferredLifetime(pLifetime);
            long vLifetime =
                DhcpServerPolicies.EffectivePolicyAsLong(bp, link, Property.VALID_LIFETIME);
            bp.SetValidLifetime(vLifetime);
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
            log.Info("Built prefix binding pool: " + bp.GetStartAddress().ToString() + "-" +
                            bp.GetEndAddress().ToString() + ", size=" + bp.GetSize());
            return bp;
        }

        protected override List<StaticBinding> BuildStaticBindings(link link)
        {
            List<StaticBinding> staticBindings = new List<StaticBinding>();
            List<v6PrefixBinding> bindings = link.v6PrefixBindings;
            if ((bindings != null) && bindings.Count > 0)
            {
                foreach (v6PrefixBinding binding in bindings)
                {
                    V6StaticPrefixBinding spb = BuildStaticBinding(binding, link);
                    staticBindings.Add(spb);
                }
            }
            return staticBindings;
        }

        protected V6StaticPrefixBinding BuildStaticBinding(v6PrefixBinding binding, link link)
        {
            try
            {
                IPAddress inetAddr = IPAddress.Parse(binding.prefix);
                V6StaticPrefixBinding sb = new V6StaticPrefixBinding(binding);
                SetIpAsUsed(link, inetAddr);
                return sb;
            }
            catch (Exception ex)
            {
                log.Error("Invalid static binding address");
                throw new Exception("Invalid static binding address", ex);
            }
        }

        public Binding FindCurrentBinding(DhcpLink clientLink, DhcpV6ClientIdOption clientIdOption,
                DhcpV6IaPdOption iaPdOption, DhcpMessage requestMsg)
        {
            byte[] duid = clientIdOption.GetDuid();
            long iaid = iaPdOption.GetIaId();

            return base.FindCurrentBinding(clientLink, duid, IdentityAssoc.PD_TYPE,
                    iaid, requestMsg);

        }

        public Binding CreateSolicitBinding(DhcpLink clientLink, DhcpV6ClientIdOption clientIdOption,
                DhcpV6IaPdOption iaPdOption, DhcpMessage requestMsg, byte state, IPAddress clientV4IPAddress)
        {
            byte[] duid = clientIdOption.GetDuid();
            long iaid = iaPdOption.GetIaId();

            StaticBinding staticBinding =
                FindStaticBinding(clientLink.GetLink(), duid, IdentityAssoc.PD_TYPE, iaid, requestMsg);

            if (staticBinding != null)
            {
                return base.CreateStaticBinding(clientLink, duid, IdentityAssoc.PD_TYPE,
                        iaid, staticBinding, requestMsg);
            }
            else
            {
                return base.CreateBinding(clientLink, duid, IdentityAssoc.PD_TYPE,
                        iaid, GetInetAddrs(iaPdOption), requestMsg, state, clientV4IPAddress);
            }
        }

        public Binding UpdateBinding(Binding binding, DhcpLink clientLink,
                DhcpV6ClientIdOption clientIdOption, DhcpV6IaPdOption iaPdOption,
                DhcpMessage requestMsg, byte state, IPAddress clientV4IPAddress)
        {
            byte[] duid = clientIdOption.GetDuid();
            long iaid = iaPdOption.GetIaId();

            StaticBinding staticBinding =
                FindStaticBinding(clientLink.GetLink(), duid, IdentityAssoc.PD_TYPE, iaid, requestMsg);

            if (staticBinding != null)
            {
                return base.UpdateStaticBinding(binding, clientLink, duid, IdentityAssoc.PD_TYPE,
                        iaid, staticBinding, requestMsg);
            }
            else
            {
                return base.UpdateBinding(binding, clientLink, duid, IdentityAssoc.PD_TYPE,
                        iaid, GetInetAddrs(iaPdOption), requestMsg, state, clientV4IPAddress);
            }
        }

        public void ReleaseIaPrefix(IaPrefix iaPrefix)
        {
            try
            {
                if (DhcpServerPolicies.GlobalPolicyAsBoolean(
                        Property.BINDING_MANAGER_DELETE_OLD_BINDINGS))
                {
                    iaMgr.DeleteIaPrefix(iaPrefix);
                    // free the prefix only if it is deleted from the db,
                    // otherwise, we will get a unique constraint violation
                    // if another client obtains this released prefix
                    FreeAddress(iaPrefix.GetIpAddress());
                }
                else
                {
                    iaPrefix.SetStartTime(DateTime.Now);
                    iaPrefix.SetPreferredEndTime(DateTime.Now);
                    iaPrefix.SetValidEndTime(DateTime.Now);
                    iaPrefix.SetState(IaPrefix.RELEASED);
                    iaMgr.UpdateIaPrefix(iaPrefix);
                }
            }
            catch (Exception ex)
            {
                log.Error("Failed to release address");
            }
        }

        public void DeclineIaPrefix(IaPrefix iaPrefix)
        {
            try
            {
                iaPrefix.SetStartTime(DateTime.Now);
                iaPrefix.SetPreferredEndTime(DateTime.Now);
                iaPrefix.SetValidEndTime(DateTime.Now);
                iaPrefix.SetState(IaPrefix.DECLINED);
                iaMgr.UpdateIaPrefix(iaPrefix);
            }
            catch (Exception ex)
            {
                log.Error("Failed to decline address");
            }
        }

        /**
         * Callback from the ExpireTimerTask started when the lease was granted.
         * NOT CURRENTLY USED
         * 
         * @param iaPrefix the ia prefix
         */
        public void ExpireIaPrefix(IaPrefix iaPrefix)
        {
            try
            {
                if (DhcpServerPolicies.GlobalPolicyAsBoolean(
                        Property.BINDING_MANAGER_DELETE_OLD_BINDINGS))
                {
                    log.Debug("Deleting expired prefix: " + iaPrefix.GetIpAddress());
                    iaMgr.DeleteIaPrefix(iaPrefix);
                    // free the prefix only if it is deleted from the db,
                    // otherwise, we will get a unique constraint violation
                    // if another client obtains this released prefix
                    FreeAddress(iaPrefix.GetIpAddress());
                }
                else
                {
                    iaPrefix.SetStartTime(DateTime.Now);
                    iaPrefix.SetPreferredEndTime(DateTime.Now);
                    iaPrefix.SetValidEndTime(DateTime.Now);
                    iaPrefix.SetState(IaPrefix.EXPIRED);
                    log.Debug("Updating expired prefix: " + iaPrefix.GetIpAddress());
                    iaMgr.UpdateIaPrefix(iaPrefix);
                }
            }
            catch (Exception ex)
            {
                log.Error("Failed to expire address");
            }
        }

        /**
         * Callback from the RepearTimerTask started when the BindingManager initialized.
         * Find any expired prefixes as of now, and expire them already.
         */
        public void ExpirePrefixes()
        {
            List<IaPrefix> expiredPrefs = iaMgr.FindExpiredIaPrefixes();
            if ((expiredPrefs != null) && expiredPrefs.Count > 0)
            {
                foreach (IaPrefix iaPrefix in expiredPrefs)
                {
                    ExpireIaPrefix(iaPrefix);
                }
            }
        }

        /**
         * Extract the list of IP addresses from within the given IA_PD option.
         * 
         * @param iaNaOption the IA_PD option
         * 
         * @return the list of InetAddresses for the IPs in the IA_PD option
         */
        private List<IPAddress> GetInetAddrs(DhcpV6IaPdOption iaPdOption)
        {
            List<IPAddress> inetAddrs = null;
            List<DhcpV6IaPrefixOption> iaPrefs = iaPdOption.GetIaPrefixOptions();
            if ((iaPrefs != null) && iaPrefs.Count > 0)
            {
                inetAddrs = new List<IPAddress>();
                foreach (DhcpV6IaPrefixOption iaPrefix in iaPrefs)
                {
                    IPAddress inetAddr = iaPrefix.GetInetAddress();
                    inetAddrs.Add(inetAddr);
                }
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
        protected override Binding BuildBindingFromIa(IdentityAssoc ia,
                DhcpLink clientLink, DhcpMessage requestMsg)
        {
            Binding binding = new Binding(ia, clientLink);
            List<IaAddress> iaPrefs = ia.GetIaAddresses();
            if ((iaPrefs != null) && iaPrefs.Count > 0)
            {
                List<IaAddress> bindingPrefixes = new List<IaAddress>();
                foreach (IaAddress iaAddr in iaPrefs)
                {
                    // off-link check needed only for v4?
                    //				if (!clientLink.getSubnet().contains(iaAddr.getIpAddress())) {
                    //					log.info("Ignoring off-link binding address: " + 
                    //							iaAddr.getIpAddress().getHostAddress());
                    //					continue;
                    //				}
                    V6BindingPrefix bindingPrefix = null;
                    StaticBinding staticBinding = FindStaticBinding(clientLink.GetLink(), ia.GetDuid(), ia.GetIatype(), ia.GetIaid(), requestMsg);
                    if (staticBinding != null)
                    {
                        bindingPrefix =
                            BuildV6BindingPrefixFromIaPrefix((IaPrefix)iaAddr, staticBinding);
                    }
                    else
                    {
                        bindingPrefix =
                            BuildBindingAddrFromIaPrefix((IaPrefix)iaAddr, clientLink.GetLink(), requestMsg);
                    }
                    if (bindingPrefix != null)
                        bindingPrefixes.Add(bindingPrefix);
                }
                // replace the collection of IaPrefixes with BindingPrefixes
                binding.SetIaAddresses(bindingPrefixes);
            }
            else
            {
                log.Warn("IA has no prefixes, binding is empty.");
            }
            return binding;
        }

        /**
         * Create a BindingPrefix given an IaPrefix loaded from the database.
         * 
         * @param iaPrefix the ia prefix
         * @param clientLink the client link
         * @param requestMsg the request msg
         * 
         * @return the binding address
         */
        private V6BindingPrefix BuildBindingAddrFromIaPrefix(IaPrefix iaPrefix,
                link clientLink, DhcpMessage requestMsg)
        {
            IPAddress inetAddr = iaPrefix.GetIpAddress();
            BindingPool bp = FindBindingPool(clientLink, inetAddr, requestMsg);
            if (bp != null)
            {
                // TODO store the configured options in the persisted binding?
                // ipAddr.setDhcpOptions(bp.getDhcpOptions());
                return new V6BindingPrefix(iaPrefix, (V6PrefixBindingPool)bp);
            }
            else
            {
                log.Error("Failed to create BindingPrefix: No BindingPool found for IP=" +
                        inetAddr.ToString());
            }
            // MUST have a BindingPool, otherwise something's broke
            return null;
        }

        /**
         * Build a BindingPrefix given an IaAddress loaded from the database
         * and a static binding for the client request.
         * 
         * @param iaPrefix
         * @param staticBinding
         * @return
         */
        private V6BindingPrefix BuildV6BindingPrefixFromIaPrefix(IaPrefix iaPrefix,
                StaticBinding staticBinding)
        {
            V6BindingPrefix bindingPrefix = new V6BindingPrefix(iaPrefix, staticBinding);
            return bindingPrefix;
        }

        /**
         * Build a BindingPrefix for the given InetAddress and Link.
         * 
         * @param inetAddr the inet addr
         * @param clientLink the client link
         * @param requestMsg the request msg
         * 
         * @return the binding address
         */
        protected override BindingObject BuildBindingObject(IPAddress inetAddr,
                DhcpLink clientLink, DhcpMessage requestMsg)
        {
            V6PrefixBindingPool bp =
                (V6PrefixBindingPool)FindBindingPool(clientLink.GetLink(), inetAddr, requestMsg);
            if (bp != null)
            {
                bp.SetUsed(inetAddr);   // TODO check if this is necessary
                IaPrefix iaPrefix = new IaPrefix();
                iaPrefix.SetIpAddress(inetAddr);
                iaPrefix.SetPrefixLength((short)bp.GetAllocPrefixLen());
                V6BindingPrefix bindingPrefix = new V6BindingPrefix(iaPrefix, bp);
                SetBindingObjectTimes(bindingPrefix,
                        bp.GetPreferredLifetimeMs(), bp.GetPreferredLifetimeMs());
                // TODO store the configured options in the persisted binding?
                // bindingPrefix.setDhcpOptions(bp.getDhcpOptions());
                return bindingPrefix;
            }
            else
            {
                log.Error("Failed to create BindingPrefix: No BindingPool found for IP=" +
                        inetAddr.ToString());
            }
            // MUST have a BindingPool, otherwise something's broke
            return null;
        }
    }
}
