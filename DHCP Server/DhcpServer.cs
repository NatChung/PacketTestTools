using PIXIS.DHCP.Config;
using PIXIS.DHCP.DB;
using PIXIS.DHCP.Message;
using PIXIS.DHCP.Request;
using PIXIS.DHCP.Request.Bind;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.V4Process;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP
{
    public class DhcpServer
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //private DhcpListener _listerner;

        private DhcpServerConfiguration _dhcpServerConfig = new DhcpServerConfiguration();
        /// <summary>
        /// DHCP 派發IP前，確認IP是否已經使用
        /// </summary>
        /// <param name="ip">檢查IP</param>
        /// <param name="wait">等待時間(毫秒)</param>
        /// <returns></returns>
        public Func<IPAddress, int, bool> CheckIPIsUsed { get; set; }

        public DhcpServer()
        {
            // _listerner = new DhcpListener();
        }

        public void DeleteLinkMap(Subnet s)
        {
            _dhcpServerConfig.DeleteLinkMap(s);
        }
        public void RemoveLink(Subnet subnet)
        {
            _dhcpServerConfig.RemoveLink(subnet);
        }
        public void SetLinkMap(Dictionary<Subnet, DhcpLink> map)
        {
            _dhcpServerConfig.SetLinkMap(map);
        }

        /// <summary>
        ///  DHCPv6 ServerID should be auto-generated hex data
        /// </summary>
        //public void SetDHCPV6ServerId(string duid)
        //{
        //    _dhcpServerConfig.SetV6ServerIdOption(new v6ServerIdOption()
        //    {
        //        opaqueData = new opaqueData
        //        {
        //            hexValue = Util.FromHexString(duid)
        //        }
        //    });
        //}

        /// <summary>
        /// DHCPv4 ServerID must be a local IP address
        /// </summary>
        public void SetDHCPV4ServerId(string localAddress)
        {
            _dhcpServerConfig.SetV4ServerIdOption(new v4ServerIdOption()
            {
                ipAddress = localAddress
            });
        }
        //public DhcpV6Message DhcpV6MessageReceived(DhcpV6Message msg)
        //{
        //    return DhcpV6MessageHandler.HandleMessage(_serverAddr, msg);
        //}

        //public DhcpV4Message DhcpV4MessageReceived(DhcpV4Message msg)
        //{
        //    return DhcpV4MessageHandler.HandleMessage(_serverAddr, msg);
        //}

        public void AddOrUpdateLinkMap(Subnet subnet, DhcpLink link)
        {
            _dhcpServerConfig.AddOrUpdateLinkMap(subnet, link);
            _dhcpServerConfig.InitV4AddrBindingManager();
            _dhcpServerConfig.InitV6AddrBindingManager();
        }
        private void LoadManagers()
        {
            Debug.Assert(CheckIPIsUsed != null, "DhcpServer --LoadManagers-- CheckIPIsUsed = null");
            log.Info("Loading managers from context...");

            V6NaAddrBindingManager v6NaAddrBindingMgr = new V6NaAddrBindingManagerImpl();
            v6NaAddrBindingMgr.CheckIPIsUsed = CheckIPIsUsed;
            try
            {
                log.Info("Initializing V6 NA Address Binding Manager");
                v6NaAddrBindingMgr.Init();
                _dhcpServerConfig.SetNaAddrBindingMgr(v6NaAddrBindingMgr);
            }
            catch (Exception ex)
            {
                log.Error("Failed initialize V6 NA Address Binding Manager");
                throw ex;
            }

            V6TaAddrBindingManager v6TaAddrBindingMgr = new V6TaAddrBindingManagerImpl();
            v6TaAddrBindingMgr.CheckIPIsUsed = CheckIPIsUsed;
            try
            {
                log.Info("Initializing V6 TA Address Binding Manager");
                v6TaAddrBindingMgr.Init();
                _dhcpServerConfig.SetTaAddrBindingMgr(v6TaAddrBindingMgr);
            }
            catch (Exception ex)
            {
                log.Error("Failed initialize V6 TA Address Binding Manager");
                throw ex;
            }

            V6PrefixBindingManager v6PrefixBindingMgr = new V6PrefixBindingManagerImpl();
            v6PrefixBindingMgr.CheckIPIsUsed = CheckIPIsUsed;
            try
            {
                log.Info("Initializing V6 Prefix Binding Manager");
                v6PrefixBindingMgr.Init();
                _dhcpServerConfig.SetPrefixBindingMgr(v6PrefixBindingMgr);
            }
            catch (Exception ex)
            {
                log.Error("Failed initialize V6 Prefix Binding Manager");
                throw ex;
            }

            V4AddrBindingManager v4AddrBindingMgr = new V4AddrBindingManagerImpl();
            v4AddrBindingMgr.CheckIPIsUsed = CheckIPIsUsed;
            try
            {
                log.Info("Initializing V4 Address Binding Manager");
                v4AddrBindingMgr.Init();
                _dhcpServerConfig.SetV4AddrBindingMgr(v4AddrBindingMgr);
            }
            catch (Exception ex)
            {
                log.Error("Failed initialize V4 Address Binding Manager");
                throw ex;
            }

            IaManager iaMgr = new LeaseManager();
            _dhcpServerConfig.SetIaMgr(iaMgr);

            log.Info("Managers loaded.");
        }
        private void BindDhcpPort()
        {

        }
        public void Start()
        {
            LoadManagers();

            // _listerner.Open();
        }
        public void Stop()
        {
            // _listerner.Close();
        }


    }
}
