using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Request.Bind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.DB
{
    public interface IaManager
    {
        /**
	 * Initialize the IaManager.
	 */
        void Init();

        /**
         * Create an IdentityAssoc object, including any contained
         * IaAddresses, IaPrefixes and DhcpOptions, as well as any DhcpOptions
         * contained in the IaAddresses or IaPrefixes themselves.
         * 
         * @param ia the IdentityAssoc to create
         */
        void CreateIA(IdentityAssoc ia);

        /**
         * Update an IdentityAssoc object, including any contained
         * IaAddresses, IaPrefixes and DhcpOptions, as well as any DhcpOptions
         * contained in the IaAddresses or IaPrefixes themselves.
         * 
         * @param ia the IdentityAssoc to update
         * @param addAddrs the IaAddresses/IaPrefixes to add to the IdentityAssoc
         * @param updateAddrs the IaAddresses/IaPrefixes to update in the IdentityAssoc
         * @param delAddrs the IaAddresses/IaPrefixes to delete from the IdentityAssoc
         */
        void UpdateIA(IdentityAssoc ia, List<IaAddress> addAddrs,
               List<IaAddress> updateAddrs, List<IaAddress> delAddrs);

        /**
         * Delete an IdentityAssoc object, and allow the database
         * constraints (cascade delete) to care of deleting any
         * contained IaAddresses, IaPrefixes and DhcpOptions, and further
         * cascading to delete any DhcpOptions contained in the
         * IaAddresses or IaPrefixes themselves.
         * 
         * @param ia the IdentityAssoc to delete
         */
        void DeleteIA(IdentityAssoc ia);

        /**
         * Get an IdentityAssoc by id.
         * 
         * @param id the ID of the IdentityAssoc to get
         * @return the IdentityAssoc for the given ID
         */
        //	public IdentityAssoc getIA(long id);

        /**
         * Locate an IdentityAssoc object by the key tuple duid-iaid-iatype.
         * Populate any contained IaAddresses, IaPrefixes and DhcpOptions, as well as
         * any DhcpOptions contained in the IaAddresses or IaPrefixes themselves.
         * 
         * @param duid the duid
         * @param iatype the iatype
         * @param iaid the iaid
         * 
         * @return a fully-populated IdentityAssoc, or null if not found
         */
        IdentityAssoc FindIA(byte[] duid, byte iatype, long iaid);

        /**
         * Locate an IdentityAssoc object by IaAddress
         * @param iaAddress
         * @return
         */
        IdentityAssoc FindIA(IaAddress iaAddress);

        /**
         * Find an IdentityAssoc for the given IP address.  That is,
         * locate the IaAddress or IaPrefix for the address, and then
         * locate the IdentityAssoc that contains that address object.
         * 
         * @param inetAddr the inet addr
         * 
         * @return the identity assoc
         */
        IdentityAssoc FindIA(IPAddress inetAddr);

        /**
         * Find expired IAs.  That is, find all the expired IaAddresses
         * and for each IaAddress, wrap it inside a corresponding
         * IdentityAssoc to be returned to the caller.
         * 
         * @param iatype
         * @return
         */
        List<IdentityAssoc> FindExpiredIAs(byte iatype);

        /**
         * Save dhcp option associated with an iaAddr.  Add the option
         * if it does not exist, otherwise update the existing option
         * only if the value has actually changed.
         * 
         * @param iaAddr
         * @param option
         */
        void SaveDhcpOption(IaAddress iaAddr, BaseDhcpOption option);

        /**
         * Delete dhcp option associated with an iaAddr.
         * 
         * @param iaAddr
         * @param option
         */
        void DeleteDhcpOption(IaAddress iaAddr, BaseDhcpOption option);

        /**
         * Update an IaAddress.
         * 
         * @param iaAddr the IaAddress to update
         */
        void UpdateIaAddr(IaAddress iaAddr);

        /**
         * Delete an IaAddress.
         * 
         * @param iaAddr the IaAddress to delete
         */
        void DeleteIaAddr(IaAddress iaAddr);

        /**
         * Update an IaPrefix.
         * 
         * @param iaPrefix the IaPrefix to update
         */
        void UpdateIaPrefix(IaPrefix iaPrefix);

        /**
         * Update an IaPrefix.
         * 
         * @param iaPrefix the IaPrefix to delete
         */
        void DeleteIaPrefix(IaPrefix iaPrefix);

        /**
         * Find existing IPs within an inclusive address range.
         * 
         * @param startAddr the start address of the range
         * @param endAddr the end address of the range
         * 
         * @return the list of InetAddress objects for existing IPs in the range
         */
        List<IPAddress> FindExistingIPs(IPAddress startAddr, IPAddress endAddr);

        /**
         * Find the unused IA Addresses within an inclusive address range.
         * 
         * @param startAddr the start address of the range
         * @param endAddr the end address of the range
         * 
         * @return the list of IaAddress objects that are unused in the range
         */
        List<IaAddress> FindUnusedIaAddresses(IPAddress startAddr, IPAddress endAddr);

        /**
         * Find the expired IA Addresses for the given IA type.
         * 
         * @param iatype the IA type
         * 
         * @return the list of IaAddress objects that are expired for the type
         */
        List<IaAddress> FindExpiredIaAddresses(byte iatype);

        /**
         * Find the unused IA Prefixes within an inclusive address range.
         * 
         * @param startAddr the start address of the range
         * @param endAddr the end address of the range
         * 
         * @return the list of IaPrefix objects that are unused in the range
         */
        List<IaPrefix> FindUnusedIaPrefixes(IPAddress startAddr, IPAddress endAddr);

        /**
         * Find the expired IA Prefixes.
         * 
         * @return the list of IaPrefix objects that are expired
         */
        List<IaPrefix> FindExpiredIaPrefixes();

        /**
         * Reconcile IA addresses to a set of ranges.  That is, delete any
         * lease or binding information for IPs that are outside the list of ranges.
         *   
         * @param ranges a list of IP address ranges to reconcile against
         */
        void ReconcileIaAddresses(List<Range> ranges);


        /**
         * For unit tests only
         */
        void DeleteAllIAs();
    }
}
