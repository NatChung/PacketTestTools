using PIXIS.DHCP.Option.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PIXIS.DHCP.Request.Bind;
using PIXIS.DHCP.Config;
using static PIXIS.DHCP.Config.DhcpServerPolicies;
using PIXIS.DHCP.Option;

namespace PIXIS.DHCP.DB
{
    public class LeaseManager : IaManager
    {
        /** The log. */
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<DhcpLease> _DhcpLeaseData;

        public LeaseManager()
        {
            _DhcpLeaseData = new List<DhcpLease>();
        }
        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.db.IaManager#createIA(com.jagornet.dhcpv6.db.IdentityAssoc)
         */
        public void CreateIA(IdentityAssoc ia)
        {
            if (ia != null)
            {
                List<DhcpLease> leases = ToDhcpLeases(ia);
                if ((leases != null) && leases.Count > 0)
                {
                    foreach (DhcpLease lease in leases)
                    {
                        InsertDhcpLease(lease);
                    }
                }
            }
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.db.IaManager#updateIA(com.jagornet.dhcpv6.db.IdentityAssoc, java.util.List, java.util.List, java.util.List)
         */
        public void UpdateIA(IdentityAssoc ia, List<IaAddress> addAddrs, List<IaAddress> updateAddrs, List<IaAddress> delAddrs)
        {
            if ((addAddrs != null) && addAddrs.Count > 0)
            {
                foreach (IaAddress addAddr in addAddrs)
                {
                    DhcpLease lease = ToDhcpLease(ia, addAddr);
                    InsertDhcpLease(lease);
                }
            }
            if ((updateAddrs != null) && updateAddrs.Count > 0)
            {
                foreach (IaAddress updateAddr in updateAddrs)
                {
                    DhcpLease lease = ToDhcpLease(ia, updateAddr);
                    UpdateDhcpLease(lease);
                }
            }
            if ((delAddrs != null) && delAddrs.Count > 0)
            {
                foreach (IaAddress delAddr in delAddrs)
                {
                    DhcpLease lease = ToDhcpLease(ia, delAddr);
                    DeleteDhcpLease(lease);
                }
            }
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.db.IaManager#deleteIA(com.jagornet.dhcpv6.db.IdentityAssoc)
         */
        public void DeleteIA(IdentityAssoc ia)
        {
            if (ia != null)
            {
                List<DhcpLease> leases = ToDhcpLeases(ia);
                if ((leases != null) && leases.Count > 0)
                {
                    foreach (DhcpLease lease in leases)
                    {
                        DeleteDhcpLease(lease);
                    }
                }
            }
        }

        public List<IdentityAssoc> FindExpiredIAs(byte iatype)
        {
            List<DhcpLease> leases = FindExpiredLeases(iatype);
            return ToIdentityAssocs(leases);
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.db.IaManager#findExpiredIaAddresses(byte)
         */
        public List<IaAddress> FindExpiredIaAddresses(byte iatype)
        {
            List<DhcpLease> leases = FindExpiredLeases(iatype);
            return ToIaAddresses(leases);
        }

        public void SaveDhcpOption(IaAddress iaAddr, BaseDhcpOption baseOption)
        {
            try
            {
                //byte[] newVal = baseOption.encode().array();
                // don't store the option code, start with length to
                // simplify decoding when retrieving from database
                //if (baseOption.IsV4())
                //{
                //    newVal = Arrays.copyOfRange(newVal, 1, newVal.length);
                //}
                //else
                //{
                //    newVal = Arrays.copyOfRange(newVal, 2, newVal.length);
                //}
                ////			DhcpOption dbOption = iaAddr.getDhcpOption(baseOption.getCode());
                //DhcpOption dbOption = FindIaAddressOption(iaAddr, baseOption);
                //if (dbOption == null)
                //{
                //    dbOption = new DhcpOption();
                //    dbOption.SetCode(baseOption.GetCode());
                //    dbOption.SetValue(newVal);
                //    SetDhcpOption(iaAddr, dbOption);
                //}
                //else
                //{
                //    if (!Arrays.equals(dbOption.getValue(), newVal))
                //    {
                //        dbOption.setValue(newVal);
                //        setDhcpOption(iaAddr, dbOption);
                //    }
                //}
            }
            catch (Exception ex)
            {
                log.Error("Failed to update binding with option");
            }

        }

        public void DeleteDhcpOption(IaAddress iaAddr, BaseDhcpOption baseOption)
        {
            DhcpLease lease = FindDhcpLeaseForInetAddr(iaAddr.GetIpAddress());
            if (lease != null)
            {
                List<DhcpOption> iaAddrOptions = lease.GetIaAddrDhcpOptions();
                if (iaAddrOptions != null)
                {
                    bool deleted = false;
                    foreach (DhcpOption iaAddrOption in iaAddrOptions)
                    {
                        if (iaAddrOption.GetCode() == baseOption.GetCode())
                        {
                            iaAddrOptions.Remove(iaAddrOption);
                            deleted = true;
                            break;
                        }
                    }
                    if (deleted)
                    {
                        UpdateIpAddrOptions(iaAddr.GetIpAddress(), iaAddrOptions);
                    }
                }
            }
        }

        protected DhcpOption FindIaAddressOption(IaAddress iaAddr, BaseDhcpOption baseOption)
        {
            DhcpOption dbOption = null;
            DhcpLease lease = FindDhcpLeaseForInetAddr(iaAddr.GetIpAddress());
            if (lease != null)
            {
                List<DhcpOption> iaAddrOptions = lease.GetIaAddrDhcpOptions();
                if (iaAddrOptions != null)
                {
                    foreach (DhcpOption iaAddrOption in iaAddrOptions)
                    {
                        if (iaAddrOption.GetCode() == baseOption.GetCode())
                        {
                            dbOption = iaAddrOption;
                            break;
                        }
                    }
                }
            }
            return dbOption;
        }

        protected void SetDhcpOption(IaAddress iaAddr, DhcpOption option)
        {
            iaAddr.SetDhcpOption(option);
            UpdateIpAddrOptions(iaAddr.GetIpAddress(), iaAddr.GetDhcpOptions());
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.db.IaManager#findIA(byte[], byte, long)
         */
        public IdentityAssoc FindIA(byte[] duid, byte iatype, long iaid)
        {
            List<DhcpLease> leases = FindDhcpLeasesForIA(duid, iatype, iaid);
            return ToIdentityAssoc(leases);
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.db.IaManager#findIA(com.jagornet.dhcpv6.db.IaAddress)
         */
        public IdentityAssoc FindIA(IaAddress iaAddress)
        {
            return FindIA(iaAddress.GetIpAddress(), true);
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.db.IaManager#findIA(java.net.IPAddress)
         */
        public IdentityAssoc FindIA(IPAddress inetAddr)
        {
            return FindIA(inetAddr, true);
        }

        /**
         * Find ia.
         *
         * @param inetAddr the inet addr
         * @param allBindings the all bindings
         * @return the identity assoc
         */
        public IdentityAssoc FindIA(IPAddress inetAddr, bool allBindings)
        {
            IdentityAssoc ia = null;
            DhcpLease lease = FindDhcpLeaseForInetAddr(inetAddr);
            if (lease != null)
            {
                // use a set here, so that if we are getting all bindings, then we don't
                // include the lease found above again in the returned collection
                List<DhcpLease> leases = new List<DhcpLease>();
                leases.Add(lease);
                if (allBindings)
                {
                    leases.AddRange(FindDhcpLeasesForIA(lease.GetDuid(), lease.GetIatype(), lease.GetIaid()));
                    //leases = leases.Concat(FindDhcpLeasesForIA(lease.GetDuid(), lease.GetIatype(), lease.GetIaid())).ToList();
                }
                ia = ToIdentityAssoc(leases);
            }
            return ia;
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.db.IaManager#updateIaPrefix(com.jagornet.dhcpv6.db.IaPrefix)
         */
        public void UpdateIaPrefix(IaPrefix iaPrefix)
        {
            UpdateIaAddr(iaPrefix);
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.db.IaManager#deleteIaPrefix(com.jagornet.dhcpv6.db.IaPrefix)
         */
        public void DeleteIaPrefix(IaPrefix iaPrefix)
        {
            DeleteIaAddr(iaPrefix);
        }

        // Conversion methods

        /**
         * To identity assoc.
         *
         * @param leases the leases
         * @return the identity assoc
         */
        protected IdentityAssoc ToIdentityAssoc(List<DhcpLease> leases)
        {
            IdentityAssoc ia = null;
            if ((leases != null) && leases.Count > 0)
            {
                var enumerator = leases.GetEnumerator();
                bool hasNext = enumerator.MoveNext();

                DhcpLease lease = enumerator.Current;
                ia = new IdentityAssoc();
                ia.SetDuid(lease.GetDuid());
                ia.SetIatype(lease.GetIatype());
                ia.SetIaid(lease.GetIaid());
                ia.SetState(lease.GetState());
                ia.SetDhcpOptions(lease.GetIaDhcpOptions());
                if (lease.GetIatype() == IdentityAssoc.PD_TYPE)
                {
                    List<IaAddress> iaPrefixes = new List<IaAddress>();
                    iaPrefixes.Add(ToIaPrefix(lease));
                    while (enumerator.MoveNext())
                    {
                        //TODO: should confirm that the duid/iatype/iaid/state still match
                        iaPrefixes.Add(ToIaPrefix(lease));
                    }
                    ia.SetIaAddresses(iaPrefixes);
                }
                else
                {
                    List<IaAddress> iaAddrs = new List<IaAddress>();
                    iaAddrs.Add(ToIaAddress(lease));
                    while (enumerator.MoveNext())
                    {
                        //TODO: should confirm that the duid/iatype/iaid/state still match
                        iaAddrs.Add(ToIaAddress(lease));
                    }
                    ia.SetIaAddresses(iaAddrs);
                }
            }
            return ia;
        }

        protected List<IdentityAssoc> ToIdentityAssocs(List<DhcpLease> leases)
        {
            List<IdentityAssoc> ias = null;
            if ((leases != null) && leases.Count > 0)
            {
                ias = new List<IdentityAssoc>();
                // for each lease, create a separate IdentityAssoc
                foreach (DhcpLease lease in leases)
                {
                    List<DhcpLease> _leases = new List<DhcpLease>();
                    _leases.Add(lease);
                    IdentityAssoc ia = ToIdentityAssoc(_leases);
                    ias.Add(ia);
                }
            }
            return ias;
        }

        protected List<IaAddress> ToIaAddresses(List<DhcpLease> leases)
        {
            List<IaAddress> addrs = null;
            if (leases != null)
            {
                addrs = new List<IaAddress>();
                foreach (DhcpLease dhcpLease in leases)
                {
                    addrs.Add(this.ToIaAddress(dhcpLease));
                }
            }
            return addrs;
        }

        /**
         * To ia address.
         *
         * @param lease the lease
         * @return the ia address
         */
        protected IaAddress ToIaAddress(DhcpLease lease)
        {
            IaAddress iaAddr = new IaAddress();
            iaAddr.SetIpAddress(lease.GetIpAddress());
            iaAddr.SetState(lease.GetState());
            iaAddr.SetStartTime(lease.GetStartTime());
            iaAddr.SetPreferredEndTime(lease.GetPreferredEndTime());
            iaAddr.SetValidEndTime(lease.GetValidEndTime());
            iaAddr.SetDhcpOptions(lease.GetIaAddrDhcpOptions());
            return iaAddr;
        }


        protected List<IaPrefix> ToIaPrefixes(List<DhcpLease> leases)
        {
            List<IaPrefix> prefixes = null;
            if (leases != null)
            {
                prefixes = new List<IaPrefix>();
                foreach (DhcpLease dhcpLease in leases)
                {
                    prefixes.Add(ToIaPrefix(dhcpLease));
                }
            }
            return prefixes;
        }

        /**
         * To ia prefix.
         *
         * @param lease the lease
         * @return the ia prefix
         */
        protected IaPrefix ToIaPrefix(DhcpLease lease)
        {
            IaPrefix iaPrefix = new IaPrefix();
            iaPrefix.SetIpAddress(lease.GetIpAddress());
            iaPrefix.SetPrefixLength(lease.GetPrefixLength());
            iaPrefix.SetState(lease.GetState());
            iaPrefix.SetStartTime(lease.GetStartTime());
            iaPrefix.SetPreferredEndTime(lease.GetPreferredEndTime());
            iaPrefix.SetValidEndTime(lease.GetValidEndTime());
            iaPrefix.SetDhcpOptions(lease.GetIaAddrDhcpOptions());
            return iaPrefix;
        }

        /**
         * To dhcp leases.
         *
         * @param ia the ia
         * @return the list
         */
        protected List<DhcpLease> ToDhcpLeases(IdentityAssoc ia)
        {
            if (ia != null)
            {
                List<IaAddress> iaAddrs = ia.GetIaAddresses();
                if ((iaAddrs != null) && iaAddrs.Count > 0)
                {
                    List<DhcpLease> leases = new List<DhcpLease>();
                    foreach (IaAddress iaAddr in iaAddrs)
                    {
                        DhcpLease lease = ToDhcpLease(ia, iaAddr);
                        leases.Add(lease);
                    }
                    return leases;
                }
                else
                {
                    log.Warn("No addresses in lease");
                }
            }
            return null;
        }

        /**
         * To dhcp lease.
         *
         * @param ia the ia
         * @param iaAddr the ia addr
         * @return the dhcp lease
         */
        protected DhcpLease ToDhcpLease(IdentityAssoc ia, IaAddress iaAddr)
        {
            DhcpLease lease = new DhcpLease();
            lease.SetDuid(ia.GetDuid());
            lease.SetIaid(ia.GetIaid());
            lease.SetIatype(ia.GetIatype());
            lease.SetIpAddress(iaAddr.GetIpAddress());
            if (iaAddr is IaPrefix)
            {
                lease.SetPrefixLength(((IaPrefix)iaAddr).GetPrefixLength());
            }
            lease.SetState(iaAddr.GetState());
            lease.SetStartTime(iaAddr.GetStartTime());
            lease.SetPreferredEndTime(iaAddr.GetPreferredEndTime());
            lease.SetValidEndTime(iaAddr.GetValidEndTime());
            lease.SetIaAddrDhcpOptions(iaAddr.GetDhcpOptions());
            lease.SetIaDhcpOptions(ia.GetDhcpOptions());
            return lease;
        }

        public void Init()
        {
            // throw new NotImplementedException();
        }

        public void UpdateIaAddr(IaAddress iaAddr)
        {
            _DhcpLeaseData.Where(d => d.GetIpAddress() == iaAddr.GetIpAddress()).ToList().ForEach(d =>
            {
                d.SetStartTime(iaAddr.GetStartTime());
                d.SetPreferredEndTime(iaAddr.GetPreferredEndTime());
                d.SetValidEndTime(iaAddr.GetValidEndTime());
            });



            //(_DhcpLeaseData.Where(d => d.GetIpAddress().ToString() == iaAddr.GetIpAddress().ToString()).First());
        }

        public void DeleteIaAddr(IaAddress iaAddr)
        {
            _DhcpLeaseData.RemoveAll(d => d.GetIpAddress().ToString() == iaAddr.GetIpAddress().ToString());
        }

        public List<IPAddress> FindExistingIPs(IPAddress startAddr, IPAddress endAddr)
        {
            return _DhcpLeaseData.Where(d =>
                                         d.GetIpAddress().GetLong() >= startAddr.GetLong() &&
                                         d.GetIpAddress().GetLong() <= endAddr.GetLong())
                                        .Select(d => d.GetIpAddress()).ToList();

        }

        public List<IaAddress> FindUnusedIaAddresses(IPAddress startAddr, IPAddress endAddr)
        {
            long offerExpireMillis =
             DhcpServerPolicies.GlobalPolicyAsLong(Property.BINDING_MANAGER_OFFER_EXPIRATION);
            long offerExpiration = OpaqueDataUtil.GetCurrentMilli() - offerExpireMillis;

            List<DhcpLease> leases = _DhcpLeaseData.Where(d =>
                            ((d.GetState() == IaAddress.ADVERTISED &&
                            d.GetStartTime() <= new DateTime(offerExpiration)) |
                            (d.GetState() == IaAddress.EXPIRED |
                            d.GetState() == IaAddress.RELEASED)) &&
                            d.GetIpAddress().GetLong() >= startAddr.GetLong() &&
                            d.GetIpAddress().GetLong() <= endAddr.GetLong()).
                            OrderBy(s => s.GetState()).
                            ThenBy(t => t.GetValidEndTime()).
                            //ThenBy(d => d.GetIpAddress())
                            ToList();
            return ToIaAddresses(leases);
        }

        public List<IaPrefix> FindUnusedIaPrefixes(IPAddress startAddr, IPAddress endAddr)
        {
            long offerExpiration = OpaqueDataUtil.GetCurrentMilli() - 12000;  // 2 min = 120 sec = 12000 ms

            List<DhcpLease> leases = _DhcpLeaseData.Where(d =>
                             ((d.GetState() == IaPrefix.ADVERTISED &&
                             d.GetStartTime() <= new DateTime(offerExpiration)) |
                             (d.GetState() == IaPrefix.EXPIRED |
                             d.GetState() == IaPrefix.RELEASED)) &&
                             d.GetIpAddress().GetLong() >= startAddr.GetLong() &&
                             d.GetIpAddress().GetLong() <= endAddr.GetLong()).
                             OrderBy(s => s.GetState()).
                             ThenBy(t => t.GetValidEndTime()).
                             //ThenBy(d => d.GetIpAddress())
                             ToList();
            return ToIaPrefixes(leases);
        }

        public void ReconcileIaAddresses(List<Range> ranges)
        {
            List<byte[]> args = new List<byte[]>();
            StringBuilder query = new StringBuilder();
            var rangeIter = ranges.GetEnumerator();
            while (rangeIter.MoveNext())
            {
                Range range = rangeIter.Current;
                args.Add(range.GetStartAddress().GetAddressBytes());
                args.Add(range.GetEndAddress().GetAddressBytes());
            }
        }

        public void DeleteAllIAs()
        {
            _DhcpLeaseData.Clear();
        }

        protected List<DhcpLease> FindDhcpLeasesForIA(byte[] duid, byte iatype, long iaid)
        {
            return _DhcpLeaseData.Where(d => d.GetDuid().SequenceEqual(duid) && d.GetIatype().Equals(iatype) && d.GetIaid() == iaid).ToList();
        }

        public List<IaPrefix> FindExpiredIaPrefixes()
        {
            List<DhcpLease> leases = _DhcpLeaseData.Where(d =>
            d.GetIatype() == IdentityAssoc.PD_TYPE &&
            d.GetValidEndTime() < DateTime.Now).OrderBy(d => d.GetValidEndTime()).ToList();
            return ToIaPrefixes(leases);
        }

        protected void InsertDhcpLease(DhcpLease lease)
        {
            _DhcpLeaseData.Add(lease);
        }
        /**
	 * Update dhcp lease.
	 *
	 * @param lease the lease
	 */
        protected void UpdateDhcpLease(DhcpLease lease)
        {
            foreach (var item in _DhcpLeaseData.Where(p => p.GetIpAddress() == lease.GetIpAddress()))
            {
                item.SetState(lease.GetState());
                item.SetStartTime(lease.GetStartTime());
                item.SetPreferredEndTime(lease.GetPreferredEndTime());
                item.SetValidEndTime(lease.GetValidEndTime());
                item.SetIaDhcpOptions(lease.GetIaDhcpOptions());
                item.SetIaAddrDhcpOptions(lease.GetIaAddrDhcpOptions());
            }
        }
        /**
	 * Delete dhcp lease.
	 *
	 * @param lease the lease
	 */
        protected void DeleteDhcpLease(DhcpLease lease)
        {
            _DhcpLeaseData.RemoveAll(d => d.GetIpAddress().ToString() == lease.GetIpAddress().ToString());
        }
        protected List<DhcpLease> FindExpiredLeases(byte iatype)
        {
            return _DhcpLeaseData.Where(d =>
                                        d.GetIatype() == iatype &&
                                        d.GetState() != IaAddress.STATIC &&
                                        d.GetValidEndTime() < DateTime.Now
                                        ).OrderBy(d => d.GetValidEndTime()).ToList();
        }

        /**
        * Find dhcp lease for InetAddr.
        *
        * @param inetAddr the InetAddr
        * @return the DhcpLease
        */
        protected DhcpLease FindDhcpLeaseForInetAddr(IPAddress inetAddr)
        {
            return _DhcpLeaseData.Where(d => d.GetIpAddress() == inetAddr).FirstOrDefault();
        }

        /**
	     * Update ipaddr options.
	     */
        protected void UpdateIpAddrOptions(IPAddress inetAddr, List<DhcpOption> ipAddrOptions)
        {
            foreach (var item in _DhcpLeaseData.Where(p => p.GetIpAddress().ToString() == inetAddr.ToString()))
            {
                item.SetIaAddrDhcpOptions(ipAddrOptions);
            }
        }
    }
}
