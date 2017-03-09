using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using System.Collections.Generic;
using System.Text;
using System;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6GeoconfCivicOption : BaseDhcpOption
    {
        private short what;
        private string countryCode;
        private List<CivicAddress> civicAddressList;

        public class CivicAddress
        {
            public short type;
            public string value;
            public CivicAddress() : this(null)
            {
            }
            public CivicAddress(civicAddressElement caElement)
            {
                if (caElement != null)
                {
                    type = caElement.caType;
                    value = caElement.caValue;
                }
            }
        }


        /**
        * Instantiates a new dhcp geoconf civic option.
        */
        public DhcpV6GeoconfCivicOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp geoconf civic option.
         * 
         * @param geoconfCivicOption the geoconf civic option
         */
        public DhcpV6GeoconfCivicOption(v6GeoconfCivicOption geoconfCivicOption) : base()
        {
            if (geoconfCivicOption != null)
            {
                what = geoconfCivicOption.what;
                countryCode = geoconfCivicOption.countryCode;
                List<civicAddressElement> cas = geoconfCivicOption.civicAddressElement;
                if ((cas != null) && cas.Count > 0)
                {
                    foreach (civicAddressElement civicAddressElement in cas)
                    {
                        AddCivicAddress(new CivicAddress(civicAddressElement));
                    }
                }
            }
        }

        public short GetWhat()
        {
            return what;
        }

        public void SetWhat(short what)
        {
            this.what = what;
        }

        public string GetCountryCode()
        {
            return countryCode;
        }

        public void SetCountryCode(string countryCode)
        {
            this.countryCode = countryCode;
        }

        public List<CivicAddress> GetCivicAddressList()
        {
            return civicAddressList;
        }

        public void SetCivicAddressList(List<CivicAddress> civicAddressList)
        {
            this.civicAddressList = civicAddressList;
        }

        public void AddCivicAddress(CivicAddress civicAddress)
        {
            if (civicAddressList == null)
            {
                civicAddressList = new List<CivicAddress>();
            }
            civicAddressList.Add(civicAddress);
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.DhcpOption#getLength()
         */
        public int getLength()
        {
            int len = 3;    // size of what(1) + country code(2)
            if ((civicAddressList != null) && civicAddressList.Count > 0)
            {
                foreach (CivicAddress civicAddr in civicAddressList)
                {
                    len += 2;   // CAtype byte + CAlength byte
                    string caVal = civicAddr.value;
                    if (caVal != null)
                        len += caVal.Length;
                }
            }
            return len;
        }


        public override int GetLength()
        {
            int len = 3;    // size of what(1) + country code(2)
            if ((civicAddressList != null) && civicAddressList.Count > 0)
            {
                foreach (CivicAddress civicAddr in civicAddressList)
                {
                    len += 2;   // CAtype byte + CAlength byte
                    String caVal = civicAddr.value;
                    if (caVal != null)
                        len += caVal.Length;
                }
            }
            return len;
        }
        /* (non-Javadoc)
         * @see java.lang.Object#toString()
         */
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Util.LINE_SEPARATOR);
            sb.Append(base.GetName());
            sb.Append(": what=");
            sb.Append(what);
            sb.Append(" countryCode=");
            sb.Append(countryCode);
            if ((civicAddressList != null) && civicAddressList.Count > 0)
            {
                sb.Append(Util.LINE_SEPARATOR);
                foreach (CivicAddress civicAddr in civicAddressList)
                {
                    sb.Append("civicAddress: type=");
                    sb.Append(civicAddr.type);
                    sb.Append(" value=");
                    sb.Append(civicAddr.value);
                }
            }
            return sb.ToString();
        }

        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            buf.put((byte)what);
            if (countryCode != null)
            {
                buf.put(Encoding.ASCII.GetBytes(countryCode));
            }
            else
            {
                //TODO: throw exception?
                buf.put(Encoding.ASCII.GetBytes("XX"));
            }
            if ((civicAddressList != null) && civicAddressList.Count > 0)
            {
                foreach (CivicAddress civicAddr in civicAddressList)
                {
                    buf.put((byte)civicAddr.type);
                    String caVal = civicAddr.value;
                    if (caVal != null)
                    {
                        buf.put((byte)caVal.Length);
                        buf.put(Encoding.ASCII.GetBytes(caVal));
                    }
                    else
                    {
                        buf.put((byte)0);
                    }
                }
            }
            return (ByteBuffer)buf.flip();
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.Decodable#decode(java.nio.ByteBuffer)
         */
        public override void Decode(ByteBuffer buf)
        {

            int len = base.DecodeLength(buf);
            if ((len > 0) && (len <= buf.remaining()))
            {
                long eof = buf.position() + len;
                if (buf.position() < eof)
                {
                    what = Util.GetUnsignedByte(buf);
                    if (buf.position() < eof)
                    {
                        byte[] country = buf.getBytes(2);
                        countryCode = Encoding.ASCII.GetString(country);
                        while (buf.position() < eof)
                        {
                            CivicAddress civicAddr = new CivicAddress();
                            civicAddr.type = Util.GetUnsignedByte(buf);
                            short caLen = Util.GetUnsignedByte(buf);
                            if (caLen > 0)
                            {
                                byte[] caVal = buf.getBytes(caLen);
                                civicAddr.value = Encoding.ASCII.GetString(caVal);
                            }
                            AddCivicAddress(civicAddr);
                        }
                    }
                }
            }
        }
    }
}