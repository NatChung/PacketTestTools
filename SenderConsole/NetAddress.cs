//using System;
//using System.Net;
//using System.Net.NetworkInformation;

//namespace MaxP.Arpro.Probe.Utils
//{
//    public class NetAddress
//    {
//        private static PhysicalAddress _zeroMAC = PhysicalAddress.Parse("000000000000");
//        private static PhysicalAddress _broadMAC = PhysicalAddress.Parse("FFFFFFFFFFFF");
//        private static IPAddress _emptyIPv4 = IPAddress.Parse("0.0.0.0");
//        private static IPAddress _v6BroadIP = IPAddress.Parse("ff02::1");
//        private static PhysicalAddress _v6BroadMAC = PhysicalAddress.Parse("333300000001");

//        public static PhysicalAddress ZeroMAC { get { return _zeroMAC; } }
//        public static PhysicalAddress BroadcastMAC { get { return _broadMAC; } }
//        public static IPAddress EmptyIPv4 { get { return _emptyIPv4; } }
//        public static IPAddress FF02 { get { return _v6BroadIP; } }
//        public static PhysicalAddress MAC3333 { get { return _v6BroadMAC; } }


//        public static PhysicalAddress GetBrocastMAC(IPAddress ip)
//        {
//            string strIp = ip.ToString();
//            string brocastMac = "33-33-FF-" + strIp.Split(':')[4].Substring(strIp.Split(':')[4].Length - 2, 2) + "-" + strIp.Split(':')[5].Substring(0, 2) + "-" + strIp.Split(':')[5].Substring(strIp.Split(':')[5].Length - 2, 2);
//            return PhysicalAddress.Parse(brocastMac.ToUpper());
//        }
//        public static IPAddress GetBrocastIP(IPAddress ip)
//        {
//            string strIp = ip.ToString();
//            string brocastIp = "ff02::1:ff" + strIp.Split(':')[4].Substring(strIp.Split(':')[4].Length - 2, 2) + ":" + strIp.Split(':')[5];
//            return IPAddress.Parse(brocastIp);
//        }
//        public static bool IsIpv6Multicast(PhysicalAddress mac)
//        {
//            byte[] macBytes = mac.GetAddressBytes();
//            return macBytes[0] == 0x33 && macBytes[1] == 0x33;
//        }
//    }
//}
