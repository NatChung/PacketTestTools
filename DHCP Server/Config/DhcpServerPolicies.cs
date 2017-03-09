using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIXIS.DHCP.Message;
using PIXIS.DHCP.V4Process;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Config
{
    public class DhcpServerPolicies
    {
        public enum Propertya
        {

        }
        public class Property
        {
            public static Property CHANNEL_THREADPOOL_SIZE = new Property("channel.threadPoolSize", "16");
            public static Property CHANNEL_MAX_CHANNEL_MEMORY = new Property("channel.maxChannelMemory", "1048576");// 1024 x 1024
            public static Property CHANNEL_MAX_TOTAL_MEMORY = new Property("channel.maxTotalMemory", "1048576");// 1024 x 1024
            public static Property CHANNEL_READ_BUFFER_SIZE = new Property("channel.readBufferSize", "307200");// 300 bytes x 1K clients
            public static Property CHANNEL_WRITE_BUFFER_SIZE = new Property("channel.writeBufferSize", "307200");//  300 bytes x 1K clients
            public static Property DATABASE_SCHEMA_TYTPE = new Property("database.schemaType", "jdbc-derby");
            public static Property DATABASE_SCHEMA_VERSION = new Property("database.schemaVersion", "2");
            public static Property DHCP_PROCESSOR_RECENT_MESSAGE_TIMER = new Property("dhcp.processor.recentMessageTimer", "5000");
            public static Property DHCP_IGNORE_LOOPBACK = new Property("dhcp.ignoreLoopback", "true");
            public static Property DHCP_IGNORE_LINKLOCAL = new Property("dhcp.ignoreLinkLocal", "true");
            public static Property DHCP_IGNORE_SELF_PACKETS = new Property("dhcp.ignoreSelfPackets", "true");
            public static Property BINDING_MANAGER_REAPER_STARTUP_DELAY = new Property("binding.manager.reaper.startupDelay", "10000");
            public static Property BINDING_MANAGER_REAPER_RUN_PERIOD = new Property("binding.manager.reaper.runPeriod", "60000");
            public static Property BINDING_MANAGER_OFFER_EXPIRATION = new Property("binding.manager.offerExpiration", "12000");
            public static Property BINDING_MANAGER_DELETE_OLD_BINDINGS = new Property("binding.manager.deleteOldBindings", "false");
            public static Property SEND_REQUESTED_OPTIONS_ONLY = new Property("sendRequestedOptionsOnly", "false");
            public static Property SUPPORT_RAPID_COMMIT = new Property("supportRapidCommit", "false");
            public static Property VERIFY_UNKNOWN_REBIND = new Property("verifyUnknownRebind", "false");
            public static Property PREFERRED_LIFETIME = new Property("preferredLifetime", "3600");
            public static Property VALID_LIFETIME = new Property("validLifetime", "3600");
            public static Property IA_NA_T1 = new Property("iaNaT1", "0.5");
            public static Property IA_NA_T2 = new Property("iaNaT2", "0.8");
            public static Property IA_PD_T1 = new Property("iaPdT1", "0.5");
            public static Property IA_PD_T2 = new Property("iaPdT2", "0.8");
            public static Property DDNS_UPDATE = new Property("ddns.update", "none");// acceptable values: none, server, client, etc...
            public static Property DDNS_SYNCHRONIZE = new Property("ddns.synchronize", "false");//
            public static Property DDNS_DOMAIN = new Property("ddns.domain", "");//
            public static Property DDNS_TTL = new Property("ddns.ttl", "0.3");//1/3 of the lifetime
            public static Property DDNS_SERVER = new Property("ddns.server", "");
            public static Property DDNS_TSIG_KEYNAME = new Property("ddns.tsig.keyNam", "");
            public static Property DDNS_TSIG_ALGORITHM = new Property("ddns.tsig.algorithm", "");
            public static Property DDNS_TSIG_KEYDATA = new Property("ddns.tsig.keyData", "");
            public static Property DDNS_FORWARD_ZONE_NAME = new Property("ddns.forward.zone.name", "");
            public static Property DDNS_FORWARD_ZONE_TTL = new Property("ddns.forward.zone.ttl", "0.3");
            public static Property DDNS_FORWARD_ZONE_SERVER = new Property("ddns.forward.zone.server", "");
            public static Property DDNS_FORWARD_ZONE_TSIG_KEYNAME = new Property("ddns.forward.zone.tsig.keyName", "");
            public static Property DDNS_FORWARD_ZONE_TSIG_ALGORITHM = new Property("ddns.forward.zone.tsig.algorithm", "");
            public static Property DDNS_FORWARD_ZONE_TSIG_KEYDATA = new Property("ddns.forward.zone.tsig.keyData", "");
            public static Property DDNS_REVERSE_ZONE_NAME = new Property("ddns.reverse.zone.name", "");
            public static Property DDNS_REVERSE_ZONE_BITLENGTH = new Property("ddns.reverse.zone.bitLength", "64");
            public static Property DDNS_REVERSE_ZONE_TTL = new Property("ddns.reverse.zone.ttl", "0.3");
            public static Property DDNS_REVERSE_ZONE_SERVER = new Property("ddns.reverse.zone.server", "");
            public static Property DDNS_REVERSE_ZONE_TSIG_KEYNAME = new Property("ddns.reverse.zone.tsig.keyName", "");
            public static Property DDNS_REVERSE_ZONE_TSIG_ALGORITHM = new Property("ddns.reverse.zone.tsig.algorithm", "");
            public static Property DDNS_REVERSE_ZONE_TSIG_KEYDATA = new Property("ddns.reverse.zone.tsig.keyData", "");
            public static Property V4_HEADER_SNAME = new Property("v4.header.sname", "");
            public static Property V4_HEADER_FILENAME = new Property("v4.header.filename", "");
            public static Property V4_IGNORED_MACS = new Property("v4.ignoredMacAddrs", "000000000000, FFFFFFFFFFFF");
            public static Property V4_DEFAULT_LEASETIME = new Property("v4.defaultLeasetime", "3600");
            public static Property V4_PINGCHECK_TIMEOUT = new Property("v4.pingCheckTimeout", "2000");

            public static IEnumerable<Property> Values
            {
                get
                {
                    yield return CHANNEL_THREADPOOL_SIZE;
                    yield return CHANNEL_MAX_CHANNEL_MEMORY;
                    yield return CHANNEL_MAX_TOTAL_MEMORY;
                    yield return CHANNEL_READ_BUFFER_SIZE;
                    yield return CHANNEL_WRITE_BUFFER_SIZE;
                    yield return DATABASE_SCHEMA_TYTPE;
                    yield return DATABASE_SCHEMA_VERSION;
                    yield return DHCP_PROCESSOR_RECENT_MESSAGE_TIMER;
                    yield return DHCP_IGNORE_LOOPBACK;
                    yield return DHCP_IGNORE_LINKLOCAL;
                    yield return DHCP_IGNORE_SELF_PACKETS;
                    yield return BINDING_MANAGER_REAPER_STARTUP_DELAY;
                    yield return BINDING_MANAGER_REAPER_RUN_PERIOD;
                    yield return BINDING_MANAGER_OFFER_EXPIRATION;
                    yield return BINDING_MANAGER_DELETE_OLD_BINDINGS;
                    yield return SEND_REQUESTED_OPTIONS_ONLY;
                    yield return SUPPORT_RAPID_COMMIT;
                    yield return VERIFY_UNKNOWN_REBIND;
                    yield return PREFERRED_LIFETIME;
                    yield return VALID_LIFETIME;
                    yield return IA_NA_T1;
                    yield return IA_NA_T2;
                    yield return IA_PD_T1;
                    yield return IA_PD_T2;
                    yield return DDNS_UPDATE;
                    yield return DDNS_SYNCHRONIZE;
                    yield return DDNS_DOMAIN;
                    yield return DDNS_TTL;
                    yield return DDNS_SERVER;
                    yield return DDNS_TSIG_KEYNAME;
                    yield return DDNS_TSIG_ALGORITHM;
                    yield return DDNS_TSIG_KEYDATA;
                    yield return DDNS_FORWARD_ZONE_NAME;
                    yield return DDNS_FORWARD_ZONE_TTL;
                    yield return DDNS_FORWARD_ZONE_SERVER;
                    yield return DDNS_FORWARD_ZONE_TSIG_KEYNAME;
                    yield return DDNS_FORWARD_ZONE_TSIG_ALGORITHM;
                    yield return DDNS_FORWARD_ZONE_TSIG_KEYDATA;
                    yield return DDNS_REVERSE_ZONE_NAME;
                    yield return DDNS_REVERSE_ZONE_BITLENGTH;
                    yield return DDNS_REVERSE_ZONE_TTL;
                    yield return DDNS_REVERSE_ZONE_SERVER;
                    yield return DDNS_REVERSE_ZONE_TSIG_KEYNAME;
                    yield return DDNS_REVERSE_ZONE_TSIG_ALGORITHM;
                    yield return DDNS_REVERSE_ZONE_TSIG_KEYDATA;
                    yield return V4_HEADER_SNAME;
                    yield return V4_HEADER_FILENAME;
                    yield return V4_IGNORED_MACS;
                    yield return V4_DEFAULT_LEASETIME;
                    yield return V4_PINGCHECK_TIMEOUT;
                }
            }

            /** The key. */
            private string key;

            /** The value. */
            private string value;

            /**
             * Instantiates a new property.
             * 
             * @param key the key
             * @param value the value
             */
            Property(string key, string value)
            {
                this.key = key;
                this.value = value;
            }

            /**
             * Key.
             * 
             * @return the string
             */
            public string Key() { return key; }

            /**
             * Value.
             * 
             * @return the string
             */
            public string Value() { return value; }
        }

        public static bool EffectivePolicyAsBoolean(link link, Property prop)
        {
            return bool.Parse(EffectivePolicy(link, prop));
        }

        /**
         * Effective policy as boolean.
         *
         * @param requestMsg the request msg
         * @param link the link
         * @param prop the prop
         * @return true, if successful
         */
        public static bool EffectivePolicyAsBoolean(DhcpMessage requestMsg,
                link link, Property prop)
        {
            return bool.Parse(EffectivePolicy(requestMsg, link, prop));
        }

        /** The Constant DEFAULT_PROPERTIES. */
        protected static Dictionary<string, string> DEFAULT_PROPERTIES
        {
            get
            {
                Dictionary<string, string> DEFAULT_PROPERTIES = new Dictionary<string, string>();
                IEnumerable<Property> defaultProperties = Property.Values;
                foreach (Property prop in defaultProperties)
                {
                    DEFAULT_PROPERTIES[prop.Key()] = prop.Value();
                }
                return DEFAULT_PROPERTIES;
            }
        }

        protected static Dictionary<string, string> SERVER_PROPERTIES = DEFAULT_PROPERTIES;

        public static string GlobalPolicy(Property prop)
        {

            string policy =
                GetPolicy(new DhcpServerConfiguration().GetDhcpServerConfig().policies,
                        prop.Key());
            if (policy != null)
            {
                return policy;
            }

            return SERVER_PROPERTIES[prop.Key()];
        }

        /**
         * Effective policy as boolean.
         *
         * @param requestMsg the request msg
         * @param pool the pool
         * @param link the link
         * @param prop the prop
         * @return true, if successful
         */
        public static bool EffectivePolicyAsbool(DhcpMessage requestMsg, DhcpConfigObject configObj, link link, Property prop)
        {
            return bool.Parse(EffectivePolicy(requestMsg, configObj, link, prop));
        }

        public static bool GlobalPolicyAsBoolean(Property prop)
        {
            return bool.Parse(GlobalPolicy(prop));
        }

        public static bool EffectivePolicyAsBoolean(DhcpV6Message requestMsg, link link, Property prop)
        {
            return bool.Parse(EffectivePolicy(requestMsg, link, prop));
        }

        /**
         * Effective policy as int.
         *
         * @param requestMsg the request msg
         * @param pool the pool
         * @param link the link
         * @param prop the prop
         * @return the int
         */
        public static int EffectivePolicyAsInt(DhcpMessage requestMsg,
            DhcpConfigObject configObj, link link, Property prop)
        {
            return int.Parse(EffectivePolicy(requestMsg, configObj, link, prop));
        }

        /**
         * Effective policy as float.
         *
         * @param requestMsg the request msg
         * @param pool the pool
         * @param link the link
         * @param prop the prop
         * @return the float
         */
        public static float EffectivePolicyAsFloat(DhcpMessage requestMsg,
            DhcpConfigObject configObj, link link, Property prop)
        {
            return long.Parse(EffectivePolicy(requestMsg, configObj, link, prop));
        }

        /**
       * Effective policy.
       * 
       * @param link the link
       * @param prop the prop
       * 
       * @return the string
       */
        public static string DffectivePolicy(link link, Property prop)
        {
            String policy = GetPolicy(link.policies, prop.Key());
            if (policy != null)
            {
                return policy;
            }
            return GlobalPolicy(prop);
        }

        /**
        * Gets the policy.
        * 
        * @param policies the policies
        * @param name the name
        * 
        * @return the policy
*/
        protected static string GetPolicy(List<Policy> policies, string name)
        {
            if (policies != null)
            {
                foreach (Policy policy in policies)
                {
                    if (policy.name.ToUpper() == name.ToUpper())
                    {
                        return policy.value;
                    }
                }
            }
            return null;
        }

        internal static float EffectivePolicyAsFloat(link link, Property prop)
        {
            return float.Parse(EffectivePolicy(link, prop));
        }

        public static string EffectivePolicy(DhcpMessage requestMsg, link link, Property prop)
        {
            string policy = null;
            if ((requestMsg != null) && (link != null))
            {
                List<linkFilter> linkFilters = link.linkFilters;
                if (linkFilters != null)
                {
                    foreach (linkFilter linkFilter in linkFilters)
                    {

                        if (DhcpServerConfiguration.MsgMatchesFilter(requestMsg, linkFilter))
                        {
                            policy = GetPolicy(linkFilter.policies, prop.Key());
                        }
                    }
                    // if the client request matches at least one filter on the link,
                    // and that filter has configured a value for the policy, then return
                    // that value from the last filter that the client matches
                    if (policy != null)
                    {
                        return policy;
                    }

                }
            }
            if (link != null)
            {
                // client does not match a link filter 
                // get the value of the policy on the link, if any
                policy = GetPolicy(link.policies, prop.Key());
                if (policy != null)
                {
                    return policy;
                }
            }
            return GlobalPolicy(prop);
        }

        public static string EffectivePolicy(DhcpMessage requestMsg,
            DhcpConfigObject configObj, link link, Property prop)
        {
            //string policy = null;
            //if ((requestMsg != null) && (configObj != null))
            //{
            //    if (configObj.GetFilters() != null)
            //    {
            //        List<filter> filters = configObj.GetFilters().filter;
            //        if (filters != null)
            //        {
            //            for (Filter filter : filters)
            //            {
            //                if (DhcpServerConfiguration.msgMatchesFilter(requestMsg, filter))
            //                {
            //                    policy = getPolicy(filter.getPolicies(), prop.key());
            //                }
            //            }
            //            // if the client request matches at least one filter on the pool,
            //            // and that filter has configured a value for the policy, then return
            //            // that value from the last filter that the client matches
            //            if (policy != null)
            //            {
            //                return policy;
            //            }
            //        }
            //    }
            //}
            //if (configObj != null)
            //{
            //    // client does not match a pool filter 
            //    // get the value of the policy on the pool, if any
            //    policy = getPolicy(configObj.getPolicies(), prop.key());
            //    if (policy != null)
            //    {
            //        return policy;
            //    }
            //}
            return EffectivePolicy(requestMsg, link, prop);
        }

        /**
        * Effective policy as boolean.
        * 
        * @param pool the pool
        * @param link the link
        * @param prop the prop
        * 
        * @return true, if successful
        */
        public static bool EffectivePolicyAsBoolean(DhcpConfigObject configObj,
            link link, Property prop)
        {
            return bool.Parse(EffectivePolicy(configObj, link, prop));
        }

        /**
	     * Effective policy.
	     *
	     * @param requestMsg the request msg
	     * @param link the link
	     * @param prop the prop
	     * @return the string
	     */
        public static string EffectivePolicy(DhcpV4Message requestMsg, link link, Property prop)
        {
            string policy = null;
            if ((requestMsg != null) && (link != null))
            {
                List<linkFilter> linkFilters = link.linkFilters;
                if (linkFilters != null)
                {
                    foreach (linkFilter linkFilter in linkFilters)
                    {
                        if (DhcpServerConfiguration.MsgMatchesFilter(requestMsg, linkFilter))
                        {
                            policy = GetPolicy(linkFilter.policies, prop.Key());
                        }
                    }
                    // if the client request matches at least one filter on the link,
                    // and that filter has configured a value for the policy, then return
                    // that value from the last filter that the client matches
                    if (policy != null)
                    {
                        return policy;
                    }

                }
            }
            if (link != null)
            {
                // client does not match a link filter 
                // get the value of the policy on the link, if any
                policy = GetPolicy(link.policies, prop.Key());
                if (policy != null)
                {
                    return policy;
                }
            }
            return GlobalPolicy(prop);
        }

        /**
        * Effective policy.
        * 
        * @param link the link
        * @param prop the prop
        * 
        * @return the string
        */
        public static string EffectivePolicy(link link, Property prop)
        {
            string policy = GetPolicy(link.policies, prop.Key());
            if (policy != null)
            {
                return policy;
            }
            return GlobalPolicy(prop);
        }

        /**
        * Effective policy as long.
        * 
        * @param pool the pool
        * @param link the link
        * @param prop the prop
        * 
        * @return the long
*/
        public static long EffectivePolicyAsLong(DhcpConfigObject configObj,
                link link, Property prop)
        {
            return long.Parse(EffectivePolicy(configObj, link, prop));
        }

        /**
          * Effective policy.
          * 
          * @param pool the pool
          * @param link the link
          * @param prop the prop
          * 
          * @return the string
          */
        public static string EffectivePolicy(DhcpConfigObject configObj, link link, Property prop)
        {
            string policy = null;
            if (configObj != null)
            {
                policy = GetPolicy(configObj.GetPolicies(), prop.Key());
                if (policy != null)
                {
                    return policy;
                }
            }
            if (link != null)
            {
                policy = GetPolicy(link.policies, prop.Key());
                if (policy != null)
                {
                    return policy;
                }
            }
            return GlobalPolicy(prop);
        }

        /**
       * Global policy as long.
       * 
       * @param prop the prop
       * 
       * @return the long
       */
        public static long GlobalPolicyAsLong(Property prop)
        {
            return long.Parse(GlobalPolicy(prop));
        }
        /**
	 * Global policy as int.
	 * 
	 * @param prop the prop
	 * 
	 * @return the int
	 */
        public static int GlobalPolicyAsInt(Property prop)
        {
            return int.Parse(GlobalPolicy(prop));
        }
    }
}
