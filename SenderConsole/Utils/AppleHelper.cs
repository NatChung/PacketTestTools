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
    public class AppleHelper
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string _listFileDirectory = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Utils");
        private static readonly string _listAppleMacFileName = "MAC.txt";
        private static readonly string _listAllVendorFileName = "Nic.txt";
        private static List<string> _appleListInstance;
        private static Dictionary<string, string> _nicMap;

        internal static bool IsAppleHost(PhysicalAddress mac)
        {
            List<string> appleMac = GetAppleMacList();
            string prefix = mac.ToString().Substring(0, 6);
            return appleMac.Contains(prefix);
        }

        //取得Apple的MAC前六碼列表
        internal static List<string> GetAppleMacList()
        {
            if (_appleListInstance == null)
            {
                string filePath = Path.Combine(_listFileDirectory, _listAppleMacFileName);
                if (!File.Exists(filePath))
                {
                    _log.WarnFormat("GetAppleMacList Failed !! File: {0} is not existed !!", filePath);
                    return null;
                }
                _appleListInstance = new List<string>();
                _appleListInstance.AddRange(File.ReadAllLines(filePath));
            }
            return _appleListInstance;
        }

        internal static Dictionary<string, string> GetAllVendorMacList()
        {
            if (_nicMap == null)
            {
                string filePath = Path.Combine(_listFileDirectory, _listAllVendorFileName);
                if (!File.Exists(filePath))
                {
                    _log.WarnFormat("GetAllVendorMacList Failed !! File: {0} is not existed !!", filePath);
                    return null;
                }
                _nicMap = new Dictionary<string, string>();
                char[] trimSpliters = new char[] { ' ', ',' };
                string[] data = File.ReadAllLines(filePath);
                foreach (string s in data)
                {
                    string[] temp = s.Split('|');
                    string macVendor = TrimMacAddress(temp[0].Trim());
                    if (_nicMap.ContainsKey(macVendor))
                    {
                        continue;
                    }
                    _nicMap.Add(macVendor, temp[1].Trim(trimSpliters).ToUpper());
                }
            }
            return _nicMap;
        }
        private static string TrimMacAddress(string mac)
        {
            string trimMac = mac;
            char[] Spliters = new char[] { ':', '-', ' ', '.' };
            foreach (char EachSpliter in Spliters)
            {
                if (mac.Contains(new string(EachSpliter, 1)))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string EachSlice in mac.Split(EachSpliter))
                    {
                        sb.Append(EachSlice);
                    }
                    trimMac = sb.ToString().ToUpper();
                }
            }
            return trimMac;
        }
    }
}
