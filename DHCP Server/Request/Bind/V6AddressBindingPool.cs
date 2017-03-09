using PIXIS.DHCP.Config;
using PIXIS.DHCP.DB;
using PIXIS.DHCP.Option.V6;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static PIXIS.DHCP.Config.DhcpServerPolicies;

namespace PIXIS.DHCP.Request.Bind
{
    public class V6AddressBindingPool : BindingPool, DhcpV6OptionConfigObject
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected Range range;
        protected FreeList freeList;
        protected long preferredLifetime;
        protected long validLifetime;
        protected v6AddressPool pool;
        protected DhcpV6ConfigOptions dhcpConfigOptions;
        protected linkFilter linkFilter;
        //protected AssignDhcpV6Rule _assignDhcpV6Rule;
        /// <summary>
        /// DHCP 派發IP前，確認IP是否已經使用
        /// </summary>
        /// <param name="ip">檢查IP</param>
        /// <param name="wait">等待時間(毫秒)</param>
        /// <returns></returns>
        public Func<IPAddress, int, bool> CheckIPIsUsed { get; set; }

        /**
	 * Instantiates a new binding pool.
	 * 
	 * @param pool the pool
	 * 
	 * @throws DhcpServerConfigException if the AddressPool definition is invalid
	 */
        public V6AddressBindingPool(v6AddressPool pool)
        {
            this.pool = pool;
            //this._assignDhcpV6Rule = pool.assignDhcpv6Rule;
            try
            {
                this.range = new Range(pool.range);
            }
            catch (Exception ex)
            {
                log.Error("Invalid AddressPool definition");
                throw new Exception("Invalid AddressPool definition", ex);
            }
            //catch (UnknownHostException ex)
            //{
            //    log.error("Invalid AddressPool definition", ex);
            //    throw new DhcpServerConfigException("Invalid AddressPool definition", ex);
            //}
            freeList =

            new FreeList(new BigInteger(range.GetStartAddress().GetAddressBytes()),
                   new BigInteger(range.GetEndAddress().GetAddressBytes()));
            //reaper = new Timer(pool.getRange() + "_Reaper");
            dhcpConfigOptions = new DhcpV6ConfigOptions(pool.addrConfigOptions);
        }

        /**
         * Gets the range.
         * 
         * @return the range
         */
        public Range GetRange()
        {
            return range;
        }

        /// <summary>
        /// Gets the next available address in this address pool.
        /// </summary>
        /// <returns>the next available address</returns>
        public IPAddress GetNextAvailableAddress()
        {
            Debug.Assert(CheckIPIsUsed != null, "V6AddressBindingPool --GetNextAvailableAddress-- CheckIPIsUsed = null");
            if (freeList != null)
            {
                BigInteger next = freeList.GetNextFree();
                if (next.IntValue() != 0)
                {
                    try
                    {

                        byte[] nextByte = next.GetBytes();
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
                                log.InfoFormat("Assign IPv6 Address : {0}", ip.ToString());
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
                        log.Error("Unable to build IPv6 address from next free: " + ex);
                    }
                }
            }
            return null;
        }
        public IPAddress GetNextAvailableAddress(string v4IP1, string v4IP2)
        {
            if (freeList != null)
            {
                BigInteger next = freeList.GetNextFree();
                if (next != null)
                {
                    try
                    {
                        byte[] nextByte = next.GetBytes();
                        if (v4IP1 != "")
                        {
                            nextByte[12] = v4IP1.Length > 2 ? HexString(v4IP1.Substring(0, v4IP1.Length - 2)) : HexString("0");
                            nextByte[13] = v4IP1.Length > 2 ? HexString(v4IP1.Substring(v4IP1.Length - 2, v4IP1.Length - 1)) : HexString(v4IP1.Substring(0, v4IP1.Length));
                        }
                        nextByte[14] = v4IP2.Length > 2 ? HexString(v4IP2.Substring(0, v4IP2.Length - 2)) : HexString("0");
                        nextByte[15] = v4IP2.Length > 2 ? HexString(v4IP2.Substring(v4IP2.Length - 2, v4IP2.Length - 1)) : HexString(v4IP2.Substring(0, v4IP2.Length));
                        return new IPAddress(nextByte);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Unable to build IPv6 address from next free: " + ex);
                    }
                }
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
            if ((Util.CompareInetAddrs(range.GetStartAddress(), addr) >= 0) &&
                    (Util.CompareInetAddrs(range.GetEndAddress(), addr) <= 0))
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
         * Gets the preferred lifetime.
         * 
         * @return the preferred lifetime
         */
        public long GetPreferredLifetime()
        {
            return preferredLifetime;
        }

        /**
         * Gets the preferred lifetime ms.
         * 
         * @return the preferred lifetime ms
         */
        public long GetPreferredLifetimeMs()
        {
            return preferredLifetime * 1000;
        }

        /**
         * Sets the preferred lifetime.
         * 
         * @param preferredLifetime the new preferred lifetime
         */
        public void SetPreferredLifetime(long preferredLifetime)
        {
            this.preferredLifetime = preferredLifetime;
        }

        /**
         * Gets the valid lifetime.
         * 
         * @return the valid lifetime
         */
        public long GetValidLifetime()
        {
            return validLifetime;
        }

        /**
         * Gets the valid lifetime ms.
         * 
         * @return the valid lifetime ms
         */
        public long GetValidLifetimeMs()
        {
            return validLifetime * 1000;
        }

        /**
         * Sets the valid lifetime.
         * 
         * @param validLifetime the new valid lifetime
         */
        public void SetValidLifetime(long validLifetime)
        {
            this.validLifetime = validLifetime;
        }

        /**
         * Gets the pool.
         * 
         * @return the pool
         */
        public v6AddressPool GetV6AddressPool()
        {
            return pool;
        }

        /**
         * Sets the pool.
         * 
         * @param pool the new pool
         */
        public void SetV6AddressPool(v6AddressPool pool)
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

        public DhcpV6ConfigOptions GetDhcpConfigOptions()
        {
            return dhcpConfigOptions;
        }

        public void SetDhcpConfigOptions(DhcpV6ConfigOptions dhcpConfigOptions)
        {
            this.dhcpConfigOptions = dhcpConfigOptions;
        }

        public override string ToString()
        {
            return range.GetStartAddress().ToString() + "-" +
                    range.GetEndAddress().ToString();
        }

        public string FreeListToString()
        {
            return freeList.ToString();
        }



        public BigInteger GetSize()
        {
            return range.Size();
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

        //public AssignDhcpV6Rule GetV6AssignRule()
        //{
        //    return _assignDhcpV6Rule;
        //}

        private byte HexString(string str)
        {
            return Byte.Parse(str, System.Globalization.NumberStyles.HexNumber);
        }
        public bool IsFree(BigInteger free)
        {
            return !freeList.IsUsed(free);
        }
    }
}
