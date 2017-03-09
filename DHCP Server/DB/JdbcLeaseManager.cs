using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Request.Bind;
using System.Net;

namespace PIXIS.DHCP.DB
{
    //public class JdbcIaManager extends JdbcDaoSupport implements IaManager
    public class JdbcLeaseManager : IaManager
    {
        /** The log. */
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected IdentityAssocDAO iaDao;
        protected IaAddressDAO iaAddrDao;
        protected IaPrefixDAO iaPrefixDao;
        protected DhcpOptionDAO dhcpOptDao;

        public void Init()
        {
        }

        public void CreateIA(IdentityAssoc ia)
        {
        }

        public void UpdateIA(IdentityAssoc ia, List<IaAddress> addAddrs, List<IaAddress> updateAddrs, List<IaAddress> delAddrs)
        {
        }

        public void DeleteIA(IdentityAssoc ia)
        {
        }

        public IdentityAssoc FindIA(byte[] duid, byte iatype, long iaid)
        {
            return null;
        }

        public IdentityAssoc FindIA(IaAddress iaAddress)
        {
            return null;
        }

        public IdentityAssoc FindIA(IPAddress inetAddr)
        {
            return null;
        }

        public List<IdentityAssoc> FindExpiredIAs(byte iatype)
        {
            return null;
        }

        public void SaveDhcpOption(IaAddress iaAddr, BaseDhcpOption option)
        {
        }

        public void DeleteDhcpOption(IaAddress iaAddr, BaseDhcpOption option)
        {
        }

        public void UpdateIaAddr(IaAddress iaAddr)
        {
        }

        public void DeleteIaAddr(IaAddress iaAddr)
        {
        }

        public void UpdateIaPrefix(IaPrefix iaPrefix)
        {
        }

        public void DeleteIaPrefix(IaPrefix iaPrefix)
        {
        }

        public List<IPAddress> FindExistingIPs(IPAddress startAddr, IPAddress endAddr)
        {
            return null;
        }

        public List<IaAddress> FindUnusedIaAddresses(IPAddress startAddr, IPAddress endAddr)
        {
            return null;
        }

        public List<IaAddress> FindExpiredIaAddresses(byte iatype)
        {
            return null;
        }

        public List<IaPrefix> FindUnusedIaPrefixes(IPAddress startAddr, IPAddress endAddr)
        {
            return null;
        }

        public List<IaPrefix> FindExpiredIaPrefixes()
        {
            return null;
        }

        public void ReconcileIaAddresses(List<Range> ranges)
        {
        }

        public void DeleteAllIAs()
        {
        }
    }
}
