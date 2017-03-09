using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Utility
{
    public class Util
    {
        public static bool IS_WINDOWS = (Environment.OSVersion.Platform == PlatformID.Win32NT) ||
               (Environment.OSVersion.Platform == PlatformID.Win32Windows);

        public static string LINE_SEPARATOR = Environment.NewLine;

        public static TimeZone GMT_TIMEZONE = TimeZone.CurrentTimeZone;

        public static DateTime GMT_CALENDAR = DateTime.Now.ToUniversalTime();

        public static string GMT_DATEFORMAT = "yyyy-MM-dd HH:mm:ss";

        public static string ToHexString(byte[] binary)
        {
            if ((binary != null))
            {
                StringBuilder str = new StringBuilder((binary.Length * 2));
                // TODO: Warning!!!, inline IF is not supported ?
                for (int i = 0; (i < binary.Length); i++)
                {
                    long v = Helper.MoveByte((binary[i] << 24), 24);
                    str.Append((v < 0x10 ? "0" : "") + Convert.ToString(v, 16));
                    // TODO: Warning!!!, inline IF is not supported ?
                }

                return str.ToString();
            }

            return null;
        }
        public static byte[] FromHexString(string hexString)
        {
            if (!string.IsNullOrEmpty(hexString))
            {
                byte[] barray = Enumerable.Range(0, hexString.Length)
                   .Where(x => x % 2 == 0)
                   .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                   .ToArray();

                return barray;
            }

            return null;
        }

        public static short GetUnsignedByte(ByteBuffer buf)
        {
            return (short)(Convert.ToByte(buf.get()) & 0xff);
        }

        public static int GetUnsignedShort(ByteBuffer buf)
        {
            var temp1 = buf.getByte();
            var temp2 = buf.getByte();
            var result = new byte[] { temp2, temp1 };
            return BitConverter.ToInt16(result, 0);
        }

        public static long GetUnsignedInt(ByteBuffer buf)
        {
            var temp1 = buf.getByte();
            var temp2 = buf.getByte();
            var temp3 = buf.getByte();
            var temp4 = buf.getByte();
            var result = new byte[] { temp4, temp3, temp2, temp1 };
            return BitConverter.ToInt32(result, 0);
            //return (buf.getInt() & 0xffffffffL);
        }

        public static BigInteger GetUnsignedLong(ByteBuffer buf)
        {
            byte[] data = new byte[8];
            return new BigInteger(buf.get(data, 0, 8).getLong());
        }

        public static int GetUnsignedMediumInt(ByteBuffer buf)
        {
            int b1 = Util.GetUnsignedByte(buf);
            int b2 = Util.GetUnsignedByte(buf);
            int b3 = Util.GetUnsignedByte(buf);
            if (!BitConverter.IsLittleEndian)
            {
                return b1 << 16 | b2 << 8 | b3;
            }
            else
            {
                return b1 << 16 | b2 << 8 | b3;
            }

        }

        public static ByteBuffer PutMediumInt(ByteBuffer buf, int value)
        {
            byte b1 = (byte)(value >> 16);
            byte b2 = (byte)(value >> 8);
            byte b3 = (byte)value;
            if (!BitConverter.IsLittleEndian)
            {
                buf.put(b3).put(b2).put(b1);
            }
            else
            {
                buf.put(b1).put(b2).put(b3);

            }

            return buf;
        }

        public static int CompareInetAddrs(IPAddress ip1, IPAddress ip2)
        {
            ulong bi1 = BitConverter.ToUInt32(ip1.GetAddressBytes(), 0);
            ulong bi2 = BitConverter.ToUInt32(ip2.GetAddressBytes(), 0);
            return bi1.CompareTo(bi2);
        }

        public static string SocketAddressAsString(IPEndPoint saddr)
        {
            if ((saddr == null))
            {
                return null;
            }
            else
            {
                return (saddr.ToString() + (":" + saddr.Port));
            }

        }

        public static IPAddress NetIfIPv6LinkLocalAddress(NetworkInterface netIf)
        {
            IPAddress v6Addr = null;
            var ifAddrs = netIf.GetIPProperties();
            foreach (var ip in ifAddrs.UnicastAddresses)
            {
                if ((netIf.OperationalStatus == OperationalStatus.Up)
                    && (ip.Address.AddressFamily == AddressFamily.InterNetworkV6) && ip.Address.IsIPv6LinkLocal)
                {
                    v6Addr = ip.Address;
                }
            }
            return v6Addr;
        }
    }
}
