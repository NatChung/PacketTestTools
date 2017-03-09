using PIXIS.DHCP.Config;
using PIXIS.DHCP.Message;
using PIXIS.DHCP.Xml;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static PIXIS.DHCP.Config.DhcpServerPolicies;

namespace PIXIS.DHCP.Request.Dns
{
    public class DdnsUpdater
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static ExecutorService executor = Executors.newCachedThreadPool();

        /** The sync. */
        private bool sync;

        /** The fwd zone. */
        private string fwdZone;

        /** The fwd ttl. */
        private long fwdTtl;

        /** The fwd server. */
        private string fwdServer;

        /** The fwd tsig key name. */
        private string fwdTsigKeyName;

        /** The fwd tsig algorithm. */
        private string fwdTsigAlgorithm;

        /** The fwd tsig key data. */
        private string fwdTsigKeyData;

        /** The rev zone. */
        private string revZone;

        /** The rev zone bit length. */
        private int revZoneBitLength;

        /** The rev ttl. */
        private long revTtl;

        /** The rev server. */
        private string revServer;

        /** The rev tsig key name. */
        private string revTsigKeyName;

        /** The rev tsig algorithm. */
        private string revTsigAlgorithm;

        /** The rev tsig key data. */
        private string revTsigKeyData;

        private DhcpMessage requestMsg;

        /** The client link. */
        private link clientLink;

        /** The config object. */
        private DhcpConfigObject configObj;

        /** the inet addr. */
        private IPAddress addr;

        /** The fqdn. */
        private string fqdn;

        /** The duid. */
        private byte[] duid;

        /** The lifetime. */
        private long lifetime;

        /** The do forward update. */
        private bool doForwardUpdate;

        /** The is delete. */
        private bool isDelete;

        private DdnsCallback callback;
        //private link link;
        //private IPAddress iPAddress;
        //private byte[] v1;
        //private bool v2;
        //private bool v3;
        //private DdnsCallback ddnsComplete;

        /**
         * Instantiates a new ddns updater.
         * 
         * @param clientLink the client link
         * @param bindingAddr the binding addr
         * @param fqdn the fqdn
         * @param duid the duid
         * @param doForwardUpdate the do forward update
         * @param isDelete the is delete
         */
        public DdnsUpdater(link clientLink, DhcpConfigObject configObj,
                IPAddress addr, string fqdn, byte[] duid, long lifetime,
                bool doForwardUpdate, bool isDelete, DdnsCallback callback) :
            this(null, clientLink, configObj, addr, fqdn, duid, lifetime, doForwardUpdate, isDelete, callback)
        {

        }

        /**
         * Instantiates a new ddns updater.
         *
         * @param requestMsg the request msg
         * @param clientLink the client link
         * @param bindingAddr the binding addr
         * @param fqdn the fqdn
         * @param doForwardUpdate the do forward update
         * @param isDelete the is delete
         */
        public DdnsUpdater(DhcpMessage requestMsg, link clientLink, DhcpConfigObject configObj,
                IPAddress addr, string fqdn, byte[] duid, long lifetime,
                bool doForwardUpdate, bool isDelete,
                DdnsCallback callback)
        {
            this.requestMsg = requestMsg;
            this.duid = duid;
            this.clientLink = clientLink;
            this.configObj = configObj;
            this.addr = addr;
            this.fqdn = fqdn;
            this.lifetime = lifetime;
            this.doForwardUpdate = doForwardUpdate;
            this.isDelete = isDelete;
            this.callback = callback;
        }

        //public DdnsUpdater(DhcpV6Message requestMsg, link link, DhcpConfigObject configObj, IPAddress iPAddress, string fqdn, byte[] v1, bool v2, bool doForwardUpdate, bool v3, DdnsCallback ddnsComplete)
        //{
        //    this.requestMsg = requestMsg;
        //    this.link = link;
        //    this.configObj = configObj;
        //    this.iPAddress = iPAddress;
        //    this.fqdn = fqdn;
        //    this.v1 = v1;
        //    this.v2 = v2;
        //    this.doForwardUpdate = doForwardUpdate;
        //    this.v3 = v3;
        //    this.ddnsComplete = ddnsComplete;
        //}

