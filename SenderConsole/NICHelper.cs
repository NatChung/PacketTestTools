using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using SharpPcap;
using SharpPcap.LibPcap;

namespace MaxP.Arpro.Probe.Utils
{
    public class NICHelper
    {
        //private static ICaptureDevice _ncard;
        private static List<string> _appleMACList;
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ICaptureDevice GetNcard(PhysicalAddress mac)
        {
            try
            {
                string osType = Environment.OSVersion.Platform.ToString();
                LibPcapLiveDeviceList allDev = LibPcapLiveDeviceList.Instance;
                LibPcapLiveDevice device = allDev[0];
                _log.DebugFormat("Trying to get NIC by address {0}", mac.ToString());
                _log.Debug("List all device : ");
                foreach (LibPcapLiveDevice nCard in allDev)
                {
                    _log.DebugFormat("Name {0}. Address count :{1}", nCard.Name, nCard.Interface.Addresses.Count);
                    if (osType.ToUpper().Contains("WIN"))
                    {
                        //VLan enabled
                        if (nCard.Addresses.Count == 0)
                        {
                            device = nCard;
                            _log.Info("Get vlan nCard: " + device.Name);
                            break;
                        }

                        //VLan disabled
                        foreach (var addr in nCard.Addresses)
                        {
                            if (addr.Addr.hardwareAddress != null && mac.Equals(addr.Addr.hardwareAddress))
                                device = nCard;
                        }
                    }
                    else
                    {
                        if (!nCard.Name.Contains("."))
                        {
                            foreach (var addr in nCard.Addresses)
                            {
                                if (addr.Addr.hardwareAddress != null && mac.Equals(addr.Addr.hardwareAddress))
                                    device = nCard;
                            }
                        }
                    }
                }
                _log.DebugFormat("Get NIC : " + device.Name + " System is : " + osType);
                return device;
            }
            catch (NullReferenceException ex)
            {
                Exception msg = new Exception("Can't Get Ncard");
                _log.Error("Have null ref, maybe macaddress in app.config incorrect Error is :"+ ex.Message);
                throw msg;
            }
        }



        internal static bool IsAppleHost(PhysicalAddress mac)
        {
            List<string> appleMac = GetMacList();
            string prefix = mac.ToString().Substring(0, 6);
            if (appleMac.Contains(prefix))
            {
                _log.Debug("Get Apple Mac: " + mac);
                return true;
            }
            else
                return false;
        }

        //取得Apple的MAC前六碼列表
        internal static List<string> GetMacList()
        {
            if (_appleMACList == null)
            {
                _appleMACList = new List<string>();
                _appleMACList.AddRange(File.ReadAllLines("Utils/MAC.txt"));
            }
            return _appleMACList;
        }
    }
}
