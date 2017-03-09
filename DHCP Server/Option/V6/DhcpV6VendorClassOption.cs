using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6VendorClassOption : BaseOpaqueDataListOption
    {
        private long enterpriseNumber;  // long for unsigned int

        /**
         * Instantiates a new dhcp vendor class option.
         */
        public DhcpV6VendorClassOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp vendor class option.
         * 
         * @param vendorClassOption the vendor class option
         */
        public DhcpV6VendorClassOption(v6VendorClassOption vendorClassOption) : base(vendorClassOption)
        {
            if (vendorClassOption != null)
            {
                enterpriseNumber = vendorClassOption.enterpriseNumber;
            }
            SetCode(DhcpConstants.V6OPTION_VENDOR_CLASS);
        }

        public long GetEnterpriseNumber()
        {
            return enterpriseNumber;
        }

        public void SetEnterpriseNumber(long enterpriseNumber)
        {
            this.enterpriseNumber = enterpriseNumber;
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.DhcpOption#getLength()
         */
        public override int GetLength()
        {
            int len = 4 + base.GetLength();  // size of enterprise number (int)
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
            return sb.ToString();
        }

        public bool Matches(DhcpV6VendorClassOption that, @operator op)
        {
            if (that == null)
                return false;
            if (that.GetCode() != this.GetCode())
                return false;
            if (that.GetEnterpriseNumber() != this.GetEnterpriseNumber())
                return false;

            return base.Matches(that, op);
        }
        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            buf.putInt((int)GetEnterpriseNumber());
            List<BaseOpaqueData> vendorClasses = GetOpaqueDataList();
            if ((vendorClasses != null) && vendorClasses.Count > 0)
            {
                foreach (BaseOpaqueData opaque in vendorClasses)
                {
                    opaque.EncodeLengthAndData(buf);
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
                    SetEnterpriseNumber(Util.GetUnsignedInt(buf));
                    while (buf.position() < eof)
                    {
                        BaseOpaqueData opaque = new BaseOpaqueData();
                        opaque.DecodeLengthAndData(buf);
                        AddOpaqueData(opaque);
                    }
                }
            }
        }
    }
}
