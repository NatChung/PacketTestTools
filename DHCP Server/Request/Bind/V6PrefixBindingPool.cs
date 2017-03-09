using PIXIS.DHCP.Config;
using PIXIS.DHCP.DB;
using PIXIS.DHCP.Option.V6;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Request.Bind
{
    public class V6PrefixBindingPool : BindingPool, DhcpV6OptionConfigObject
    {

        /** The log. */
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected Subnet subnet;
        protected int allocPrefixLen;
        protected FreeList freeList;
        protected long preferredLifetime;
        protected long validLifetime;
        protected v6PrefixPool pool;
        protected DhcpV6ConfigOptions dhcpConfigOptions;
        protected linkFilter linkFilter;
        // protected Timer reaper;

        /**
         * Instantiates a new binding pool.
         * 
         * @param pool the pool
         * 
         * @throws DhcpServerConfigException if the PrefixPool definition is invalid
         */
        public V6PrefixBindingPool(v6PrefixPool pool)
        {
            try
            {
                this.pool = pool;
                allocPrefixLen = pool.prefixLength;
                string[] cidr = pool.range.Split('/');
                if ((cidr == null) || (cidr.Length != 2))
                {
                    throw new Exception(
                            "Prefix pool must be specified in prefix/len notation");
                }
                subnet = new Subnet(cidr[0], cidr[1]);
                if (allocPrefixLen < subnet.GetPrefixLength())
                {
                    throw new Exception(
                            "Allocation prefix length must be greater or equal to pool prefix length");
                }
                int numPrefixes = (int)Math.Pow(2, (allocPrefixLen - subnet.GetPrefixLength()));
                freeList = new FreeList(new BigInteger(0),
                        new BigInteger(numPrefixes) - new BigInteger(1));
                //reaper = new Timer(pool.getRange() + "_Reaper");
                dhcpConfigOptions = new DhcpV6ConfigOptions(pool.prefixConfigOptions);
            }
            catch (Exception ex)
            {
                log.Error("Invalid PrefixPool definition");
                throw new Exception("Invalid PrefixPool definition", ex);
            }
        }

        public int GetAllocPrefixLen()
        {
            return allocPrefixLen;
        }

        private BigInteger CalculatePrefix()
        {
            return new BigInteger(2) ^ (128 - allocPrefixLen);
        }

        /**
         * Gets the next available prefix.
         * 
         * @return the next available prefix
         */
        public IPAddress GetNextAvailableAddress()
        {
            if (freeList != null)
            {
                BigInteger start = new BigInteger(subnet.GetSubnetAddress().GetAddressBytes());
                BigInteger next = freeList.GetNextFree();
                if (next != null)
                {
                    //TODO: if there are no more free addresses, then find one
                    //		that has been released or has been expired
                    try
                    {
                        BigInteger prefix = next * CalculatePrefix();
                        byte[] prefixByte = (start + prefix).GetBytes();
                        return new IPAddress(prefixByte);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Unable to build IPv6 prefix from next free: " + ex);
                    }
                }
            }
            return null;
        }

        /**
         * Sets the used.
         * 
         * @param addr the new used
         */
        public void SetUsed(IPAddress addr)
        {
            if (Contains(addr))
            {
                BigInteger prefix = new BigInteger(addr.GetAddressBytes());
                BigInteger start = new BigInteger(subnet.GetSubnetAddress().GetAddressBytes());
                freeList.SetUsed((prefix - start) / CalculatePrefix());
            }
        }

        /**
         * Sets the free.
         * 
         * @param addr the new free
         */
        public void SetFree(IPAddress addr)
        {
            if (Contains(addr))
            {
                BigInteger prefix = new BigInteger(addr.GetAddressBytes());
                BigInteger start = new BigInteger(subnet.GetSubnetAddress().GetAddressBytes());
                freeList.SetFree((prefix - start) / CalculatePrefix());
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
         * Contains.
         * 
         * @param addr the addr
         * 
         * @return true, if successful
         */
        public bool Contains(IPAddress addr)
        {
            if ((Util.CompareInetAddrs(addr, subnet.GetSubnetAddress()) >= 0) &&
                    (Util.CompareInetAddrs(addr, subnet.GetEndAddress()) <= 0))
            {
                return true;
            }
            return false;
        }

        public IPAddress GetStartAddress()
        {
            return subnet.GetSubnetAddress();
        }

        public IPAddress GetEndAddress()
        {
            return subnet.GetEndAddress();
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
        public v6PrefixPool GetV6PrefixPool()
        {
            return pool;
        }

        /**
         * Sets the pool.
         * 
         * @param pool the new pool
         */
        public void SetV6PrefixPool(v6PrefixPool pool)
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
            return subnet.GetSubnetAddress().ToString() + "/" + subnet.GetPrefixLength();
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
            // TODO: account for delegated prefix size
            return new BigInteger(subnet.GetEndAddress().GetAddressBytes()) -
                    new BigInteger(subnet.GetSubnetAddress().GetAddressBytes());
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
