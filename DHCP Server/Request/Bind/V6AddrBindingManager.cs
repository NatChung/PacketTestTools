using PIXIS.DHCP.Config;
using PIXIS.DHCP.DB;
using PIXIS.DHCP.Message;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Option.V6;
using PIXIS.DHCP.Request.Dns;
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
    public abstract class V6AddrBindingManager : BaseAddrBindingManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public V6AddrBindingManager() : base()
        {
        }


        /**
         * Fetch the AddressBindings from the given XML Link object.
         * 
         * @param link
         * @return
         */
        protected abstract List<v6AddressBinding> GetV6AddressBindings(link link);

        /**
         * Build the list of static bindings for the given link.
         * 
         * @param link the link
         * @return the list of static bindings
         * @throws DhcpServerConfigException if the static binding is invalid
         */
        protected override List<StaticBinding> BuildStaticBindings(link link)
        {
            List<StaticBinding> staticBindings = new List<StaticBinding>();
            List<v6AddressBinding> bindings = GetV6AddressBindings(link);
            
            if ((bindings != null) && bindings.Count > 0)
            {
                foreach (v6AddressBinding binding in bindings)
                {
                    V6StaticAddressBinding sab = BuildV6StaticBinding(binding, link);
                    staticBindings.Add(sab);
                }

            }
            return staticBindings;
        }

        /**
         * Build a static address binding from the given address binding.
         * 
         * @param binding the address binding
         * @param link the link
         * @return the static address binding
         * @throws DhcpServerConfigException if the static binding is invalid
         */
        protected V6StaticAddressBinding BuildV6StaticBinding(v6AddressBinding binding, link link)
        {
            try
            {
                IPAddress inetAddr = IPAddress.Parse(binding.ipAddress);
                V6StaticAddressBinding sb = new V6StaticAddressBinding(binding, GetIaType());
                SetIpAsUsed(link, inetAddr);
                return sb;
            }
            catch (Exception ex)
            {
                log.Error("Invalid static binding address");
                throw new Exception("Invalid static binding address", ex);
            }
        }

        /**
	     * Build a BindingAddress for the given InetAddress and DhcpLink.
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
            V6AddressBindingPool bp =
                (V6AddressBindingPool)FindBindingPool(clientLink.GetLink(), inetAddr, requestMsg);
            if (bp != null)
            {
                bp.SetUsed(inetAddr);   // TODO check if this is necessary
                IaAddress iaAddr = new IaAddress();
                iaAddr.SetIpAddress(inetAddr);
                V6BindingAddress bindingAddr = new V6BindingAddress(iaAddr, bp);
                SetBindingObjectTimes(bindingAddr,
                        bp.GetPreferredLifetimeMs(), bp.GetPreferredLifetimeMs());
                // TODO store the configured options in the persisted binding?
                // bindingAddr.setDhcpOptions(bp.getDhcpOptions());
                return bindingAddr;
            }
            else
            {
                log.Error("Failed to create BindingAddress: No BindingPool found for IP=" +
                        inetAddr.ToString());
            }
            // MUST have a BindingPool, otherwise something's broke
            return null;
        }

        /**
         * Build the list of V6AddressBindingPools from the list of configured V6AddressPools
         * for the given configuration Link container object. The list of V6AddressBindingPools
         * starts with the filtered AddressPools followed by non-filtered AddressPools.
         * 
         * @param link the configuration Link object
         * 
         * @return the list of V6AddressBindingPools (<? extends BindingPool>)
         * 
         * @throws DhcpServerConfigException if there is a problem parsing a configured range
         */
        protected override List<BindingPool> BuildBindingPools(link link)
        {
            Debug.Assert(CheckIPIsUsed != null, "V6AddrBindingManager --BuildBindingPools-- CheckIPIsUsed = null");
            List<BindingPool> bindingPools = new List<BindingPool>();
            // Put the filtered pools first in the list of pools on this link
            List<linkFilter> linkFilters = link.linkFilters;
            if ((linkFilters != null) && linkFilters.Count > 0)
            {
                foreach (linkFilter linkFilter in linkFilters)
                {
                    List<v6AddressPool> pools = GetV6AddressPools(linkFilter);
                    if ((pools != null) && pools.Count > 0)
                    {
                        foreach (v6AddressPool pool in pools)
                        {
                            V6AddressBindingPool abp = BuildV6BindingPool(pool, link, linkFilter);
                            abp.CheckIPIsUsed = CheckIPIsUsed;
                            bindingPools.Add(abp);
                        }
                    }
                    //else
                    //{
                    //    log.Error("PoolList is null for PoolsType: " + linkFilter);
                    //}
                }
            }

            List<v6AddressPool> unpools = GetV6AddressPools(link);
            // add the unfiltered pools to the mapped list
            if ((unpools != null) && unpools.Count > 0)
            {
                foreach (v6AddressPool pool in unpools)
                {
                    V6AddressBindingPool abp = BuildV6BindingPool(pool, link);
                    bindingPools.Add(abp);
                }
            }
            //else
            //{
            //    log.Error("PoolList is null for PoolsType: " + poolsType);
            //}

            //TODO this is very dangerous if the server is managing
            //	    both NA and TA address pools because we'd delete
            //	    all the addresses in pools of the other type
            //		reconcilePools(bindingPools);

            return bindingPools;
        }

        /**
         * Builds a binding pool from an AddressPool using the given link.
         * 
         * @param pool the AddressPool to wrap as an AddressBindingPool
         * @param link the link
         * 
         * @return the binding pool
         * 
         * @throws DhcpServerConfigException if there is a problem parsing the configured range
         */
        private V6AddressBindingPool BuildV6BindingPool(v6AddressPool pool, link link, linkFilter linkFilter)
        {
            Debug.Assert(CheckIPIsUsed != null, "V6AddrBindingManager --BuildV6BindingPool-- CheckIPIsUsed = null");
            V6AddressBindingPool bp = new V6AddressBindingPool(pool);
            long pLifetime = long.Parse(Property.PREFERRED_LIFETIME.Value());
            // DhcpServerPolicies.EffectivePolicyAsLong(bp, link, Property.PREFERRED_LIFETIME);
            bp.SetPreferredLifetime(pLifetime);
            long vLifetime = long.Parse(Property.VALID_LIFETIME.Value());
            //  DhcpServerPolicies.EffectivePolicyAsLong(bp, link, Property.VALID_LIFETIME);
            bp.SetValidLifetime(vLifetime);
            bp.SetLinkFilter(linkFilter);
            bp.CheckIPIsUsed = CheckIPIsUsed;
            List<IPAddress> usedIps = null;// iaMgr.findExistingIPs(bp.getStartAddress(), bp.getEndAddress());
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
            log.Info("Built address binding pool: " + bp.GetStartAddress().ToString() + "-" +
                    bp.GetEndAddress().ToString());
            return bp;
        }

        /**
         * Builds a binding pool from an AddressPool using the given link.
         * 
         * @param pool the AddressPool to wrap as an AddressBindingPool
         * @param link the link
         * 
         * @return the binding pool
         * 
         * @throws DhcpServerConfigException if there is a problem parsing the configured range
         */
        protected V6AddressBindingPool BuildV6BindingPool(v6AddressPool pool, link link)
        {
            return BuildV6BindingPool(pool, link, null);
        }

        /**
	 * Fetch the V6AddressPoolsType XML object from the given LinkFilter XML object.
	 * The subclasses fetch the address pools for either NA or TA addresses.
	 * 
	 * @param linkFilter the link filter
	 * @return the V6AddressPoolsType for this link filter, or null if none
	 */
        protected abstract List<v6AddressPool> GetV6AddressPools(linkFilter linkFilter);
        protected abstract List<v6AddressPool> GetV6AddressPools(link link);

        /**
         * Perform the DDNS delete processing when a lease is released or expired.
         * 
         * @param ia the IdentityAssoc of the client
         * @param iaAddr the released or expired IaAddress 
         */
        protected override void DdnsDelete(IdentityAssoc ia, IaAddress iaAddr)
        {
            DhcpV6ClientFqdnOption clientFqdnOption = null;
            try
            {
                if ((ia != null) && (iaAddr != null))
                {
                    List<DhcpOption> opts = iaAddr.GetDhcpOptions();
                    if (opts != null)
                    {
                        foreach (DhcpOption opt in opts)
                        {
                            if (opt.GetCode() == DhcpConstants.V6OPTION_CLIENT_FQDN)
                            {
                                clientFqdnOption = new DhcpV6ClientFqdnOption();
                                //clientFqdnOption.Decode(ByteBuffer.Wrap(opt.GetValue()));
                                break;
                            }
                        }
                    }
                    if (clientFqdnOption != null)
                    {
                        string fqdn = clientFqdnOption.GetDomainName();
                        if (!String.IsNullOrEmpty(fqdn))
                        {
                            DhcpLink link = serverConfig.FindLinkForAddress(iaAddr.GetIpAddress());
                            if (link != null)
                            {
                                V6BindingAddress bindingAddr = null;
                                StaticBinding staticBinding =
                                    FindStaticBinding(link.GetLink(), ia.GetDuid(),
                                            ia.GetIatype(), ia.GetIaid(), null);
                                if (staticBinding != null)
                                {
                                    bindingAddr =
                                        BuildV6StaticBindingFromIaAddr(iaAddr, staticBinding);
                                }
                                else
                                {
                                    bindingAddr =
                                        BuildV6BindingAddressFromIaAddr(iaAddr, link, null);    // safe to send null requestMsg
                                }
                                if (bindingAddr != null)
                                {

                                    DdnsCallback ddnsComplete =
                                        new DhcpV6DdnsComplete(bindingAddr, clientFqdnOption);

                                    DhcpConfigObject configObj = bindingAddr.GetConfigObj();

                                    DdnsUpdater ddns =
                                        new DdnsUpdater(link.GetLink(), configObj,
                                                bindingAddr.GetIpAddress(), fqdn, ia.GetDuid(),
                                                configObj.GetValidLifetime(),
                                                clientFqdnOption.GetUpdateAaaaBit(), true,
                                                ddnsComplete);

                                    ddns.ProcessUpdates();
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

        /**
	     * Create a Binding given an IdentityAssoc loaded from the database.
	     * 
	     * @param ia the ia
	     * @param clientLink the client link
	     * @param requestMsg the request msg
	     * 
	     * @return the binding
	     */
        protected override Binding BuildBindingFromIa(IdentityAssoc ia, DhcpLink clientLink, DhcpMessage requestMsg)
        {
            Binding binding = new Binding(ia, clientLink);
            List<IaAddress> iaAddrs = ia.GetIaAddresses();
            if ((iaAddrs != null) && iaAddrs.Count > 0)
            {
                List<IaAddress> bindingAddrs = new List<IaAddress>();
                foreach (IaAddress iaAddr in iaAddrs)
                {
                    // off-link check needed only for v4?
                    //				if (!clientLink.getSubnet().contains(iaAddr.getIpAddress())) {
                    //					log.info("Ignoring off-link binding address: " + 
                    //							iaAddr.getIpAddress().getHostAddress());
                    //					continue;
                    //				}
                    V6BindingAddress bindingAddr = null;
                    StaticBinding staticBinding =
                        FindStaticBinding(clientLink.GetLink(), ia.GetDuid(),
                                ia.GetIatype(), ia.GetIaid(), requestMsg);
                    if (staticBinding != null)
                    {
                        bindingAddr =
                            BuildV6StaticBindingFromIaAddr(iaAddr, staticBinding);
                    }
                    else
                    {
                        bindingAddr =
                            BuildV6BindingAddressFromIaAddr(iaAddr, clientLink, requestMsg);
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
         * Create a V6BindingAddress given an IaAddress loaded from the database.
         * 
         * @param iaAddr the ia addr
         * @param clientLink the client link
         * @param requestMsg the request msg
         * 
         * @return the binding address
         */
        private V6BindingAddress BuildV6BindingAddressFromIaAddr(IaAddress iaAddr,
                DhcpLink clientLink, DhcpMessage requestMsg)
        {
            IPAddress inetAddr = iaAddr.GetIpAddress();
            BindingPool bp = FindBindingPool(clientLink.GetLink(), inetAddr, requestMsg);
            if (bp != null)
            {
                // TODO store the configured options in the persisted binding?
                // ipAddr.setDhcpOptions(bp.getDhcpOptions());
                return new V6BindingAddress(iaAddr, (V6AddressBindingPool)bp);
            }
            else
            {
                log.Error("Failed to create BindingAddress: No BindingPool found for IP=" +
                        inetAddr.ToString());
            }
            // MUST have a BindingPool, otherwise something's broke
            return null;
        }

        /**
         * Build a V6BindingAddress given an IaAddress loaded from the database
         * and a static binding for the client request.
         * 
         * @param iaAddr
         * @param staticBinding
         * @return
         */
        private V6BindingAddress BuildV6StaticBindingFromIaAddr(IaAddress iaAddr,
                StaticBinding staticBinding)
        {
            V6BindingAddress bindingAddr = new V6BindingAddress(iaAddr, staticBinding);
            return bindingAddr;
        }

    }
}
