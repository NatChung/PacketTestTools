using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIXIS.DHCP.Xml;
using System.Net;

namespace PIXIS.DHCP.Option.Base
{
    public abstract class BaseIpAddressListOption : BaseDhcpOption, DhcpComparableOption
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private List<string> ipAddressList;

        public BaseIpAddressListOption()
            : base()
        { }
        public BaseIpAddressListOption(ipAddressListOptionType ipAddressListOption)
            : base()
        {
            if (ipAddressListOption != null)
            {
                if (ipAddressListOption.ipAddress != null)
                {
                    ipAddressList = ipAddressListOption.ipAddress;
                }
            }
        }
        public List<string> GetIpAddressList()
        {
            return ipAddressList;
        }
        public void SetIpAddressList(List<string> ipAddresses)
        {
            ipAddressList = ipAddresses;
        }
        //public void addIpAddress(string ipAddress)
        //{
        //    if (ipAddress != null)
        //    {
        //        if (_ipAddressList == null)
        //        {
        //            _ipAddressList = new ArrayList<String>();
        //        }
        //        ipAddressList.add(ipAddress);
        //    }
        //}
        //public void AddIpAddress(byte[] addr)
        //{
        //    try
        //    {
        //        if (addr != null)
        //        {
        //            IPAddress inetAddr = InetAddress.getByAddress(addr);
        //            this.addIpAddress(inetAddr);
        //        }
        //    }
        //    catch (UnknownHostException ex)
        //    {
        //        log.error("Failed to add DnsServer: " + ex);
        //    }
        //}
        //public void AddIpAddress(IPAddress inetAddr)
        //{
        //    if (inetAddr != null)
        //    {
        //        this.AddIpAddress(inetAddr.getHostAddress());
        //    }
        //}
        public override void Decode(ByteBuffer buf)
        {

            int len = base.DecodeLength(buf);
            if ((len > 0) && (len <= buf.remaining()))
            {
                long eof = buf.position()+len;
                while (buf.position() < eof)
                {
                    // it has to be hex from the wire, right?
                    byte[] b;
                    if (!base.IsV4())
                    {
                        b = new byte[16];
                    }
                    else
                    {
                        b = new byte[4];
                    }
                    buf.put(b);
                    //this.addipaddress(b);
                }
            }
        }
        public override int GetLength()
        {
            int len = 0;
            if (ipAddressList != null)
            {
                if (!base.IsV4())
                {
                    len += ipAddressList.Count * 16;   // each IPv6 address is 16 bytes
                }
                else
                {
                    len += ipAddressList.Count * 4;    // each IPv4 address is 4 bytes
                }
            }
            return len;
        }
        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            if (ipAddressList != null)
            {
                foreach (string ip in ipAddressList)
                {
                    IPAddress inetAddr = null;
                    if (!base.IsV4())
                    {
                        inetAddr = IPAddress.Parse(ip);
                    }
                    else
                    {
                        inetAddr = IPAddress.Parse(ip);
                    }
                    buf.put(inetAddr.GetAddressBytes());
                }
            }
            return (ByteBuffer)buf.flip();
        }
        public bool Matches(optionExpression expression)
        {
            throw new NotImplementedException();
        }
    }
}
