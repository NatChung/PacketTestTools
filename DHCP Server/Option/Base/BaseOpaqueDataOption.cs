using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option.Base
{
    public abstract class BaseOpaqueDataOption : BaseDhcpOption, DhcpComparableOption
    {

        protected BaseOpaqueData opaqueData;

        public BaseOpaqueDataOption() :
                this(null)
        {
        }

        public BaseOpaqueDataOption(opaqueDataOptionType opaqueDataOption) :
                base()
        {
            if (((opaqueDataOption != null)
                        && (opaqueDataOption.opaqueData != null)))
            {
                this.opaqueData = new BaseOpaqueData(opaqueDataOption.opaqueData);
            }
            else
            {
                this.opaqueData = new BaseOpaqueData();
            }

        }

        public BaseOpaqueData GetOpaqueData()
        {
            return this.opaqueData;
        }

        public void SetOpaqueData(BaseOpaqueData opaqueData)
        {
            this.opaqueData = opaqueData;
        }

        public override int GetLength()
        {
            return this.opaqueData.GetLength();
        }

        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            this.opaqueData.Encode(buf);
            return ((ByteBuffer)(buf.flip()));
        }

        public override void Decode(ByteBuffer buf)
        {
            long len = base.DecodeLength(buf);
            if (((len > 0)
                        && (len <= buf.remaining())))
            {
                long eof = (buf.position() + len);
                if ((buf.position() < eof))
                {
                    this.opaqueData.Decode(buf, len);
                }

            }

        }

        public virtual bool Matches(optionExpression expression)
        {
            if ((expression == null))
            {
                return false;
            }

            if ((expression.code != this.code))
            {
                return false;
            }

            return OpaqueDataUtil.Matches(expression, this.opaqueData);

        }

        public override bool Equals(object obj)
        {
            if ((this == obj))
            {
                return true;
            }

            if ((obj == null))
            {
                return false;
            }

            if (this.GetType().Name != obj.GetType().Name)
            {
                return false;
            }

            if ((obj is BaseOpaqueDataOption))
            {
                BaseOpaqueDataOption that = ((BaseOpaqueDataOption)(obj));
                if ((that.opaqueData != null))
                {
                    if ((this.opaqueData.GetAscii() != null))
                    {
                        return this.opaqueData.GetAscii().ToUpper() == that.opaqueData.GetAscii().ToUpper();
                    }
                    else
                    {
                        return this.opaqueData.GetHex().SequenceEqual(that.opaqueData.GetHex());
                    }

                }

            }

            return false;
        }

        //public String toString()
        //{
        //    StringBuilder sb = new StringBuilder(Util.LINE_SEPARATOR);
        //    sb.append(base.getName());
        //    sb.append(": data=");
        //    if ((this.opaqueData != null))
        //    {
        //        sb.append(this.opaqueData.toString());
        //    }

        //    return sb.toString();
        //}
    }
}