        /**
         * Process updates.
         */
        public void ProcessUpdates()
        {
            if (sync)
            {
                //Run();
            }
            else
            {
                executor.Submit(this);
            }
        }

        /* (non-Javadoc)
         * @see java.lang.Runnable#run()
         */
        //public void Run()
        //{
        //    SetupPolicies(configObj, lifetime);
        //    try
        //    {
        //        if (doForwardUpdate)
        //        {
        //            ForwardDdnsUpdate fwdUpdate = new ForwardDdnsUpdate(fqdn, addr, duid);
        //            fwdUpdate.SetServer(fwdServer);
        //            fwdUpdate.SetZone(fwdZone);
        //            fwdUpdate.SetTtl(fwdTtl);
        //            fwdUpdate.SetTsigKeyName(fwdTsigKeyName);
        //            fwdUpdate.SetTsigAlgorithm(fwdTsigAlgorithm);
        //            fwdUpdate.SetTsigKeyData(fwdTsigKeyData);
        //            if (!isDelete)
        //                callback.FwdAddComplete(fwdUpdate.SendAdd());
        //            else
        //                callback.FwdDeleteComplete(fwdUpdate.SendDelete());
        //        }
        //        ReverseDdnsUpdate revUpdate = new ReverseDdnsUpdate(fqdn, addr, duid);
        //        revUpdate.SetServer(revServer);
        //        revUpdate.SetZone(revZone);
        //        revUpdate.SetRevZoneBitLength(revZoneBitLength);
        //        revUpdate.SetTtl(revTtl);
        //        revUpdate.SetTsigKeyName(revTsigKeyName);
        //        revUpdate.SetTsigAlgorithm(revTsigAlgorithm);
        //        revUpdate.SetTsigKeyData(revTsigKeyData);
        //        if (!isDelete)
        //            callback.RevAddComplete(revUpdate.SendAdd());
        //        else
        //            callback.RevDeleteComplete(revUpdate.SendDelete());
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex, "Failure performing DDNS updates");
        //        callback.FwdAddComplete(false);
        //        callback.FwdDeleteComplete(false);
        //        callback.RevAddComplete(false);
        //        callback.RevDeleteComplete(false);
        //    }
        //}

        /**
         * Sets the up policies.
         * 
         * @param addrBindingPool the new up policies
         */
        private void SetupPolicies(DhcpConfigObject addrBindingPool, long lifetime)
        {
            sync = DhcpServerPolicies.EffectivePolicyAsbool(requestMsg,
                    addrBindingPool, clientLink, Property.DDNS_SYNCHRONIZE);

            string zone = DhcpServerPolicies.EffectivePolicy(requestMsg,
                    addrBindingPool, clientLink, Property.DDNS_FORWARD_ZONE_NAME);
            if ((zone != null) && !string.IsNullOrEmpty(zone))
                fwdZone = zone;

            zone = DhcpServerPolicies.EffectivePolicy(requestMsg,
                    addrBindingPool, clientLink, Property.DDNS_REVERSE_ZONE_NAME);
            if ((zone != null) && !string.IsNullOrEmpty(zone))
                revZone = zone;

            revZoneBitLength = DhcpServerPolicies.EffectivePolicyAsInt(requestMsg,
                    addrBindingPool, clientLink, Property.DDNS_REVERSE_ZONE_BITLENGTH);

            long ttl = 0;
            float ttlFloat = DhcpServerPolicies.EffectivePolicyAsFloat(requestMsg,
                    addrBindingPool, clientLink, Property.DDNS_TTL);
            if (ttlFloat < 1)
            {
                // if less than one, then percentage of lifetime seconds
                ttl = (long)(lifetime * ttlFloat);
            }
            else
            {
                // if greater than one, then absolute number of seconds
                ttl = (long)ttlFloat;
            }

            fwdTtl = ttl;
            ttlFloat = DhcpServerPolicies.EffectivePolicyAsFloat(requestMsg,
                    addrBindingPool, clientLink, Property.DDNS_FORWARD_ZONE_TTL);
            if (ttlFloat < 1)
            {
                // if less than one, then percentage of lifetime seconds
                fwdTtl = (long)(lifetime * ttlFloat);
            }
            else
            {
                // if greater than one, then absolute number of seconds
                fwdTtl = (long)ttlFloat;
            }

            revTtl = ttl;
            ttlFloat = DhcpServerPolicies.EffectivePolicyAsFloat(requestMsg,
                    addrBindingPool, clientLink, Property.DDNS_REVERSE_ZONE_TTL);
            if (ttlFloat < 1)
            {
                // if less than one, then percentage of lifetime seconds
                revTtl = (long)(lifetime * ttlFloat);
            }
            else
            {
                // if greater than one, then absolute number of seconds
                revTtl = (long)ttlFloat;
            }

            string server = DhcpServerPolicies.EffectivePolicy(requestMsg,
                    addrBindingPool, clientLink, Property.DDNS_SERVER);

            fwdServer = server;
            revServer = server;

            server = DhcpServerPolicies.EffectivePolicy(requestMsg,
                    addrBindingPool, clientLink, Property.DDNS_FORWARD_ZONE_SERVER);
            if ((server != null) && !string.IsNullOrEmpty(server))
                fwdServer = server;
            server = DhcpServerPolicies.EffectivePolicy(requestMsg,
                    addrBindingPool, clientLink, Property.DDNS_REVERSE_ZONE_SERVER);
            if ((server != null) && !string.IsNullOrEmpty(server))
                revServer = server;

            string tsigKeyName = DhcpServerPolicies.EffectivePolicy(requestMsg,
                    addrBindingPool, clientLink, Property.DDNS_TSIG_KEYNAME);

            fwdTsigKeyName = tsigKeyName;
            revTsigKeyName = tsigKeyName;

            tsigKeyName = DhcpServerPolicies.EffectivePolicy(requestMsg,
                    addrBindingPool, clientLink, Property.DDNS_FORWARD_ZONE_TSIG_KEYNAME);
            if ((tsigKeyName != null) && !string.IsNullOrEmpty(tsigKeyName))
                fwdTsigKeyName = tsigKeyName;
            tsigKeyName = DhcpServerPolicies.EffectivePolicy(requestMsg,
                    addrBindingPool, clientLink, Property.DDNS_REVERSE_ZONE_TSIG_KEYNAME);
            if ((tsigKeyName != null) && !string.IsNullOrEmpty(tsigKeyName))
                revTsigKeyName = tsigKeyName;

            string tsigAlgorithm = DhcpServerPolicies.EffectivePolicy(requestMsg,
                    addrBindingPool, clientLink, Property.DDNS_TSIG_ALGORITHM);

            fwdTsigAlgorithm = tsigAlgorithm;
            revTsigAlgorithm = tsigAlgorithm;

            tsigAlgorithm = DhcpServerPolicies.EffectivePolicy(requestMsg,
                    addrBindingPool, clientLink, Property.DDNS_FORWARD_ZONE_TSIG_ALGORITHM);
            if ((tsigAlgorithm != null) && !string.IsNullOrEmpty(tsigAlgorithm))
                fwdTsigAlgorithm = tsigAlgorithm;
            tsigAlgorithm = DhcpServerPolicies.EffectivePolicy(requestMsg,
                    addrBindingPool, clientLink, Property.DDNS_REVERSE_ZONE_TSIG_ALGORITHM);
            if ((tsigAlgorithm != null) && !string.IsNullOrEmpty(tsigAlgorithm))
                revTsigAlgorithm = tsigAlgorithm;

            string tsigKeyData = DhcpServerPolicies.EffectivePolicy(requestMsg,
                    addrBindingPool, clientLink, Property.DDNS_TSIG_KEYDATA);

            fwdTsigKeyData = tsigKeyData;
            revTsigKeyData = tsigKeyData;

            tsigKeyData = DhcpServerPolicies.EffectivePolicy(requestMsg,
                    addrBindingPool, clientLink, Property.DDNS_FORWARD_ZONE_TSIG_KEYDATA);
            if ((tsigKeyData != null) && !string.IsNullOrEmpty(tsigKeyData))
                fwdTsigKeyData = tsigKeyData;
            tsigKeyData = DhcpServerPolicies.EffectivePolicy(requestMsg,
                    addrBindingPool, clientLink, Property.DDNS_REVERSE_ZONE_TSIG_KEYDATA);
            if ((tsigKeyData != null) && !string.IsNullOrEmpty(tsigKeyData))
                revTsigKeyData = tsigKeyData;
        }
    }
}
