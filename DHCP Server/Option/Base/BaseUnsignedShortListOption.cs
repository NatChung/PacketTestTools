using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.Base
{
    public class BaseUnsignedShortListOption : BaseDhcpOption, DhcpComparableOption
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected List<int> unsignedShortList;

        public BaseUnsignedShortListOption() : this(null)
        {
        }
        public BaseUnsignedShortListOption(unsignedShortListOptionType uShortListOption) : base()
        {
            if (uShortListOption != null)
            {
                if (uShortListOption.unsignedShort != null)
                {
                    unsignedShortList = uShortListOption.unsignedShort;
                }
            }
        }

        public List<int> getUnsignedShortList()
        {
            return unsignedShortList;
        }

        public void setUnsignedShortList(List<int> unsignedShorts)
        {
            this.unsignedShortList = unsignedShorts;
        }

        public void AddUnsignedShort(int unsignedShort)
        {
            if (unsignedShortList == null)
            {
                unsignedShortList = new List<int>();
            }
            unsignedShortList.Add(unsignedShort);
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.Encodable#encode()
         */
        //        public ByteBuffer encode() throws IOException
        //        {
        //            ByteBuffer buf = super.encodeCodeAndLength();
        //        if ((unsignedShortList != null) && !unsignedShortList.isEmpty()) {
        //                for (int ushort : unsignedShortList)
        //                {
        //                    buf.putShort((short)ushort);
        //                }
        //            }
        //        return (ByteBuffer) buf.flip();
        //        }

        //        /* (non-Javadoc)
        //         * @see com.jagornet.dhcpv6.option.Decodable#decode(java.nio.ByteBuffer)
        //         */
        //        public void decode(ByteBuffer buf) throws IOException
        //        {

        //        int len = super.decodeLength(buf);
        //    	if ((len > 0) && (len <= buf.remaining())) {
        //            for (int i = 0; i<len/2; i++) {
        //                if (buf.hasRemaining()) {

        //                    addUnsignedShort(Util.getUnsignedShort(buf));
        //                }
        //}
        //        }
        //    }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.DhcpOption#getLength()
         */
        public override int GetLength()
        {
            int len = 0;
            if ((unsignedShortList != null) && unsignedShortList.Count > 0)
            {
                len = unsignedShortList.Count * 2;
            }
            return len;
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.DhcpComparableOption#matches(com.jagornet.dhcp.xml.OptionExpression)
         */
        public bool Matches(optionExpression expression)
        {
            //if (expression == null)
            //    return false;
            //if (expression.GetCode() != this.GetCode())
            //    return false;
            //if (unsignedShortList == null)
            //    return false;

            //UnsignedShortListOptionType exprOption = expression.getUShortListOption();
            //if (exprOption != null)
            //{
            //    List<Integer> exprUshorts = exprOption.getUnsignedShortList();
            //    Operator.Enum op = expression.getOperator();
            //    if (op.equals(Operator.EQUALS))
            //    {
            //        return unsignedShortList.equals(exprUshorts);
            //    }
            //    else if (op.equals(Operator.CONTAINS))
            //    {
            //        return unsignedShortList.containsAll(exprUshorts);
            //    }
            //    else
            //    {
            //        log.warn("Unsupported expression operator: " + op);
            //    }
            //}

            return false;
        }

        public override void Decode(ByteBuffer buf)
        {
            int len = base.DecodeLength(buf);
            if ((len > 0) && (len <= buf.remaining()))
            {
                for (int i = 0; i < len / 2; i++)
                {
                    if (buf.hasRemaining())
                    {
                        AddUnsignedShort(Util.GetUnsignedShort(buf));
                    }
                }
            }
        }

        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            if ((unsignedShortList != null) && unsignedShortList.Count > 0)
            {
                foreach (int s in unsignedShortList)
                {
                    buf.putShort((short)s);
                }
            }
            return (ByteBuffer)buf.flip();
        }

        /* (non-Javadoc)
         * @see java.lang.Object#toString()
         */
        //public String toString()
        //{
        //    StringBuilder sb = new StringBuilder(Util.LINE_SEPARATOR);
        //    sb.append(super.getName());
        //    sb.append(": unsignedShortList=");
        //    if ((unsignedShortList != null) && !unsignedShortList.isEmpty())
        //    {
        //        for (Integer ushort : unsignedShortList)
        //        {
        //            sb.append(ushort);
        //            sb.append(',');
        //        }
        //        sb.setLength(sb.length() - 1);
        //    }
        //    return sb.toString();
        //}
    }
}
