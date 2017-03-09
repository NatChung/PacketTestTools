using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Option.Generic;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using System.Collections.Generic;
using System.Text;
using System;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6VendorInfoOption : BaseDhcpOption
    {
        private long enterpriseNumber;  // long for unsigned int
        private List<DhcpOption> suboptionList;

        /**
         * Instantiates a new dhcp vendor info option.
         */
        public DhcpV6VendorInfoOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp vendor info option.
         * 
         * @param vendorInfoOption the vendor info option
         */
        public DhcpV6VendorInfoOption(v6VendorInfoOption vendorInfoOption) : base()
        {
            if (vendorInfoOption != null)
            {
                enterpriseNumber = vendorInfoOption.enterpriseNumber;
                List<optionDefType> optdefs = vendorInfoOption.suboptionList;
                if (optdefs != null)
                {
                    foreach (optionDefType optdef in optdefs)
                    {
                        DhcpOption subopt = GenericOptionFactory.GetDhcpOption(optdef);
                        AddSuboption(subopt);
                    }
                }
            }
            SetCode(DhcpConstants.V6OPTION_VENDOR_OPTS);
        }

        public long GetEnterpriseNumber()
        {
            return enterpriseNumber;
        }

        public void SetEnterpriseNumber(long enterpriseNumber)
        {
            this.enterpriseNumber = enterpriseNumber;
        }

        public List<DhcpOption> GetSuboptionList()
        {
            return suboptionList;
        }

        public void SetSuboptionList(List<DhcpOption> suboptionList)
        {
            this.suboptionList = suboptionList;
        }

        public void AddSuboption(DhcpOption suboption)
        {
            if (suboptionList == null)
            {
                suboptionList = new List<DhcpOption>();
            }
            suboptionList.Add(suboption);
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.DhcpOption#getLength()
         */
        public override int GetLength()
        {
            int len = 4;  // size of enterprise number (int)
            if ((suboptionList != null) && suboptionList.Count > 0)
            {
                foreach (DhcpOption subopt in suboptionList)
                {
                    if (subopt != null)
                    {
                        // code + len of suboption + suboption itself
                        len += 2 + 2 + subopt.GetLength();  // patch from Audrey Zhdanov 9/22/11
                    }
                }
            }
            return len;
        }

        /* (non-Javadoc)
         * @see java.lang.Object#toString()
         */
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());
            sb.Append(" enterpriseNumber=");
            sb.Append(enterpriseNumber);
            if ((suboptionList != null) && suboptionList.Count > 0)
            {
                sb.Append(" suboptions=");
                foreach (DhcpOption subopt in suboptionList)
                {
                    sb.Append(subopt.ToString());
                    sb.Append(',');
                }
            }
            return sb.ToString().Trim(',');
        }

        public override void Decode(ByteBuffer buf)
        {
            int len = base.DecodeLength(buf);
            if ((len > 0) && (len <= buf.remaining()))
            {
                long eof = buf.position() + len;
                if (buf.position() < eof)
                {
                    enterpriseNumber = Util.GetUnsignedInt(buf);
                    while (buf.position() < eof)
                    {
                        int code = Util.GetUnsignedShort(buf);
                        GenericOpaqueDataOption subopt = new GenericOpaqueDataOption(code, null);
                        subopt.Decode(buf);
                    }
                }
            }
        }

        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            buf.putInt((int)enterpriseNumber);
            if ((suboptionList != null) && suboptionList.Count > 0)
            {
                foreach (DhcpOption subopt in suboptionList)
                {
                    buf.put(subopt.Encode());
                }
            }
            return (ByteBuffer)buf.flip();
        }
    }
}