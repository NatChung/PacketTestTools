using PIXIS.DHCP.Config;
using PIXIS.DHCP.DB;
using PIXIS.DHCP.Utility;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static PIXIS.DHCP.Config.DhcpServerPolicies;

namespace PIXIS.DHCP.Request.Bind
{
    public abstract class BaseAddrBindingManager : BaseBindingManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public BaseAddrBindingManager() : base()
        {
        }

        protected override void StartReaper()
        {
            //TODO: separate properties for address/prefix binding managers?
            long reaperStartupDelay = long.Parse(Property.BINDING_MANAGER_REAPER_STARTUP_DELAY.Value());
            //DhcpServerPolicies.GlobalPolicyAsLong(Property.BINDING_MANAGER_REAPER_STARTUP_DELAY);
            long reaperRunPeriod = long.Parse(Property.BINDING_MANAGER_REAPER_RUN_PERIOD.Value());
            //DhcpServerPolicies.GlobalPolicyAsLong(Property.BINDING_MANAGER_REAPER_RUN_PERIOD);

            //reaper = new Timer("BindingReaper");
            //reaper.schedule(new ReaperTimerTask(), reaperStartupDelay, reaperRunPeriod);
        }

        /// <summary>
        /// Perform the DDNS delete processing when a lease is released or expired.
        /// </summary>
        /// <param name="ia">iaAddr the released or expired IaAddress </param>
        /// <param name="iaAddr">iaAddr the released or expired IaAddress </param>
        protected abstract void DdnsDelete(IdentityAssoc ia, IaAddress iaAddr);

        /// <summary>
        /// Return the IA type for this binding.  This is a hack to allow consolidated
        /// code in this base class (i.e. expireAddresses) for use by the subclasses.
        /// </summary>
        /// <returns></returns>
        protected abstract byte GetIaType();

        /// <summary>
        /// Release an IaAddress.  If policy dictates, the address will be deleted,
        /// otherwise the state will be marked as released instead.  In either case,
        /// a DDNS delete will be issued for the address binding.
        /// </summary>
        /// <param name="ia">iaAddr the IaAddress to be released</param>
        /// <param name="iaAddr">iaAddr the IaAddress to be released</param>
        public void ReleaseIaAddress(IdentityAssoc ia, IaAddress iaAddr)
        {
            try
            {
                log.Info("Releasing address: " + iaAddr.GetIpAddress().ToString());
                //DdnsDelete(ia, iaAddr); 
                if (DhcpServerPolicies.GlobalPolicyAsBoolean(
                        Property.BINDING_MANAGER_DELETE_OLD_BINDINGS))
                {
                    iaMgr.DeleteIaAddr(iaAddr);
                    // free the address only if it is deleted from the db,
                    // otherwise, we will get a unique constraint violation
                    // if another client obtains this released IP address
                    FreeAddress(iaAddr.GetIpAddress());
                }
                else
                {
                    iaAddr.SetStartTime(DateTime.Now);
                    iaAddr.SetPreferredEndTime(DateTime.Now);
                    iaAddr.SetValidEndTime(DateTime.Now);
                    iaAddr.SetState(IaAddress.RELEASED);
                    iaMgr.UpdateIaAddr(iaAddr);
                    log.Info("Address released: " + iaAddr.ToString());
                }
            }
            catch (Exception ex)
            {
                log.Error("Failed to release address");
            }
        }

        /// <summary>
        /// Decline an IaAddress.  This is done when the client declines an address.
        /// Perform a DDNS delete just in case it was already registered, then mark
        /// the address as declined (unavailable).
        /// </summary>
        /// <param name="ia">iaAddr the declined IaAddress.</param>
        /// <param name="iaAddr">iaAddr the declined IaAddress.</param>
        public void DeclineIaAddress(IdentityAssoc ia, IaAddress iaAddr)
        {
            try
            {
                log.Info("Declining address: " + iaAddr.GetIpAddress().ToString());
                //DdnsDelete(ia, iaAddr);
                iaAddr.SetStartTime(DateTime.Now);
                iaAddr.SetPreferredEndTime(DateTime.Now);
                iaAddr.SetValidEndTime(DateTime.Now);
                iaAddr.SetState(IaAddress.DECLINED);
                iaMgr.UpdateIaAddr(iaAddr);
                log.Info("Address declined: " + iaAddr.ToString());
            }
            catch (Exception ex)
            {
                log.Error("Failed to decline address");
            }
        }

        /// <summary>
        ///  Callback from the ExpireTimerTask started when the lease was granted.
        /// </summary>
        /// <param name="ia">iaAddr the ia addr</param>
        /// <param name="iaAddr">iaAddr the ia addr</param>
        public void ExpireIaAddress(IdentityAssoc ia, IaAddress iaAddr)
        {
            try
            {
                log.Info("Expiring: " + iaAddr.ToString());
                //DdnsDelete(ia, iaAddr);
                if (DhcpServerPolicies.GlobalPolicyAsBoolean(
                        Property.BINDING_MANAGER_DELETE_OLD_BINDINGS))
                {
                    log.Debug("Deleting expired address: " + iaAddr.GetIpAddress());
                    iaMgr.DeleteIaAddr(iaAddr);
                    // free the address only if it is deleted from the db,
                    // otherwise, we will get a unique constraint violation
                    // if another client obtains this released IP address
                    FreeAddress(iaAddr.GetIpAddress());
                }
                else
                {
                    iaAddr.SetStartTime(DateTime.Now);
                    iaAddr.SetPreferredEndTime(DateTime.Now);
                    iaAddr.SetValidEndTime(DateTime.Now);
                    iaAddr.SetState(IaAddress.EXPIRED);
                    log.Debug("Updating expired address: " + iaAddr.GetIpAddress());
                    iaMgr.UpdateIaAddr(iaAddr);
                }
            }
            catch (Exception ex)
            {
                log.Error("Failed to expire address");
            }
        }

        /// <summary>
        /// Callback from the ReaperTimerTask started when the BindingManager initialized.
        /// Find any expired addresses as of now, and expire them already.
        /// </summary>
        public void ExpireAddresses()
        {
            List<IdentityAssoc> expiredIAs = iaMgr.FindExpiredIAs(GetIaType());
            if ((expiredIAs != null) && expiredIAs.Count > 0)
            {
                log.Info("Found " + expiredIAs.Count + " expired bindings of type: " +
                        IdentityAssoc.IaTypeToString(GetIaType()));
                foreach (IdentityAssoc ia in expiredIAs)
                {
                    List<IaAddress> expiredAddrs = ia.GetIaAddresses();
                    if ((expiredAddrs != null) && expiredAddrs.Count > 0)
                    {
                        // due to the implementation of findExpiredIAs, each IdentityAssoc
                        // SHOULD have only one IaAddress within it to be expired
                        log.Info("Found " + expiredAddrs.Count + " expired bindings for IA: " +
                                "duid=" + Util.ToHexString(ia.GetDuid()) + " iaid=" + ia.GetIaid());
                        foreach (IaAddress iaAddress in expiredAddrs)
                        {
                            ExpireIaAddress(ia, iaAddress);
                        }
                    }
                }
            }
        }
    }
}
