using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Option.Base
{
    public abstract class BaseStringOption : BaseDhcpOption, DhcpComparableOption
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string _string;
        public BaseStringOption() : base()
        { }
        public BaseStringOption(stringOptionType stringOption)
            : base()
        {
            if (stringOption != null)
            {
                _string = stringOption.@string;
            }
        }

        public string GetString()
        {
            return _string;
        }

        public override void Decode(ByteBuffer buf)
        {
            int len = base.DecodeLength(buf);
            if ((len > 0) && (len <= buf.remaining()))
            {
                byte[] b = buf.getBytes(len);
                _string = Encoding.ASCII.GetString(b);
            }
        }
        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            if (_string != null)
            {
                buf.put(Encoding.ASCII.GetBytes(_string));
            }
            return (ByteBuffer)buf.flip();
        }
        public override int GetLength()
        {
            int len = 0;
            if (_string != null)
            {
                len = _string.Length;
            }
            return len;
        }
        public bool Matches(optionExpression expression)
        {
            throw new NotImplementedException();
        }
    }
}
