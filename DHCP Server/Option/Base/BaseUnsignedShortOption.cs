using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option.Base
{
    public class BaseUnsignedShortOption : BaseDhcpOption, DhcpComparableOption
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected int unsignedShort;

        /**
         * Instantiates a new unsigned short option.
         */
        public BaseUnsignedShortOption() : this(null)
        {
        }

        /**
         * Instantiates a new unsigned short option.
         * 
         * @param uShortOption the elapsed time option
         */
        public BaseUnsignedShortOption(unsignedShortOptionType uShortOption) : base()
        {
            if (uShortOption != null)
            {
                unsignedShort = uShortOption.unsignedShort;
            }
        }

        public int GetUnsignedShort()
        {
            return unsignedShort;
        }

        public void SetUnsignedShort(int unsignedShort)
        {
            this.unsignedShort = unsignedShort;
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.DhcpOption#getLength()
         */
        public override int GetLength()
        {
            return 2;   // always two bytes (short)
        }

        public bool Matches(optionExpression expression)
        {
            if (expression == null)
                return false;
            if (expression.code != this.code)
                return false;

            unsignedShortOptionType exprOption = (unsignedShortOptionType)expression.Item;
            if (exprOption != null)
            {
                int exprUshort = exprOption.unsignedShort;
                @operator op = expression.@operator;
                if (op.Equals(@operator.equals))
                {
                    return (unsignedShort == exprUshort);
                }
                else if (op.Equals(@operator.lessThan))
                {
                    return (unsignedShort < exprUshort);
                }
                else if (op.Equals(@operator.lessThanOrEqual))
                {
                    return (unsignedShort <= exprUshort);
                }
                else if (op.Equals(@operator.greaterThan))
                {
                    return (unsignedShort > exprUshort);
                }
                else if (op.Equals(@operator.greaterThanOrEqual))
                {
                    return (unsignedShort >= exprUshort);
                }
                else
                {
                    log.Warn("Unsupported expression operator: " + op);
                }
            }

            // then see if we have an opaque option
            opaqueDataOptionType opaqueOption = (opaqueDataOptionType)expression.Item;
            if (opaqueOption != null)
            {
                opaqueData opaque = opaqueOption.opaqueData;
                if (opaque != null)
                {
                    string ascii = opaque.asciiValue;
                    if (ascii != null)
                    {
                        try
                        {
                            // need an Integer to handle unsigned short
                            if (unsignedShort == int.Parse(ascii))
                            {
                                return true;
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error("Invalid unsigned short ASCII value for OpaqueData: " + ascii);
                        }
                    }
                    else
                    {
                        byte[] hex = opaque.hexValue;
                        if ((hex != null) &&
                             (hex.Length >= 1) && (hex.Length <= 2))
                        {
                            int hexUnsignedShort = Convert.ToInt32(Util.ToHexString(hex), 16);
                            if (unsignedShort == hexUnsignedShort)
                            {
                                return true;
                            }
                        }
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
            sb.Append(": unsignedShort=");
            sb.Append(unsignedShort);
            return sb.ToString();
        }

        public override void Decode(ByteBuffer buf)
        {
            int len = base.DecodeLength(buf);
            if ((len > 0) && (len <= buf.remaining()))
            {
                unsignedShort = Util.GetUnsignedShort(buf);
            }
        }

        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            buf.putShort((short)unsignedShort);
            return (ByteBuffer)buf.flip();
        }
    }
}
