using MaxP.Arpro.Probe;
using MaxP.Arpro.Probe.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Management;
using SharpPcap;
using SenderConsole.Tester;

namespace SenderConsole
{
    public class Program
    {
        

        static void Main(string[] args)
        {
            
            IProbeFactory _memberFactory = new WindowsProbeFactory();
            //TestDevice device = new TestDevice(PhysicalAddress.Parse("00-E0-4C-68-02-91"), IPAddress.Parse("fe80::1c66:e750:c259:df88"), _memberFactory.CreateNICHelper());
            TestDevice device = new TestDevice(PhysicalAddress.Parse("98-5F-D3-58-7D-28"), IPAddress.Parse("fe80::c71:214f:ef74:b2ce"), _memberFactory.CreateNICHelper());

            for (int i = 0; i < 10; i++)
            {
                int transationId = 0x268285 + i;
                DHCPv6Solicit solicit = new DHCPv6Solicit(transationId);
                DHCPv6Advertisement advertise = new DHCPv6Advertisement(transationId);
                DHCPv6Request request = new DHCPv6Request(transationId, advertise);
                DHCPv6Reply confirm = new DHCPv6Reply(transationId);

                DHCPv6Tester tester = new DHCPv6Tester(solicit, advertise, new DHCPv6Tester(request, confirm, null));
                tester.Start(device);
            }

            Console.ReadLine();
        }
        
        public static string GrenerateMACAddress()
        {
            var sBuilder = new StringBuilder();
            var r = new Random();
            int number;
            byte b;
            for (int i = 0; i < 3; i++)
            {
                number = r.Next(0, 255);
                b = Convert.ToByte(number);
                if (i == 0)
                {
                    b = SetBit(b, 6); //--> set locally administered
                    b = UnsetBit(b, 7); // --> set unicast 
                }
                sBuilder.Append(number.ToString("X2"));
            }
            return "00E04C" + sBuilder.ToString().ToUpper();
        }
        private static byte SetBit(byte b, int BitNumber)
        {
            if (BitNumber < 8 && BitNumber > -1)
            {
                return (byte)(b | (byte)(0x01 << BitNumber));
            }
            else
            {
                throw new InvalidOperationException(
                "Der Wert für BitNumber " + BitNumber.ToString() + " war nicht im zulässigen Bereich! (BitNumber = (min)0 - (max)7)");
            }
        }
        private static byte UnsetBit(byte b, int BitNumber)
        {
            if (BitNumber < 8 && BitNumber > -1)
            {
                return (byte)(b | (byte)(0x00 << BitNumber));
            }
            else
            {
                throw new InvalidOperationException(
                "Der Wert für BitNumber " + BitNumber.ToString() + " war nicht im zulässigen Bereich! (BitNumber = (min)0 - (max)7)");
            }
        }
        private static IPAddress[] IPs()
        {
            string strHostName = Dns.GetHostName();
            return Dns.GetHostAddresses(strHostName);
        }
    }
}
