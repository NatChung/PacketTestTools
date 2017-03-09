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
    public class DhcpV6StatusCodeOption : BaseDhcpOption, DhcpComparableOption
    {
        private int statusCode;
        private string message;

        public DhcpV6StatusCodeOption() : this(null)
        {
        }
        public DhcpV6StatusCodeOption(v6StatusCodeOption statusCodeOption) : base()
        {
            if (statusCodeOption != null)
            {
                statusCode = statusCodeOption.statusCode;
                message = statusCodeOption.message;
            }
            SetCode(DhcpConstants.V6OPTION_STATUS_CODE);
        }

        public int GetStatusCode()
        {
            return statusCode;
        }

        public void SetStatusCode(int statusCode)
        {
            this.statusCode = statusCode;
        }

        public string GetMessage()
        {
            return message;
        }

        public void SetMessage(string message)
        {
            this.message = message;
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.DhcpOption#getLength()
         */
        public override int GetLength()
        {
            int len = 2;    // status code
            if (message != null)
            {
                len += message.Length;
            }
            return len;
        }
        //public ByteBuffer Encode()
        //{
        //    ByteBuffer buf = base.EncodeCodeAndLength();
        //    buf.putShort((short)statusCode);
        //    if (message != null)
        //    {
        //        buf.put(message.GetBytes());
        //    }
        //    return (ByteBuffer)buf.flip();
        //}

        ///* (non-Javadoc)
        // * @see com.jagornet.dhcpv6.option.Decodable#decode(java.nio.ByteBuffer)
        // */
        //public void Decode(ByteBuffer buf)
        //{
        //    int len = base.DecodeLength(buf);
        //    if ((len > 0) && (len <= buf.remaining()))
        //    {
        //        long eof = buf.position() + len;
        //        if (buf.position() < eof)
        //        {
        //            statusCode = Util.DetUnsignedShort(buf);
        //            if (buf.position() < eof)
        //            {
        //                if (len > 2)
        //                {
        //                    byte[] data = new byte[len - 2];  // minus 2 for the status code
        //                    buf.get(data);
        //                    message = new string(data);
        //                }
        //            }
        //        }
        //    }
        //}

        /**
         * Matches only the status code, not the message text.
         * 
         * @param expression the expression
         * 
         * @return true, if matches
         */
        public bool Matches(optionExpression expression)
        {
            if (expression == null)
                return false;
            if (expression.code != this.GetCode())
                return false;


            /*
                    OpaqueData opaque = expression.getData();
                    if (opaque != null) {
                        String ascii = opaque.getAsciiValue();
                        if (ascii != null) {
                            try {
                                // need an int to handle unsigned short
                                if (statusCodeOption.getStatusCode() == Integer.parseInt(ascii)) {
                                    return true;
                                }
                            }
                            catch (NumberFormatException ex) { }
                        }
                        else {
                            byte[] hex = opaque.getHexValue();
                            if ( (hex != null) && 
                                 (hex.length >= 1) && (hex.length <= 2) ) {
                                int hexInt = Integer.parseInt(Util.toHexString(hex));
                                if (statusCodeOption.getStatusCode() == hexInt) {
                                    return true;
                                }
                            }
                        }
                    }
            */
            return false;
        }

        /* (non-Javadoc)
         * @see java.lang.Object#toString()
         */
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Util.LINE_SEPARATOR);
            sb.Append(base.GetName());
            sb.Append(": statusCode=");
            sb.Append(statusCode);
            sb.Append(" message=");
            sb.Append(message);
            return sb.ToString();
        }

        public override void Decode(ByteBuffer buf)
        {
            int len = base.DecodeLength(buf);
            if ((len > 0) && (len <= buf.remaining()))
            {
                long eof = buf.position() + len;
                if (buf.position() < eof)
                {
                    statusCode = Util.GetUnsignedShort(buf);
                    if (buf.position() < eof)
                    {
                        if (len > 2)
                        {
                            byte[] data = buf.getBytes(len - 2);
                            message = Encoding.ASCII.GetString(data);
                        }
                    }
                }
            }
        }

        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            buf.putShort((short)statusCode);
            if (message != null)
            {
                buf.put(Encoding.ASCII.GetBytes(message));
            }
            return (ByteBuffer)buf.flip();
        }
    }
}
