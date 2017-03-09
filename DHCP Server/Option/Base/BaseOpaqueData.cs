using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace PIXIS.DHCP.Option.Base
{
    public class BaseOpaqueData
    {
        /** The log. */
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string ascii = "";
        private byte[] hex = null;

        public BaseOpaqueData()
        {
            // empty constructor
        }
        public BaseOpaqueData(opaqueData opaqueData)
        {
            if (opaqueData != null)
            {
                SetAscii(opaqueData.asciiValue);
                SetHex(opaqueData.hexValue);
            }
        }
        public BaseOpaqueData(string ascii)
        {
            this.ascii = ascii;
        }
        public BaseOpaqueData(byte[] hex)
        {
            this.hex = hex;
        }
        public string GetAscii()
        {
            return ascii;
        }
        public void SetAscii(string ascii)
        {
            this.ascii = ascii;
        }
        public byte[] GetHex()
        {
            return hex;
        }
        public void SetHex(byte[] hex)
        {
            this.hex = hex;
        }
        public int GetLength()
        {
            if (!string.IsNullOrEmpty(ascii))
            {
                return ascii.Length;
            }
            else
            {
                return hex.Length;
            }
        }
        public void Encode(ByteBuffer buf)
        {
            if (!string.IsNullOrEmpty(ascii))
            {
                var bytes = Encoding.ASCII.GetBytes(ascii);
                buf.put(bytes, 0, bytes.Length);
            }
            else if (hex != null)
            {
                buf.put(hex, 0, hex.Length);
            }
        }
        //public void encodeLengthAndData(ByteBuffer buf)
        //{
        //    if (ascii != null)
        //    {
        //        buf.putShort((short)ascii.length());
        //        buf.put(ascii.getBytes());
        //    }
        //    else
        //    {
        //        buf.putShort((short)hex.length);
        //        buf.put(hex);
        //    }
        //}
        public void Decode(ByteBuffer buf, long len)
        {
            if (len > 0)
            {
                byte[] data = new byte[len];
                buf.get(data, 0, data.Length);
                string str = System.Text.ASCIIEncoding.Default.GetString(data);
                Regex r = new Regex("^.[A-Za-z0-9.?!:'\\s]+$");
                Match m = r.Match(str);
                if (m.Success)
                {
                    ascii = str;
                }
                else
                {
                    hex = data;
                }
            }
        }
        //public void decodeLengthAndData(ByteBuffer buf)
        //{
        //    int len = Util.getUnsignedShort(buf);
        //    if (len > 0)
        //    {
        //        decode(buf, len);
        //    }
        //}

        // for expression matching
        public bool Matches(opaqueData that, @operator op)
        {
            //if (that != null)
            //{
            //    String expAscii = that.getAsciiValue();
            //    String myAscii = getAscii();
            //    if ((expAscii != null) && (myAscii != null))
            //    {
            //        if (op.equals(Operator.EQUALS))
            //        {
            //            return myAscii.equalsIgnoreCase(expAscii);
            //        }
            //        else if (op.equals(Operator.STARTS_WITH))
            //        {
            //            return myAscii.startsWith(expAscii);
            //        }
            //        else if (op.equals(Operator.CONTAINS))
            //        {
            //            return myAscii.contains(expAscii);
            //        }
            //        else if (op.equals(Operator.ENDS_WITH))
            //        {
            //            return myAscii.endsWith(expAscii);
            //        }
            //        else if (op.equals(Operator.REG_EXP))
            //        {
            //            return myAscii.matches(expAscii);
            //        }
            //        else
            //        {
            //            log.error("Unsupported expression operator: " + op);
            //            return false;
            //        }
            //    }
            //    else if ((expAscii == null) && (myAscii == null))
            //    {
            //        byte[] expHex = that.getHexValue();
            //        byte[] myHex = getHex();
            //        if ((expHex != null) && (myHex != null))
            //        {
            //            if (op.equals(Operator.EQUALS))
            //            {
            //                return Arrays.equals(myHex, expHex);
            //            }
            //            else if (op.equals(Operator.STARTS_WITH))
            //            {
            //                if (myHex.length >= expHex.length)
            //                {
            //                    for (int i = 0; i < expHex.length; i++)
            //                    {
            //                        if (myHex[i] != expHex[i])
            //                        {
            //                            return false;
            //                        }
            //                    }
            //                    return true;    // if we get here, it matches
            //                }
            //                else
            //                {
            //                    return false;   // exp length too long
            //                }
            //            }
            //            else if (op.equals(Operator.CONTAINS))
            //            {
            //                if (myHex.length >= expHex.length)
            //                {
            //                    int j = 0;
            //                    for (int i = 0; i < myHex.length; i++)
            //                    {
            //                        if (myHex[i] == expHex[j])
            //                        {
            //                            // found a potential match
            //                            j++;
            //                            boolean matches = true;
            //                            for (int ii = i + 1; ii < myHex.length; ii++)
            //                            {
            //                                if (myHex[ii] != expHex[j++])
            //                                {
            //                                    matches = false;
            //                                    break;
            //                                }
            //                            }
            //                            if (matches)
            //                            {
            //                                return true;
            //                            }
            //                            j = 0;    // reset to start of exp
            //                        }
            //                    }
            //                    return false;    // if we get here, it didn't match
            //                }
            //                else
            //                {
            //                    return false;   // exp length too long
            //                }
            //            }
            //            else if (op.equals(Operator.ENDS_WITH))
            //            {
            //                if (myHex.length >= expHex.length)
            //                {
            //                    for (int i = myHex.length - 1;
            //                         i >= myHex.length - expHex.length;
            //                         i--)
            //                    {
            //                        if (myHex[i] != expHex[i])
            //                        {
            //                            return false;
            //                        }
            //                    }
            //                    return true;    // if we get here, it matches
            //                }
            //                else
            //                {
            //                    return false;   // exp length too long
            //                }
            //            }
            //            else if (op.equals(Operator.REG_EXP))
            //            {
            //                log.Error("Regular expression operator not valid for hex opaque opaqueData");
            //                return false;
            //            }
            //            else
            //            {
            //                log.Error("Unsupported expression operator: " + op);
            //                return false;
            //            }
            //        }
            //    }
            //}
            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (ascii != null)
            {
                sb.Append(ascii);
            }
            else
            {
                sb.Append(Util.ToHexString(hex));
            }
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
            result = prime * result + ((ascii == null) ? 0 : ascii.GetHashCode());
            result = prime * result + hex.GetHashCode();
            return result;
        }

        public override bool Equals(Object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if ((this.GetType() != obj.GetType()))
                return false;

            BaseOpaqueData other = (BaseOpaqueData)obj;
            if (ascii == null)
            {
                if (other.ascii != null)
                    return false;
            }
            else if (!ascii.Equals(other.ascii))
                return false;
            if (!Array.Equals(hex, other.hex))
                return false;
            return true;
        }
        public void EncodeLengthAndData(ByteBuffer buf)
        {
            if (ascii != null)
            {
                buf.putShort((short)ascii.Length);
                buf.put(Encoding.ASCII.GetBytes(ascii));
            }
            else
            {
                buf.putShort((short)hex.Length);
                buf.put(hex);
            }
        }
        public void DecodeLengthAndData(ByteBuffer buf)
        {
            int len = Util.GetUnsignedShort(buf);
            if (len > 0)
            {
                Decode(buf, len);
            }
        }
    }
}