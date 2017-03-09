using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option.Base
{
    public class BaseOpaqueDataListOption : BaseDhcpOption, DhcpComparableOption
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<BaseOpaqueData> opaqueDataList;

        /**
         * Instantiates a new opaque opaqueData list option.
         */
        public BaseOpaqueDataListOption() : this(null)
        {
        }

        /**
         * Instantiates a new opaque opaqueData list option.
         * 
         * @param opaqueDataListOption the opaque opaqueData list option
         */
        public BaseOpaqueDataListOption(opaqueDataListOptionType opaqueDataListOption) : base()
        {
            if (opaqueDataListOption != null)
            {
                List<opaqueData> _opaqueDataList = opaqueDataListOption.opaqueData;
                if (_opaqueDataList != null)
                {
                    foreach (opaqueData opaqueData in _opaqueDataList)
                    {
                        AddOpaqueData(new BaseOpaqueData(opaqueData));
                    }
                }
            }
        }

        public List<BaseOpaqueData> GetOpaqueDataList()
        {
            return opaqueDataList;
        }

        public void SetOpaqueDataList(List<BaseOpaqueData> opaqueDataList)
        {
            this.opaqueDataList = opaqueDataList;
        }

        public void AddOpaqueData(BaseOpaqueData baseOpaque)
        {
            if (baseOpaque != null)
            {
                if (opaqueDataList == null)
                {
                    opaqueDataList = new List<BaseOpaqueData>();
                }
                opaqueDataList.Add(baseOpaque);
            }
        }

        public void AddOpaqueData(String opaqueString)
        {
            if (opaqueString != null)
            {
                BaseOpaqueData opaque = new BaseOpaqueData(opaqueString);
                AddOpaqueData(opaque);
            }
        }

        public void addOpaqueData(byte[] opaqueData)
        {
            if (opaqueData != null)
            {
                BaseOpaqueData opaque = new BaseOpaqueData(opaqueData);
                AddOpaqueData(opaque);
            }
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.DhcpOption#getLength()
         */
        public override int GetLength()
        {
            int len = 0;
            if ((opaqueDataList != null) && opaqueDataList.Count > 0)
            {
                foreach (BaseOpaqueData opaque in opaqueDataList)
                {
                    len += 2 + opaque.GetLength();
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
            if (expression.code != this.GetCode())
                return false;

            return Matches((opaqueDataListOptionType)expression.Item, expression.@operator);
        }

        public bool Matches(opaqueDataListOptionType that, @operator op)
        {
            if (that != null)
            {
                List<opaqueData> opaqueList = that.opaqueData;
                if (opaqueList != null)
                {
                    if (op.Equals(@operator.equals))
                    {
                        if (opaqueList.Count != opaqueDataList.Count)
                        {
                            return false;
                        }
                        for (int i = 0; i < opaqueList.Count; i++)
                        {
                            BaseOpaqueData opaque = new BaseOpaqueData(opaqueList[i]);
                            BaseOpaqueData myOpaque = opaqueDataList[i];
                            if (!OpaqueDataUtil.Equals(opaque, myOpaque))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    else if (op.Equals(@operator.contains))
                    {
                        if (opaqueList.Count > opaqueDataList.Count)
                        {
                            return false;
                        }
                        for (int i = 0; i < opaqueList.Count; i++)
                        {
                            BaseOpaqueData opaque = new BaseOpaqueData(opaqueList[i]);
                            bool found = false;
                            for (int j = 0; j < opaqueDataList.Count; j++)
                            {
                                BaseOpaqueData myOpaque = opaqueDataList[j];
                                if (OpaqueDataUtil.Equals(opaque, myOpaque))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        log.Warn("Unsupported expression operator: " + op);
                    }
                }
            }

            return false;
        }

        public bool Matches(BaseOpaqueDataListOption that, @operator op)
        {
            if (opaqueDataList == null)
                return false;

            if (that != null)
            {
                List<BaseOpaqueData> opaqueList = that.GetOpaqueDataList();
                if (opaqueList != null)
                {
                    if (op.Equals(@operator.equals))
                    {
                        if (opaqueList.Count != opaqueDataList.Count)
                        {
                            return false;
                        }
                        for (int i = 0; i < opaqueList.Count; i++)
                        {
                            BaseOpaqueData opaque = opaqueList[i];
                            BaseOpaqueData myOpaque = opaqueDataList[i];
                            if (!OpaqueDataUtil.Equals(opaque, myOpaque))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    else if (op.Equals(@operator.contains))
                    {
                        if (opaqueList.Count > opaqueDataList.Count)
                        {
                            return false;
                        }
                        for (int i = 0; i < opaqueList.Count; i++)
                        {
                            BaseOpaqueData opaque = opaqueList[i];
                            bool found = false;
                            for (int j = 0; j < opaqueDataList.Count; j++)
                            {
                                BaseOpaqueData myOpaque = opaqueDataList[j];
                                if (OpaqueDataUtil.Equals(opaque, myOpaque))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        log.Warn("Unsupported expression operator: " + op);
                    }
                }
            }

            return false;
        }

        /* (non-Javadoc)
         * @see java.lang.Object#toString()
         */
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Util.LINE_SEPARATOR);
            sb.Append(base.GetName());
            sb.Append(": dataList=");
            if ((opaqueDataList != null) && opaqueDataList.Count > 0)
            {
                foreach (BaseOpaqueData opaque in opaqueDataList)
                {
                    sb.Append(opaque.ToString());
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
                    BaseOpaqueData opaque = new BaseOpaqueData();
                    opaque.DecodeLengthAndData(buf);
                    AddOpaqueData(opaque);
                }
            }
        }

        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            if ((opaqueDataList != null) && opaqueDataList.Count > 0)
            {
                foreach (BaseOpaqueData opaque in opaqueDataList)
                {
                    opaque.DecodeLengthAndData(buf);
                }
            }
            return (ByteBuffer)buf.flip();
        }
    }
}
