using PIXIS.DHCP.Config;
using PIXIS.DHCP.DB;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static PIXIS.DHCP.Config.DhcpServerPolicies;
using System.Threading;

namespace PIXIS.DHCP.Request.Bind
{
    public class V4AddressBindingPool : BindingPool, DhcpV4OptionConfigObject
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected Range range;
        protected FreeList freeList;
        protected long leasetime;
        protected v4AddressPool pool;
        protected DhcpV4ConfigOptions v4ConfigOptions;
        protected linkFilter linkFilter;
        private readonly static object _lock = new object();
        /// <summary>
        /// DHCP 派發IP前，確認IP是否已經使用
        /// </summary>
        /// <param name="ip">檢查IP</param>
        /// <param name="wait">等待時間(毫秒)</param>
        /// <returns></returns>
        public Func<IPAddress, int, bool> CheckIPIsUsed { get; set; }

        //protected Timer reaper;

        /**
         * Instantiates a new binding pool.
         * 
         * @param pool the pool
         * 
         * @throws DhcpServerConfigException if the AddressPool definition is invalid
         */
        public V4AddressBindingPool(v4AddressPool pool)
        {
            this.pool = pool;
            try
            {
                this.range = new Range(pool.range);
            }
            catch (Exception ex)
            {
                log.Error("Invalid AddressPool definition");
                throw new Exception("Invalid AddressPool definition", ex);
            }
            freeList =

            new FreeList(new BigInteger(range.GetStartAddress().GetAddressBytes()),
                    new BigInteger(range.GetEndAddress().GetAddressBytes()));
            //reaper = new Timer(pool.getRange() + "_Reaper");
            v4ConfigOptions = new DhcpV4ConfigOptions(pool.configOptions);
        }

        /**
         * Gets the range.
         * 
         * @return the range
         */
        public Range getRange()
        {
            return range;
        }

