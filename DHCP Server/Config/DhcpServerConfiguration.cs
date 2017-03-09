using PIXIS.DHCP.DB;
using PIXIS.DHCP.Message;
using PIXIS.DHCP.Option;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Option.V4;
using PIXIS.DHCP.Option.V6;
using PIXIS.DHCP.Request.Bind;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.V4Process;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Config
{
    public class DhcpServerConfiguration
    {
        /** The log. */
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /** The config filename. */
        //public static String configFilename = DhcpServer.DEFAULT_CONFIG_FILENAME;

        /** The XML object representing the configuration. */
        private dhcpServerConfig xmlServerConfig;

        //private DhcpV6ConfigOptions globalMsgConfigOptions;
        //private DhcpV6ConfigOptions globalIaNaConfigOptions;
        //private DhcpV6ConfigOptions globalNaAddrConfigOptions;
        //private DhcpV6ConfigOptions globalIaTaConfigOptions;
        //private DhcpV6ConfigOptions globalTaAddrConfigOptions;
        //private DhcpV6ConfigOptions globalIaPdConfigOptions;
        //private DhcpV6ConfigOptions globalPrefixConfigOptions;
        //private DhcpV4ConfigOptions globalV4ConfigOptions;

        /** The link map. */
        private Dictionary<Subnet, DhcpLink> linkMap;

        private V6NaAddrBindingManager naAddrBindingMgr;
        private V6TaAddrBindingManager taAddrBindingMgr;
        private V6PrefixBindingManager prefixBindingMgr;
        private V4AddrBindingManager v4AddrBindingMgr;
        private IaManager iaMgr;

        public IaManager GetIaMgr()
        {
            return iaMgr;
        }

        public dhcpServerConfig GetDhcpServerConfig()
        {
            //v6ServerIdOption dhcpServerId = new v6ServerIdOption();
            //string hex = "0001000158855f23c48e8ffad74d";
            //// dhcpServerId.opaqueData.hexValue = Encoding.ASCII.GetBytes("0001000158855f23c48e8ffad74d");
            //dhcpServerId.opaqueData.hexValue = Enumerable.Range(0, hex.Length)
            //          .Where(x => x % 2 == 0)
            //          .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
            //          .ToArray();
            //xmlServerConfig.v6ServerIdOption = dhcpServerId;
            return xmlServerConfig;
        }

        public DhcpServerConfiguration()
        {

            xmlServerConfig = new dhcpServerConfig(); //LoadConfig(configFilename);
            linkMap = new Dictionary<Subnet, DhcpLink>();
            if (xmlServerConfig != null)
            {

                InitServerIds();
                //globalMsgConfigOptions = new DhcpV6ConfigOptions(xmlServerConfig.v6MsgConfigOptions);
                //globalIaNaConfigOptions = new DhcpV6ConfigOptions(xmlServerConfig.v6IaNaConfigOptions);
                //globalNaAddrConfigOptions = new DhcpV6ConfigOptions(xmlServerConfig.v6NaAddrConfigOptions);
                //globalIaTaConfigOptions = new DhcpV6ConfigOptions(xmlServerConfig.v6IaTaConfigOptions);
                //globalTaAddrConfigOptions = new DhcpV6ConfigOptions(xmlServerConfig.v6TaAddrConfigOptions);
                //globalIaPdConfigOptions = new DhcpV6ConfigOptions(xmlServerConfig.v6IaPdConfigOptions);
                //globalPrefixConfigOptions = new DhcpV6ConfigOptions(xmlServerConfig.v6PrefixConfigOptions);
                //globalV4ConfigOptions = new DhcpV4ConfigOptions(xmlServerConfig.v4ConfigOptions);
            }
            else
            {
                //throw new Exception("Failed to load configuration file: " + configFilename);
            }
        }

        /**
        * Effective msg options.
        * 
        * @param requestMsg the request msg
        * 
        * @return the map< integer, dhcp option>
        */
        public Dictionary<int, DhcpOption> EffectiveMsgOptions(DhcpV6Message requestMsg)
        {
            Dictionary<int, DhcpOption> optionMap = new Dictionary<int, DhcpOption>();
            Dictionary<int, DhcpOption> filteredOptions =
                FilteredMsgOptions(requestMsg, xmlServerConfig.filters);
            if (filteredOptions != null)
            {
                foreach (var option in filteredOptions)
                    optionMap[option.Key] = option.Value;
            }
            return optionMap;
        }

        public Dictionary<int, DhcpOption> EffectiveIaNaOptions(DhcpV6Message requestMsg)
        {
            Dictionary<int, DhcpOption> optionMap = new Dictionary<int, DhcpOption>();
            //if (globalIaNaConfigOptions != null)
            //{
            //    optionMap.PutAll(globalIaNaConfigOptions.GetDhcpOptionMap());
            //}

            Dictionary<int, DhcpOption> filteredOptions =
                FilteredIaNaOptions(requestMsg, xmlServerConfig.filters);
            if (filteredOptions != null)
            {
                optionMap.PutAll(filteredOptions);
            }
            return optionMap;
        }

        /**
        * Effective ia ta options.
        * 
        * @param requestMsg the request msg
        * @param link the link
        * 
        * @return the map< integer, dhcp option>
        */
        public Dictionary<int, DhcpOption> EffectiveMsgOptions(DhcpV6Message requestMsg, DhcpLink dhcpLink)
        {
            Dictionary<int, DhcpOption> optionMap = EffectiveMsgOptions(requestMsg);
            if ((dhcpLink != null) && (dhcpLink.GetLink() != null))
            {
                DhcpV6ConfigOptions configOptions = dhcpLink.GetMsgConfigOptions();
                if (configOptions != null)
                {
                    optionMap.PutAll(configOptions.GetDhcpOptionMap());
                }

                Dictionary<int, DhcpOption> filteredOptions =
                    FilteredMsgOptions(requestMsg, dhcpLink.GetLink().linkFilters.ToList<filter>());
                if (filteredOptions != null)
                {
                    optionMap.PutAll(filteredOptions);
                }
            }
            return optionMap;
        }

        public Dictionary<int, DhcpOption> EffectiveIaTaOptions(DhcpV6Message requestMsg, DhcpLink dhcpLink)
        {
            Dictionary<int, DhcpOption> optionMap = EffectiveIaTaOptions(requestMsg);
            if ((dhcpLink != null) && (dhcpLink.GetLink() != null))
            {
                DhcpV6ConfigOptions configOptions = dhcpLink.GetIaTaConfigOptions();
                if (configOptions != null)
                {
                    optionMap.PutAll(configOptions.GetDhcpOptionMap());
                }

                Dictionary<int, DhcpOption> filteredOptions =
                   FilteredIaTaOptions(requestMsg, dhcpLink.GetLink().linkFilters.ToList<filter>());
                if (filteredOptions != null)
                {
                    optionMap.PutAll(filteredOptions);
                }
            }
            return optionMap;
        }
        /**
	 * Filtered ia ta options.
	 * 
	 * @param requestMsg the request msg
	 * @param filters the filters
	 * 
	 * @return the map< integer, dhcp option>
	 */
        private Dictionary<int, DhcpOption> FilteredIaTaOptions(DhcpV6Message requestMsg,
                List<filter> filters)
        {
            if ((filters != null) && filters.Count > 0)
            {
                foreach (filter filter in filters)
                {
                    if (MsgMatchesFilter(requestMsg, filter))
                    {
                        log.Info("Request matches filter: " + filter.name);
                        DhcpV6ConfigOptions filterConfigOptions =
                            new DhcpV6ConfigOptions(filter.v6TaAddrConfigOptions);
                        if (filterConfigOptions != null)
                        {
                            return filterConfigOptions.GetDhcpOptionMap();
                        }
                    }
                }
            }
            return null;
        }

        public Dictionary<int, DhcpOption> EffectiveIaTaOptions(DhcpV6Message requestMsg)
        {
            Dictionary<int, DhcpOption> optionMap = new Dictionary<int, DhcpOption>();
            //if (globalIaTaConfigOptions != null)
            //{
            //    optionMap.PutAll(globalIaTaConfigOptions.GetDhcpOptionMap());
            //}

            Dictionary<int, DhcpOption> filteredOptions =
                FilteredIaTaOptions(requestMsg, xmlServerConfig.filters);
            if (filteredOptions != null)
            {
                optionMap.PutAll(filteredOptions);
            }
            return optionMap;
        }

        public Dictionary<int, DhcpOption> EffectiveIaPdOptions(DhcpV6Message requestMsg, DhcpLink dhcpLink)
        {
            Dictionary<int, DhcpOption> optionMap = new Dictionary<int, DhcpOption>();
            //if (globalIaPdConfigOptions != null)
            //{
            //    optionMap.PutAll(globalIaPdConfigOptions.GetDhcpOptionMap());
            //}

            Dictionary<int, DhcpOption> filteredOptions =
                FilteredIaPdOptions(requestMsg, xmlServerConfig.filters);
            if (filteredOptions != null)
            {
                optionMap.PutAll(filteredOptions);
            }
            return optionMap;
        }

        /**
	 * Filtered ia pd options.
	 * 
	 * @param requestMsg the request msg
	 * @param filters the filters
	 * 
	 * @return the map< integer, dhcp option>
	 */
        private Dictionary<int, DhcpOption> FilteredIaPdOptions(DhcpV6Message requestMsg,
                List<filter> filters)
        {
            if ((filters != null) && filters.Count > 0)
            {
                foreach (filter filter in filters)
                {
                    if (MsgMatchesFilter(requestMsg, filter))
                    {
                        log.Info("Request matches filter: " + filter.name);
                        DhcpV6ConfigOptions filterConfigOptions =
                            new DhcpV6ConfigOptions(filter.v6IaPdConfigOptions);
                        if (filterConfigOptions != null)
                        {
                            return filterConfigOptions.GetDhcpOptionMap();
                        }
                    }
                }
            }
            return null;
        }

        /**
        * Effective ia pd options.
        * 
        * @param requestMsg the request msg
        * 
        * @return the map< integer, dhcp option>
        */
        public Dictionary<int, DhcpOption> EffectiveIaPdOptions(DhcpV6Message requestMsg)
        {
            Dictionary<int, DhcpOption> optionMap = new Dictionary<int, DhcpOption>();
            //if (globalIaPdConfigOptions != null)
            //{
            //    optionMap.PutAll(globalIaPdConfigOptions.GetDhcpOptionMap());
            //}

            Dictionary<int, DhcpOption> filteredOptions =
                FilteredIaPdOptions(requestMsg, xmlServerConfig.filters);
            if (filteredOptions != null)
            {
                optionMap.PutAll(filteredOptions);
            }
            return optionMap;
        }

        public Dictionary<int, DhcpOption> EffectiveNaAddrOptions(DhcpV6Message requestMsg)
        {
            Dictionary<int, DhcpOption> optionMap = new Dictionary<int, DhcpOption>();
            //if ((globalNaAddrConfigOptions != null))
            //{
            //    optionMap.PutAll(globalNaAddrConfigOptions.GetDhcpOptionMap());
            //}

            Dictionary<int, DhcpOption> filteredOptions = FilteredNaAddrOptions(requestMsg, xmlServerConfig.filters);
            if ((filteredOptions != null))
            {
                optionMap.PutAll(filteredOptions);
            }

            return optionMap;
        }

        public V6NaAddrBindingManager GetNaAddrBindingMgr()
        {
            return naAddrBindingMgr;
        }


        /**
         * Effective ta addr options.
         * 
         * @param requestMsg the request msg
         * 
         * @return the map< integer, dhcp option>
         */
        public Dictionary<int, DhcpOption> EffectiveTaAddrOptions(DhcpV6Message requestMsg)
        {
            Dictionary<int, DhcpOption> optionMap = new Dictionary<int, DhcpOption>();
            //if (globalTaAddrConfigOptions != null)
            //{
            //    optionMap.PutAll(globalTaAddrConfigOptions.GetDhcpOptionMap());
            //}

            Dictionary<int, DhcpOption> filteredOptions =
                FilteredNaAddrOptions(requestMsg, xmlServerConfig.filters);
            if (filteredOptions != null)
            {
                optionMap.PutAll(filteredOptions);
            }
            return optionMap;
        }

        /**
        * Effective ia na options.
        * 
        * @param requestMsg the request msg
        * @param link the link
        * 
        * @return the map< integer, dhcp option>
        */
        public Dictionary<int, DhcpOption> EffectiveIaNaOptions(DhcpV6Message requestMsg, DhcpLink dhcpLink)
        {
            Dictionary<int, DhcpOption> optionMap = EffectiveIaNaOptions(requestMsg);
            if ((dhcpLink != null) && (dhcpLink.GetLink() != null))
            {
                DhcpV6ConfigOptions configOptions = dhcpLink.GetIaNaConfigOptions();
                if (configOptions != null)
                {
                    optionMap.PutAll(configOptions.GetDhcpOptionMap());
                }

                Dictionary<int, DhcpOption> filteredOptions =
                    FilteredIaNaOptions(requestMsg, dhcpLink.GetLink().linkFilters.ToList<filter>());
                if (filteredOptions != null)
                {
                    optionMap.PutAll(filteredOptions);
                }
            }
            return optionMap;
        }

        public Dictionary<int, DhcpOption> EffectivePrefixOptions(DhcpV6Message requestMsg)
        {
            Dictionary<int, DhcpOption> optionMap = new Dictionary<int, DhcpOption>();
            //if (globalPrefixConfigOptions != null)
            //{
            //    optionMap.PutAll(globalPrefixConfigOptions.GetDhcpOptionMap());
            //}

            Dictionary<int, DhcpOption> filteredOptions =
                FilteredPrefixOptions(requestMsg, xmlServerConfig.filters);
            if (filteredOptions != null)
            {
                optionMap.PutAll(filteredOptions);
            }
            return optionMap;
        }


        public Dictionary<int, DhcpOption> EffectiveTaAddrOptions(DhcpV6Message requestMsg, DhcpLink dhcpLink)
        {
            Dictionary<int, DhcpOption> optionMap = EffectiveTaAddrOptions(requestMsg);
            if ((dhcpLink != null) && (dhcpLink != null))
            {
                DhcpV6ConfigOptions configOptions = dhcpLink.GetTaAddrConfigOptions();
                if (configOptions != null)
                {
                    optionMap.PutAll(configOptions.GetDhcpOptionMap());
                }

                Dictionary<int, DhcpOption> filteredOptions =
                    FilteredTaAddrOptions(requestMsg, dhcpLink.GetLink().linkFilters.ToList<filter>());
                if (filteredOptions != null)
                {
                    optionMap.PutAll(filteredOptions);
                }
            }
            return optionMap;
        }


        /**
         * Filtered ta addr options.
         * 
         * @param requestMsg the request msg
         * @param filters the filters
         * 
         * @return the map< integer, dhcp option>
         */
        private Dictionary<int, DhcpOption> FilteredTaAddrOptions(DhcpV6Message requestMsg,
                List<filter> filters)
        {
            if ((filters != null) && filters.Count > 0)
            {
                foreach (filter filter in filters)
                {
                    if (MsgMatchesFilter(requestMsg, filter))
                    {
                        log.Info("Request matches filter: " + filter.name);
                        DhcpV6ConfigOptions filterConfigOptions =
                            new DhcpV6ConfigOptions(filter.v6TaAddrConfigOptions);
                        if (filterConfigOptions != null)
                        {
                            return filterConfigOptions.GetDhcpOptionMap();
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Find dhcp link.
        /// </summary>
        /// <param name="local">local the local v4 address</param>
        /// <param name="remote">remote the remote v4 address</param>
        /// <returns>dhcp link</returns>
        public DhcpLink FindDhcpLinkV4(IPAddress local, IPAddress remote)
        {
            DhcpLink link = null;
            if ((linkMap != null) && linkMap.Count > 0)
            {
                if (remote.Equals(DhcpConstants.ZEROADDR_V4))
                {
                    // if the remote address is zero, then the request was received
                    // from a client without an address on the broadcast channel, so
                    // use the local address to search the linkMap
                    log.Debug("Looking for Link by local address: " + local.ToString());
                    link = FindLink(local);
                }
                else
                {
                    // if the remote address is non-zero, then the request was received
                    // from a client or relay with that address, so use the remote address
                    // to search the linkMap
                    log.Debug("Looking for Link by remote address: " + remote.ToString());
                    link = FindLink(remote);
                }
            }
            else
            {
                log.Error("linkMap is null or empty");
            }
            if (link != null)
            {
                log.Info("Found configured Link for client request: " +
                            link.GetLink().name);
            }
            return link;
        }

        /// <summary>
        /// Find dhcp link.
        /// </summary>
        /// <param name="local">the local v6 address</param>
        /// <param name="remote">the remote v6 address</param>
        /// <returns>the dhcp link</returns>
        public DhcpLink FindDhcpLinkV6(IPAddress local, IPAddress remote)
        {
            DhcpLink link = null;
            if (linkMap != null && linkMap.Count >= 0)
            {
                if (local.IsIPv6LinkLocal)
                {
                    // if the local address is link-local, then the request
                    // was received directly from the client on the interface
                    // with that link-local address, which is the linkMap key
                    log.Debug("Looking for Link by link local address: " + local.ToString());
                    //Subnet s = new Subnet(local, 128);

                    link = linkMap.Where(l => l.Key.GetSubnetAddress().ToString() == local.ToString() && l.Key.GetPrefixLength() == 128).First().Value;
                }
                else if (!remote.IsIPv6LinkLocal)
                {
                    // if the remote (client) address is not link-local, then the client
                    // already has an address, so use that address to search the linkMap
                    log.Debug("Looking for Link by remote global address: " + remote.ToString());
                    link = FindLink(remote);
                }
                else
                {
                    // if the local address is not link-local, and the remote
                    // address is link-local, then this message was relayed and
                    // the local address is the client link address
                    log.Debug("Looking for Link by address: " + local.ToString());
                    link = FindLink(local);
                }
            }
            else
            {
                log.Error("linkMap is null or empty");
            }
            if (link != null)
            {
                log.Info("Found configured Link for client request: " +
                            link.GetLink().name);
            }
            return link;
        }
        public Dictionary<int, DhcpOption> EffectivePrefixOptions(DhcpV6Message requestMsg, DhcpLink dhcpLink)
        {
            Dictionary<int, DhcpOption> optionMap = EffectivePrefixOptions(requestMsg);
            if ((dhcpLink != null) && (dhcpLink != null))
            {
                DhcpV6ConfigOptions configOptions = dhcpLink.GetPrefixConfigOptions();
                if (configOptions != null)
                {
                    optionMap.PutAll(configOptions.GetDhcpOptionMap());
                }

                Dictionary<int, DhcpOption> filteredOptions =
                    FilteredPrefixOptions(requestMsg, dhcpLink.GetLink().linkFilters.ToList<filter>());
                if (filteredOptions != null)
                {
                    optionMap.PutAll(filteredOptions);
                }
            }
            return optionMap;
        }

        public Dictionary<int, DhcpOption> EffectiveNaAddrOptions(DhcpV6Message requestMsg, DhcpLink dhcpLink, DhcpV6OptionConfigObject configObj)
        {
            Dictionary<int, DhcpOption> optionMap = new Dictionary<int, DhcpOption>();
            //if (globalNaAddrConfigOptions != null)
            //{
            //    foreach (var option in globalNaAddrConfigOptions.GetDhcpOptionMap())
            //        optionMap[option.Key] = option.Value;
            //}

            Dictionary<int, DhcpOption> filteredOptions =
                FilteredNaAddrOptions(requestMsg, xmlServerConfig.filters);
            if (filteredOptions != null)
            {
                foreach (var option in filteredOptions)
                    optionMap[option.Key] = option.Value;
            }
            return optionMap;
        }

        public Dictionary<int, DhcpOption> EffectiveTaAddrOptions(DhcpV6Message requestMsg, DhcpLink dhcpLink, DhcpV6OptionConfigObject configObj)
        {
            Dictionary<int, DhcpOption> optionMap = EffectiveNaAddrOptions(requestMsg, dhcpLink);
            if (configObj != null)
            {
                DhcpV6ConfigOptions configOptions = configObj.GetDhcpConfigOptions();
                if (configOptions != null)
                {
                    optionMap.PutAll(configOptions.GetDhcpOptionMap());
                }

                Dictionary<int, DhcpOption> filteredOptions =
                    FilteredTaAddrOptions(requestMsg, configObj.GetFilters());
                if (filteredOptions != null)
                {
                    optionMap.PutAll(filteredOptions);
                }
            }
            return optionMap;
        }

        public Dictionary<int, DhcpOption> EffectivePrefixOptions(DhcpV6Message requestMsg, DhcpLink dhcpLink, DhcpV6OptionConfigObject configObj)
        {
            Dictionary<int, DhcpOption> optionMap = EffectivePrefixOptions(requestMsg, dhcpLink);
            if (configObj != null)
            {
                DhcpV6ConfigOptions configOptions = configObj.GetDhcpConfigOptions();
                if (configOptions != null)
                {
                    optionMap.PutAll(configOptions.GetDhcpOptionMap());
                }

                Dictionary<int, DhcpOption> filteredOptions =
                    FilteredPrefixOptions(requestMsg, configObj.GetFilters());
                if (filteredOptions != null)
                {
                    optionMap.PutAll(filteredOptions);
                }
            }
            return optionMap;
        }
        public void SetTaAddrBindingMgr(V6TaAddrBindingManager taAddrBindingMgr)
        {
            this.taAddrBindingMgr = taAddrBindingMgr;
        }

        public V6TaAddrBindingManager GetTaAddrBindingMgr()
        {
            return taAddrBindingMgr;
        }

        public Dictionary<int, DhcpOption> EffectiveNaAddrOptions(DhcpV6Message requestMsg, DhcpLink dhcpLink)
        {
            Dictionary<int, DhcpOption> optionMap = EffectiveNaAddrOptions(requestMsg);
            if (((dhcpLink != null)
                        && (dhcpLink.GetLink() != null)))
            {
                DhcpV6ConfigOptions configOptions = dhcpLink.GetNaAddrConfigOptions();
                if ((configOptions != null))
                {
                    optionMap.PutAll(configOptions.GetDhcpOptionMap());
                }

                Dictionary<int, DhcpOption> filteredOptions = FilteredNaAddrOptions(requestMsg, dhcpLink.GetLink().linkFilters);
                if ((filteredOptions != null))
                {
                    optionMap.PutAll(filteredOptions);
                }

            }

            return optionMap;
        }

        public V6PrefixBindingManager GetPrefixBindingMgr()
        {
            return prefixBindingMgr;
        }
        public void SetV6ServerIdOption(v6ServerIdOption option)
        {
            xmlServerConfig.v6ServerIdOption = option;
        }
        public void SetV4ServerIdOption(v4ServerIdOption option)
        {
            xmlServerConfig.v4ServerIdOption = option;
        }
        protected void InitServerIds()
        {
            v6ServerIdOption v6ServerId = xmlServerConfig.v6ServerIdOption;
            if ((v6ServerId == null))
            {
                v6ServerId = new v6ServerIdOption();
            }

            opaqueData opaque = v6ServerId.opaqueData;

            opaqueData duid = OpaqueDataUtil.GenerateDUID_LLT();
            if ((duid == null))
            {
                throw new Exception("Failed to create ServerID");
            }

            v6ServerId.opaqueData = duid;
            xmlServerConfig.v6ServerIdOption = v6ServerId;
            //SaveConfig(xmlServerConfig, configFilename);


            //v4ServerIdOption v4ServerId = xmlServerConfig.v4ServerIdOption;
            //if ((v4ServerId == null))
            //{
            //    v4ServerId = new v4ServerIdOption();
            //}

            //String ip = v4ServerId.ipAddress;
            //if (((ip == null)
            //            || (ip.Length <= 0)))
            //{
            //    v4ServerId.ipAddress = GetIPAddress();
            //    xmlServerConfig.v4ServerIdOption = v4ServerId;
            //    //SaveConfig(xmlServerConfig, configFilename);
            //}

        }


        public static bool MsgMatchesFilter(DhcpMessage requestMsg, linkFilter filter)
        {
            bool matches = true;     // assume match

            List<filterExpression> expressions = filter.filterExpressions;
            if (expressions != null)
            {
                foreach (filterExpression expression in expressions)
                {
                    if (expression.clientClassExpression != null)
                    {
                        matches = MsgMatchesClientClass(requestMsg, expression.clientClassExpression);
                        if (!matches)
                            break;
                    }
                    else if (expression.optionExpression != null)
                    {
                        optionExpression optexpr = expression.optionExpression;
                        DhcpOption option = requestMsg.GetDhcpOption(optexpr.code);
                        if (option != null)
                        {
                            // found the filter option in the request,
                            // so check if the expression matches
                            if (!EvaluateExpression(optexpr, option))
                            {
                                // it must match all expressions for the filter
                                // group (i.e. expressions are ANDed), so if
                                // just one doesn't match, then we're done
                                matches = false;
                                break;
                            }
                        }
                        else
                        {
                            // if the expression option wasn't found in the
                            // request message, then it can't match
                            matches = false;
                            break;
                        }
                    }
                    else
                    {
                        log.Warn("Unsupported filter expression: " + expression);
                    }
                }
            }
            return matches;
        }

        public void SetLinkMap(Dictionary<Subnet, DhcpLink> map)
        {
            this.linkMap = map;
        }
        public void AddOrUpdateLinkMap(Subnet subnet, DhcpLink link)
        {
            this.linkMap[subnet] = link;
        }
        public void DeleteLinkMap(Subnet s)
        {
            if (linkMap.ContainsKey(s)) this.linkMap.Remove(s);
        }
        public void RemoveLink(Subnet subnet)
        {
            var link = this.linkMap.Get(subnet);
            if (link == null)
            {
                return;
            }
            this.linkMap.Remove(subnet);
        }

        public Dictionary<Subnet, DhcpLink> GetLinkMap()
        {
            return linkMap;
        }

        public void SetNaAddrBindingMgr(V6NaAddrBindingManager naAddrBindingMgr)
        {
            this.naAddrBindingMgr = naAddrBindingMgr;
        }

        public void SetPrefixBindingMgr(V6PrefixBindingManager prefixBindingMgr)
        {
            this.prefixBindingMgr = prefixBindingMgr;
        }

        public V4AddrBindingManager GetV4AddrBindingMgr()
        {
            return v4AddrBindingMgr;
        }

        public void SetV4AddrBindingMgr(V4AddrBindingManager v4AddrBindingMgr)
        {
            this.v4AddrBindingMgr = v4AddrBindingMgr;
        }

        public void InitV4AddrBindingManager()
        {
            v4AddrBindingMgr.UpdatePool();
        }
        public void InitV6AddrBindingManager()
        {
            naAddrBindingMgr.UpdatePool();
        }
        public DhcpLink FindLinkForAddress(IPAddress inetAddr)
        {
            if (inetAddr.AddressFamily == AddressFamily.InterNetworkV6)
            {
                if (((linkMap != null)
                            && linkMap.Count > 0))
                {
                    foreach (DhcpLink link in linkMap.Values)
                    {
                        List<v6AddressPool> addrPools = link.GetLink().v6NaAddrPools;

                        if ((addrPools != null))
                        {
                            foreach (v6AddressPool addrPool in addrPools)
                            {
                                try
                                {
                                    Range range = new Range(addrPool.range);
                                    if (range.Contains(inetAddr))
                                    {
                                        return link;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    log.Error(("Invalid AddressPool range: "
                                                    + (addrPool.range + (": " + ex))));
                                }
                            }

                        }

                        addrPools = link.GetLink().v6TaAddrPools;
                        if ((addrPools != null))
                        {
                            foreach (v6AddressPool addrPool in addrPools)
                            {
                                try
                                {
                                    Range range = new Range(addrPool.range);
                                    if (range.Contains(inetAddr))
                                    {
                                        return link;
                                    }

                                }
                                catch (Exception ex)
                                {
                                    log.Error(("Invalid AddressPool range: "
                                                    + (addrPool.range + (": " + ex))));
                                }

                            }



                        }

                    }

                }

            }
            else if (((linkMap != null)
                        && linkMap.Count > 0))
            {
                foreach (DhcpLink link in linkMap.Values)
                {
                    List<v4AddressPool> addrPools = link.GetLink().v4AddrPools;
                    if ((addrPools != null))
                    {
                        foreach (v4AddressPool addrPool in addrPools)
                        {
                            try
                            {
                                Range range = new Range(addrPool.range);
                                if (range.Contains(inetAddr))
                                {
                                    return link;
                                }

                            }
                            catch (Exception ex)
                            {
                                log.Error(("Invalid V4AddressPool range: "
                                                + (addrPool.range + (": " + ex))));
                            }

                        }
                    }

                }

            }

            return null;
        }


        private DhcpLink FindLink(IPAddress addr)
        {
            //#warning "尚有程式待確認"

            //Subnet s = null;
            //if (addr.IsV6IP())
            //{
            //    s = new Subnet(addr, 128);
            //}
            //else
            //{
            //    s = new Subnet(addr, 32);
            //}
            //foreach (var link in linkMap)
            //{
            //    if (link.Key.Contains(addr))
            //        return link.Value;
            //}

            //  find links less than the given address
            //SortedMap<Subnet, DhcpLink> subMap = linkMap.headMap(s);
            //if ((subMap != null) && !subMap.isEmpty())
            //{
            //    // the last one in the sub map should contain the address
            //    Subnet k = subMap.lastKey();
            //    if (k.contains(addr))
            //    {
            //        return subMap.get(k);
            //    }
            //}
            //// find links greater or equal to given address
            //subMap = linkMap.tailMap(s);
            //if ((subMap != null) && !subMap.isEmpty())
            //{
            //    // the first one in the sub map should contain the address
            //    Subnet k = subMap.firstKey();
            //    if (k.contains(addr))
            //    {
            //        return subMap.get(k);
            //    }
            //}

            return null;
        }
        public Dictionary<Subnet, DhcpLink> Tail(Dictionary<Subnet, DhcpLink> list, Subnet key)
        {
            Dictionary<Subnet, DhcpLink> temp = new Dictionary<Subnet, DhcpLink>();
            foreach (var item in list)
            {
                if (key.CompareTo(item.Key) > 0)
                    temp.Add(item.Key, item.Value);
            }

            return temp;
        }
        public Dictionary<Subnet, DhcpLink> Head(Dictionary<Subnet, DhcpLink> list, Subnet key)
        {
            Dictionary<Subnet, DhcpLink> temp = new Dictionary<Subnet, DhcpLink>();
            foreach (var item in list)
            {
                if (key.CompareTo(item.Key) <= 0)
                    temp.Add(item.Key, item.Value);
            }

            return temp;
        }

        public static dhcpServerConfig LoadConfig(string filename)
        {
            //    log.info(("Loading server configuration file: " + filename));
            dhcpServerConfig config = ParseConfig(filename);
            //    if ((config != null))
            //    {
            //        log.info("Server configuration file loaded.");
            //    }
            //    else
            //    {
            //        log.error("No server configuration loaded.");
            //    }

            return config;
        }

        public static dhcpServerConfig ParseConfig(String filename)
        {
            dhcpServerConfig config = null;
            //    FileInputStream fis = null;
            //    try
            //    {
            //        fis = new FileInputStream(filename);
            //        config = DhcpServerConfigDocument.Factory.parse(fis).getDhcpServerConfig();
            //        ArrayList<XmlValidationError> validationErrors = new ArrayList<XmlValidationError>();
            //        XmlOptions validationOptions = new XmlOptions();
            //        validationOptions.setErrorListener(validationErrors);
            //        //  During validation, errors are added to the ArrayList
            //        bool isValid = config.validate(validationOptions);
            //        if (!isValid)
            //        {
            //            StringBuilder sb = new StringBuilder();
            //            Iterator<XmlValidationError> iter = validationErrors.iterator();
            //            while (iter.hasNext())
            //            {
            //                sb.append(iter.next());
            //                sb.append('\n');
            //            }

            //            throw new DhcpServerConfigException(sb.toString());
            //        }

            //    }
            //    finally
            //    {
            //        if ((fis != null))
            //        {
            //            fis.close();
            //        }

            //    }

            return config;
        }

        //public static void SaveConfig(dhcpServerConfig config, String filename)
        //{
        //    FileOutputStream fos = null;
        //    try
        //    {
        //        log.info(("Saving server configuration file: " + filename));
        //        fos = new FileOutputStream(filename);
        //        DhcpServerConfigDocument doc = DhcpServerConfigDocument.Factory.newInstance();
        //        doc.setDhcpServerConfig(config);
        //        doc.save(fos, (new XmlOptions() + setSavePrettyPrint().setSavePrettyPrintIndent(4)));
        //        log.info("Server configuration file saved.");
        //    }
        //    finally
        //    {
        //        if ((fos != null))
        //        {
        //            fos.close();
        //        }

        //    }

        //}


        private Dictionary<int, DhcpOption> FilteredMsgOptions(DhcpV6Message requestMsg, List<filter> filters)
        {
            if (((filters != null)
                        && filters.Count > 0))
            {
                foreach (filter filter in filters)
                {
                    if (MsgMatchesFilter(requestMsg, filter))
                    {
                        log.Info(("Request matches filter: " + filter.name));
                        DhcpV6ConfigOptions filterConfigOptions = new DhcpV6ConfigOptions(filter.v6MsgConfigOptions);
                        if ((filterConfigOptions != null))
                        {
                            return filterConfigOptions.GetDhcpOptionMap();
                        }

                    }

                }

            }

            return null;
        }

        private Dictionary<int, DhcpOption> FilteredIaNaOptions(DhcpV6Message requestMsg, List<filter> filters)
        {
            if (((filters != null)
                        && filters.Count > 0))
            {
                foreach (filter filter in filters)
                {
                    if (MsgMatchesFilter(requestMsg, filter))
                    {
                        log.Info(("Request matches filter: " + filter.name));
                        DhcpV6ConfigOptions filterConfigOptions = new DhcpV6ConfigOptions(filter.v6IaNaConfigOptions);
                        if ((filterConfigOptions != null))
                        {
                            return filterConfigOptions.GetDhcpOptionMap();
                        }

                    }

                }

            }

            return null;
        }

        protected Dictionary<int, DhcpOption> FilteredNaAddrOptions(DhcpV6Message requestMsg, List<linkFilter> linkFilters)
        {
            return FilteredNaAddrOptions(requestMsg, linkFilters);
        }

        /**
        * Filtered na addr options.
        * 
        * @param requestMsg the request msg
        * @param filters the filters
        * 
        * @return the map< integer, dhcp option>
        */
        private Dictionary<int, DhcpOption> FilteredNaAddrOptions(DhcpV6Message requestMsg, List<filter> filters)
        {
            if (((filters != null)
                        && filters.Count > 0))
            {
                foreach (filter filter in filters)
                {
                    if (MsgMatchesFilter(requestMsg, filter))
                    {
                        log.Info(("Request matches filter: " + filter.name));
                        DhcpV6ConfigOptions filterConfigOptions = new DhcpV6ConfigOptions(filter.v6NaAddrConfigOptions);
                        if ((filterConfigOptions != null))
                        {
                            return filterConfigOptions.GetDhcpOptionMap();
                        }

                    }

                }

            }

            return null;
        }

        private Dictionary<int, DhcpOption> FilteredPrefixOptions(DhcpV6Message requestMsg, List<filter> filters)
        {
            if (((filters != null)
                        && filters.Count > 0))
            {
                foreach (filter filter in filters)
                {
                    if (MsgMatchesFilter(requestMsg, filter))
                    {
                        log.Info(("Request matches filter: " + filter.name));
                        DhcpV6ConfigOptions filterConfigOptions = new DhcpV6ConfigOptions(filter.v6PrefixConfigOptions);
                        if ((filterConfigOptions != null))
                        {
                            return filterConfigOptions.GetDhcpOptionMap();
                        }

                    }

                }

            }

            return null;
        }

        public static bool MsgMatchesFilter(DhcpMessage requestMsg, filter filter)
        {
            bool matches = true;
            //  assume match
            List<filterExpression> expressions = filter.filterExpressions;
            if ((expressions != null))
            {
                foreach (filterExpression expression in expressions)
                {
                    if ((expression.clientClassExpression != null))
                    {
                        matches = MsgMatchesClientClass(requestMsg, expression.clientClassExpression);
                        if (!matches)
                        {
                            break;
                        }

                    }
                    else if ((expression.optionExpression != null))
                    {
                        optionExpression optexpr = expression.optionExpression;
                        DhcpOption option = requestMsg.GetDhcpOption(optexpr.code);
                        if ((option != null))
                        {
                            //  found the filter option in the request,
                            //  so check if the expression matches
                            if (!EvaluateExpression(optexpr, option))
                            {
                                //  it must match all expressions for the filter
                                //  group (i.e. expressions are ANDed), so if
                                //  just one doesn't match, then we're done
                                matches = false;
                                break;
                            }

                        }
                        else
                        {
                            //  if the expression option wasn't found in the
                            //  request message, then it can't match
                            matches = false;
                            break;
                        }

                    }
                    else
                    {
                        log.Warn(("Unsupported filter expression: " + expression));
                    }

                }

            }



            return matches;
        }

        protected static bool EvaluateExpression(optionExpression expression, DhcpOption option)
        {
            bool matches = false;
            if ((option is DhcpComparableOption))
            {
                if (option.IsV4() == expression.v4)
                {
                    matches = ((DhcpComparableOption)(option)).Matches(expression);
                }

            }
            else
            {
                log.Error(("Configured option expression is not comparable:" + (" code=" + expression.code)));
            }

            return matches;
        }

        /**
     * Find the TA address pool for an address on a link
     * 
     * @param link the link
     * @param addr the addr
     * 
     * @return the pool
     */
        public static v6AddressPool FindTaAddrPool(link link, IPAddress addr)
        {
            v6AddressPool pool = null;
            if (link != null)
            {
                pool = FindAddrPool(link.v6TaAddrPools, addr);
                if (pool == null)
                {
                    pool = FindAddrPool(link.v6TaAddrPools, addr);
                }
            }
            return pool;
        }

        /**
    * Find the address pool for an address by type
    * 
    * @param poolsType the pool type
    * @param addr the addr
    * @return the pool
    */
        private static v6AddressPool FindAddrPool(List<v6AddressPool> pools, IPAddress addr)
        {
            v6AddressPool pool = null;
            if ((pools != null) && pools.Count > 0)
            {
                foreach (v6AddressPool p in pools)
                {
                    try
                    {
                        Range r = new Range(p.range);
                        if (r.Contains(addr))
                        {
                            pool = p;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        // this can't happen because the parsing of the
                        // pool Ranges is done at startup which would cause abort
                        log.Error("Invalid Pool Range: " + p.range + ": " + ex);
                        //TODO										throw ex;
                    }
                }
            }
            return pool;
        }

        /**
         * Find the prefix pool for an address on a link
         * 
         * @param link the link
         * @param addr the addr
         * 
         * @return the pool
         */
        public static v6PrefixPool FindPrefixPool(link link, IPAddress addr)
        {
            v6PrefixPool pool = null;
            if (link != null)
            {
                pool = FindPrefixPool(link.v6PrefixPools, addr);
                if (pool == null)
                {
                    pool = FindPrefixPool(link.v6PrefixPools, addr);
                }
            }
            return pool;
        }

        /**
         * Find the prefix pool for an address by type
         * 
         * @param poolsType the pool type
         * @param addr the addr
         * @return the pool
         */
        public static v6PrefixPool FindPrefixPool(List<v6PrefixPool> pools, IPAddress addr)
        {
            v6PrefixPool pool = null;
            if ((pools != null) && pools.Count > 0)
            {
                foreach (v6PrefixPool p in pools)
                {
                    try
                    {
                        Range r = new Range(p.range);
                        if (r.Contains(addr))
                        {
                            pool = p;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        // this can't happen because the parsing of the
                        // pool Ranges is done at startup which would cause abort
                        log.Error("Invalid Pool Range: " + p.range + ": " + ex);
                        //TODO										throw ex;
                    }

                }
            }
            return pool;
        }

        protected static bool MsgMatchesClientClass(DhcpMessage requestMsg, clientClassExpression ccexpr)
        {
            if ((ccexpr.v6UserClassOption != null))
            {
                DhcpV6UserClassOption ucOption = ((DhcpV6UserClassOption)(requestMsg.GetDhcpOption(ccexpr.v6VendorClassOption.code)));
                if ((ucOption != null))
                {
                    DhcpV6UserClassOption exprOption = new DhcpV6UserClassOption(ccexpr.v6UserClassOption);
                    if (!ucOption.Matches(exprOption, ccexpr.@operator))
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }

            }
            else if ((ccexpr.v6VendorClassOption != null))
            {
                DhcpV6VendorClassOption vcOption = ((DhcpV6VendorClassOption)(requestMsg.GetDhcpOption(ccexpr.v6VendorClassOption.code)));
                if ((vcOption != null))
                {
                    DhcpV6VendorClassOption exprOption = new DhcpV6VendorClassOption(ccexpr.v6VendorClassOption);
                    if (!vcOption.Matches(exprOption, ccexpr.@operator))
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }

            }
            else if ((ccexpr.v6VendorClassOption != null))
            {
                DhcpV4VendorClassOption vcOption = ((DhcpV4VendorClassOption)(requestMsg.GetDhcpOption(ccexpr.v6VendorClassOption.code)));
                if ((vcOption != null))
                {
                    if (!vcOption.Matches(ccexpr.v4VendorClassOption, ccexpr.@operator))
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }

            }
            else
            {
                log.Warn(("Unsupported client class expression: " + ccexpr));
            }

            return true;
        }
        /**
     * Effective v4 addr options.
     * 
     * @param requestMsg the request msg
     * 
     * @return the map< integer, dhcp option>
     */
        public Dictionary<int, DhcpOption> EffectiveV4AddrOptions(DhcpV4Message requestMsg)
        {
            Dictionary<int, DhcpOption> optionMap = new Dictionary<int, DhcpOption>();
            //if (globalV4ConfigOptions != null)
            //{
            //    optionMap.PutAll(globalV4ConfigOptions.GetDhcpOptionMap());
            //}

            Dictionary<int, DhcpOption> filteredOptions =
                FilteredV4Options(requestMsg, xmlServerConfig.filters);
            if (filteredOptions != null)
            {
                optionMap.PutAll(filteredOptions);
            }
            return optionMap;
        }

        /**
         * Effective v4 addr options.
         * 
         * @param requestMsg the request msg
         * @param link the link
         * 
         * @return the map< integer, dhcp option>
         */
        public Dictionary<int, DhcpOption> EffectiveV4AddrOptions(DhcpV4Message requestMsg, DhcpLink dhcpLink)
        {
            Dictionary<int, DhcpOption> optionMap = EffectiveV4AddrOptions(requestMsg);
            if ((dhcpLink != null) && (dhcpLink.GetLink() != null))
            {
                DhcpV4ConfigOptions configOptions = dhcpLink.GetV4ConfigOptions();
                if (configOptions != null)
                {
                    optionMap.PutAll(configOptions.GetDhcpOptionMap());
                }

                Dictionary<int, DhcpOption> filteredOptions =
                    FilteredV4Options(requestMsg, dhcpLink.GetLink().linkFilters.ToList<filter>());
                if (filteredOptions != null)
                {
                    optionMap.PutAll(filteredOptions);
                }
            }
            return optionMap;
        }

        /**
        * Filtered v4 options.
        * 
        * @param requestMsg the request msg
        * @param filters the filters
        * 
        * @return the map< integer, dhcp option>
        */
        private Dictionary<int, DhcpOption> FilteredV4Options(DhcpV4Message requestMsg,
                List<filter> filters)
        {
            if ((filters != null) && filters.Count > 0)
            {
                foreach (filter filter in filters)
                {
                    if (MsgMatchesFilter(requestMsg, filter))
                    {
                        log.Info("Request matches filter: " + filter.name);
                        DhcpV4ConfigOptions filterConfigOptions =
                            new DhcpV4ConfigOptions(filter.v4ConfigOptions);
                        if (filterConfigOptions != null)
                        {
                            return filterConfigOptions.GetDhcpOptionMap();
                        }
                    }
                }
            }
            return null;
        }

        /**
         * Effective v4 addr options.
         * 
         * @param requestMsg the request msg
         * @param link the link
         * @param pool the pool
         * 
         * @return the map< integer, dhcp option>
         */
        public Dictionary<int, DhcpOption> EffectiveV4AddrOptions(DhcpV4Message requestMsg, DhcpLink dhcpLink,
                DhcpV4OptionConfigObject configObj)
        {
            Dictionary<int, DhcpOption> optionMap = EffectiveV4AddrOptions(requestMsg, dhcpLink);
            if (configObj != null)
            {
                DhcpV4ConfigOptions configOptions = configObj.GetV4ConfigOptions();
                if (configOptions != null)
                {
                    optionMap.PutAll(configOptions.GetDhcpOptionMap());
                }

                Dictionary<int, DhcpOption> filteredOptions =
                    FilteredV4Options(requestMsg, configObj.GetFilters());
                if (filteredOptions != null)
                {
                    optionMap.PutAll(filteredOptions);
                }
            }
            return optionMap;
        }
        public static v6AddressPool FindNaAddrPool(link link, IPAddress addr)
        {
            v6AddressPool pool = null;
            if (link != null)
            {
                pool = FindAddrPool(link.v6NaAddrPools, addr);
                if (pool == null)
                {
                    pool = FindAddrPool(link.v6NaAddrPools, addr);
                }
            }
            return pool;
        }
        public void SetIaMgr(IaManager iaMgr)
        {
            this.iaMgr = iaMgr;
        }
    }
}
