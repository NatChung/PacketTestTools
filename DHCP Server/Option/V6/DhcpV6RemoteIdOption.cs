using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using System.Text;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6RemoteIdOption : BaseOpaqueDataOption
    {
        private long enterpriseNumber;  // long for unsigned int

        /**
         * Instantiates a new dhcp remote id option.
         */
        public DhcpV6RemoteIdOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp remote id option.
         * 
         * @param remoteIdOption the remote id option
         */
        public DhcpV6RemoteIdOption(v6RemoteIdOption remoteIdOption) : base(remoteIdOption)
        {
            if (remoteIdOption != null)
            {
                enterpriseNumber = remoteIdOption.enterpriseNumber;
            }
            SetCode(DhcpConstants.V6OPTION_REMOTE_ID);
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
         * @see com.jagornet.dhcpv6.option.DhcpComparableOption#matches(com.jagornet.dhcp.xml.OptionExpression)
         */
        public override bool Matches(optionExpression expression)
        {
            if (expression == null)
                return false;
            if (expression.code != this.code)
                return false;

            return false;
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
    }
}