        /// <summary>
        /// Gets the next available address in this address pool.
        /// </summary>
        /// <returns>the next available address</returns>
        public IPAddress GetNextAvailableAddress()
        {
            Monitor.Enter(_lock);
            try
            {
                if (freeList != null)
                {
                    BigInteger next = freeList.GetNextFree();
                    if (next.IntValue() != 0)
                    {
                        try
                        {
                            IPAddress ip = new IPAddress(next.GetBytes());
                            int pingCheckTimeout = DhcpServerPolicies.GlobalPolicyAsInt(Property.V4_PINGCHECK_TIMEOUT);
                            if (pingCheckTimeout > 0)
                            {
                                try
                                {
                                    if (CheckIPIsUsed(ip, pingCheckTimeout))
                                    {
                                        log.Warn("Next free address answered ping check: " + ip.ToString());

                                        SetUsed(ip);
                                        return GetNextAvailableAddress();   // try again
                                    }
                                    log.InfoFormat("Assign IPv4 Address : {0}", ip.ToString());
                                }
                                catch (IOException ex)
                                {
                                    log.Error("Failed to perform v4 ping check: " + ex);
                                }
                            }
                            return ip;
                        }
                        catch (Exception ex)
                        {
                            log.Error("Unable to build IPv4 address from next free: " + ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Unable to build IPv4 address from next free: " + ex);
            }
            finally
            {
                Monitor.Exit(_lock);
            }
            return null;
        }

        /**
         * Sets an IP address in this address pool as used.
         * 
         * @param addr the address to set used
         */
        public void SetUsed(IPAddress addr)
        {
            if (Contains(addr))
            {
                freeList.SetUsed(new BigInteger(addr.GetAddressBytes()));
            }
        }

        /**
         * Sets an IP address in this address pool as free.
         * 
         * @param addr the address to set free
         */
        public void SetFree(IPAddress addr)
        {
            if (Contains(addr))
            {
                freeList.SetFree(new BigInteger(addr.GetAddressBytes()));
            }
        }

        /**
         * Start expire timer task.
         * 
         * @param iaAddr the ia addr
         * @param secsUntilExpiration the secs until expiration
         */
        public void StartExpireTimerTask(IaAddress iaAddr, long secsUntilExpiration)
        {
            // convert delay from seconds (lifetime) --> milliseconds (delay)
            //		reaper.schedule(new ExpireTimerTask(iaAddr), secsUntilExpiration*1000);
        }

        /**
         * Test if the given address is contained within this address pool.
         * 
         * @param addr the address to test for containment in this address pool
         * 
         * @return true, if successful
         */
        public bool Contains(IPAddress addr)
        {
            if ((Util.CompareInetAddrs(addr, range.GetStartAddress()) >= 0) &&
                    (Util.CompareInetAddrs(addr, range.GetEndAddress()) <= 0))
            {
                return true;
            }
            return false;
        }

        /**
         * Gets the start address.
         * 
         * @return the start address
         */
        public IPAddress GetStartAddress()
        {
            return range.GetStartAddress();
        }

        /**
         * Sets the start address.
         * 
         * @param startAddress the new start address
         */
        public void SetStartAddress(IPAddress startAddress)
        {
            range.SetEndAddress(startAddress);
        }

        /**
         * Gets the end address.
         * 
         * @return the end address
         */
        public IPAddress GetEndAddress()
        {
            return range.GetEndAddress();
        }

        /**
         * Sets the end address.
         * 
         * @param endAddress the new end address
         */
        public void SetEndAddress(IPAddress endAddress)
        {
            range.SetEndAddress(endAddress);
        }

        /**
         * Gets the leasetime.
         * 
         * @return the leasetime
         */
        public long GetLeasetime()
        {
            return leasetime;
        }

        /**
         * Gets the leasetime ms.
         * 
         * @return the leasetime ms
         */
        public long GetLeasetimeMs()
        {
            return leasetime * 1000;
        }

        /**
         * Sets the leasetime.
         * 
         * @param leasetime the new leasetime
         */
        public void SetLeasetime(long leasetime)
        {
            this.leasetime = leasetime;
        }

        /**
         * Gets the pool.
         * 
         * @return the pool
         */
        public v4AddressPool GetV4AddressPool()
        {
            return pool;
        }

        /**
         * Sets the pool.
         * 
         * @param pool the new pool
         */
        public void SetV4AddressPool(v4AddressPool pool)
        {
            this.pool = pool;
        }

        /**
         * Gets the link filter.
         * 
         * @return the link filter
         */
        public linkFilter GetLinkFilter()
        {
            return linkFilter;
        }

        /**
         * Sets the link filter.
         * 
         * @param linkFilter the new link filter
         */
        public void SetLinkFilter(linkFilter linkFilter)
        {
            this.linkFilter = linkFilter;
        }

        public DhcpV4ConfigOptions GetV4ConfigOptions()
        {
            return v4ConfigOptions;
        }

        public void SetV4ConfigOptions(DhcpV4ConfigOptions v4ConfigOptions)
        {
            this.v4ConfigOptions = v4ConfigOptions;
        }

        public override string ToString()
        {
            return range.GetStartAddress().GetAddressBytes() + "-" +
                    range.GetEndAddress().GetAddressBytes();
        }

        public string FreeListToString()
        {
            return freeList.ToString();
        }


        public long GetPreferredLifetime()
        {
            // TODO: check this overloaded use of v6 interface
            return GetLeasetime();
        }


        public long GetValidLifetime()
        {
            // TODO: check this overloaded use of v6 interface
            return GetLeasetime();
        }

        public long GetPreferredLifetimeMs()
        {
            // TODO: check this overloaded use of v6 interface
            return GetLeasetimeMs();
        }

        public long GetValidLifetimeMs()
        {
            // TODO: check this overloaded use of v6 interface
            return GetLeasetimeMs();
        }

        public List<filter> GetFilters()
        {
            if (pool != null)
                return pool.filters;
            return null;
        }

        public List<Policy> GetPolicies()
        {
            if (pool != null)
                return pool.policies;
            return null;
        }

        public BigInteger GetSize()
        {
            return range.Size();
        }

        //public AssignDhcpV6Rule GetV6AssignRule()
        //{
        //    return AssignDhcpV6Rule.Noen;
        //}
        public IPAddress GetNextAvailableAddress(string str1, string str2)
        {
            throw new NotImplementedException();
        }
        public bool IsFree(BigInteger free)
        {
            return !freeList.IsUsed(free);
        }
    }
}
