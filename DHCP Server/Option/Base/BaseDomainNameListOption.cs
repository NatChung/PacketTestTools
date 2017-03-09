using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option.Base
{
    public class BaseDomainNameListOption : BaseDhcpOption, DhcpComparableOption
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected List<String> domainNameList;

        /**
         * Instantiates a new base domain name list option.
         */
        public BaseDomainNameListOption() : this(null)
        {
        }

        /**
         * Instantiates a new base domain name list option.
         *
         * @param domainNameListOption the domain name list option
         */
        public BaseDomainNameListOption(domainNameListOptionType domainNameListOption) : base()
        {
            if (domainNameListOption != null)
            {
                if (domainNameListOption.domainName != null)
                {
                    domainNameList = domainNameListOption.domainName;
                }
            }
        }

        public List<String> GetDomainNameList()
        {
            return domainNameList;
        }

        public void SetDomainNameList(List<String> domainNames)
        {
            this.domainNameList = domainNames;
        }

        public void AddDomainName(String domainName)
        {
            if (domainName != null)
            {
                if (domainNameList == null)
                {
                    domainNameList = new List<string>();
                }
                domainNameList.Add(domainName);
            }
        }
        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.DhcpOption#getLength()
         */
        public override int GetLength()
        {
            int len = 0;
            if (domainNameList != null)
            {
                foreach (string domain in domainNameList)
                {
                    len += BaseDomainNameOption.GetDomainNameLength(domain);
                }
            }
            return len;
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.DhcpComparableOption#matches(com.jagornet.dhcp.xml.OptionExpression)
         */
        public bool Matches(optionExpression expression)
        {
            if (expression == null)
                return false;
            if (expression.code != this.code)
                return false;
            if (domainNameList == null)
                return false;

            // first see if we have a domain name list option to compare to
            //domainNameListOptionType exprOption = expression.ItemElementName.@operator.getDomainNameListOption();
            //if (exprOption != null)
            //{
            //    List<String> exprDomainNames = exprOption.getDomainNameList();
            //    Operator.Enum op = expression.getOperator();
            //    if (op.equals(Operator.EQUALS))
            //    {
            //        return domainNameList.equals(exprDomainNames);
            //    }
            //    else if (op.equals(Operator.CONTAINS))
            //    {
            //        return domainNameList.containsAll(exprDomainNames);
            //    }
            //    else
            //    {
            //        log.warn("Unsupported expression operator: " + op);
            //    }
            //}

            return false;
        }

        /* (non-Javadoc)
         * @see java.lang.Object#toString()
         */
        public String toString()
        {
            StringBuilder sb = new StringBuilder(Util.LINE_SEPARATOR);
            sb.Append(base.name);
            sb.Append(": domainNameList=");
            if (domainNameList != null)
            {
                foreach (String domain in domainNameList)
                {
                    sb.Append(domain);
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
                while (buf.position() < eof)
                {
                    string domain = BaseDomainNameOption.DecodeDomainName(buf, eof);
                    AddDomainName(domain);
                }
            }
        }

        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            if (domainNameList != null)
            {
                foreach (string domain in domainNameList)
                {
                    BaseDomainNameOption.EncodeDomainName(buf, domain);
                }
            }
            return (ByteBuffer)buf.flip();
        }
    }
}
