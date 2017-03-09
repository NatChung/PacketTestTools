using DHCP_Server.Option.Base;
using DHCP_Server.Xml;
using NLog;

namespace DHCP_Server.Option
{
    public class OpaqueDataOptionType
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        /**
         * Matches.
         * 
         * @param expression the expression
         * @param myOpaque the my opaque
         * 
         * @return true, if successful
         */
        public static bool Matches(optionExpression expression, BaseOpaqueData myOpaque)
        {
            if (expression == null)
                return false;

            opaqueData data = (opaqueData)expression.Item;
            if (data == null)
                return false;

            @operator op = expression.@operator;
            return Matches(myOpaque, data, op);
        }

        public static bool Matches(BaseOpaqueData myOpaque, opaqueData that, @operator op)
        {
            if (that != null)
            {
                string expAscii = that.asciiValue;
                string myAscii = myOpaque.GetAscii();
                if ((expAscii != null) && (myAscii != null))
                {
                    if (op.Equals(@operator.equals))
                    {
                        return myAscii.equalsIgnoreCase(expAscii);
                    }
                    else if (op.Equals(@operator.startsWith))
                    {
                        return myAscii.startsWith(expAscii);
                    }
                    else if (op.Equals(@operator.contains))
                    {
                        return myAscii.@operator(expAscii);
                    }
                    else if (op.Equals(@operator.endsWith))
                    {
                        return myAscii.endsWith(expAscii);
                    }
                    else if (op.Equals(@operator.regExp))
                    {
                        return myAscii.matches(expAscii);
                    }
                    else
                    {
                        log.error("Unsupported expression operator: " + op);
                        return false;
                    }
                }
                else if ((expAscii == null) && (myAscii == null))
                {
                    byte[] expHex = that.getHexValue();
                    byte[] myHex = myOpaque.getHex();
                    if ((expHex != null) && (myHex != null))
                    {
                        if (op.equals(Operator.EQUALS))
                        {
                            return Arrays.equals(myHex, expHex);
                        }
                        else if (op.equals(Operator.STARTS_WITH))
                        {
                            if (myHex.length >= expHex.length)
                            {
                                for (int i = 0; i < expHex.length; i++)
                                {
                                    if (myHex[i] != expHex[i])
                                    {
                                        return false;
                                    }
                                }
                                return true;    // if we get here, it matches
                            }
                            else
                            {
                                return false;   // exp length too long
                            }
                        }
                        else if (op.equals(Operator.CONTAINS))
                        {
                            if (myHex.length >= expHex.length)
                            {
                                int j = 0;
                                for (int i = 0; i < myHex.length; i++)
                                {
                                    if (myHex[i] == expHex[j])
                                    {
                                        // found a potential match
                                        j++;
                                        boolean matches = true;
                                        for (int ii = i + 1; ii < myHex.length; ii++)
                                        {
                                            if (myHex[ii] != expHex[j++])
                                            {
                                                matches = false;
                                                break;
                                            }
                                        }
                                        if (matches)
                                        {
                                            return true;
                                        }
                                        j = 0;    // reset to start of exp
                                    }
                                }
                                return false;    // if we get here, it didn't match
                            }
                            else
                            {
                                return false;   // exp length too long
                            }
                        }
                        else if (op.equals(Operator.ENDS_WITH))
                        {
                            if (myHex.length >= expHex.length)
                            {
                                for (int i = myHex.length - 1;
                                     i >= myHex.length - expHex.length;
                                     i--)
                                {
                                    if (myHex[i] != expHex[i])
                                    {
                                        return false;
                                    }
                                }
                                return true;    // if we get here, it matches
                            }
                            else
                            {
                                return false;   // exp length too long
                            }
                        }
                        else if (op.equals(Operator.REG_EXP))
                        {
                            log.error("Regular expression operator not valid for hex opaque opaqueData");
                            return false;
                        }
                        else
                        {
                            log.error("Unsupported expression operator: " + op);
                            return false;
                        }
                    }
                }
            }
            return false;
        }

        /**
         * To string.
         * 
         * @param opaque the opaque
         * 
         * @return the string
         */
        public static String toString(OpaqueData opaque)
        {
            if (opaque == null)
                return null;

            String ascii = opaque.getAsciiValue();
            if (ascii != null)
                return ascii;
            else
                return Util.toHexString(opaque.getHexValue());
        }

        /**
         * Equals.
         * 
         * @param opaque1 the opaque1
         * @param opaque2 the opaque2
         * 
         * @return true, if successful
         */
        public static boolean equals(BaseOpaqueData opaque1, BaseOpaqueData opaque2)
        {
            if ((opaque1 == null) || (opaque2 == null))
                return false;

            String ascii1 = opaque1.getAscii();
            if (ascii1 != null)
            {
                String ascii2 = opaque2.getAscii();
                if (ascii1.equalsIgnoreCase(ascii2))
                {
                    return true;
                }
            }
            else
            {
                return Arrays.equals(opaque1.getHex(), opaque2.getHex());
            }
            return false;
        }

        /**
         * Generate the DHCPv6 Server's DUID-LLT.  See sections 9 and 22.3 of RFC 3315.
         * 
         * @return the opaque opaqueData
         */
        public static OpaqueData generateDUID_LLT()
        {
            OpaqueData opaque = null;
            try
            {
                Enumeration<NetworkInterface> intfs = NetworkInterface.getNetworkInterfaces();
                if (intfs != null)
                {
                    while (intfs.hasMoreElements())
                    {
                        NetworkInterface intf = intfs.nextElement();
                        if (intf.isUp() && !intf.isLoopback() && !intf.isPointToPoint() && !intf.isVirtual())
                        {
                            opaque = OpaqueData.Factory.newInstance();
                            ByteBuffer bb = ByteBuffer.allocate(intf.getHardwareAddress().length + 8);
                            bb.putShort((short)1);  // DUID based on LLT
                            bb.putShort((short)1);  // assume ethernet
                            bb.putInt((int)(System.currentTimeMillis() / 1000));    // seconds since the Epoch
                            bb.put(intf.getHardwareAddress());
                            opaque.setHexValue(bb.array());
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.error("Failed to generate DUID-LLT: " + ex);
            }
            return opaque;
        }

        /**
         * The main method.
         * 
         * @param args the arguments
         */
        public static void main(String[] args)
        {
            /*
            OpaqueData o1 = OpaqueData.Factory.newInstance();
            OpaqueData o2 = OpaqueData.Factory.newInstance();
            o1.setHexValue(new byte[] { (byte)0xde, (byte)0xbb, (byte)0x1e });
            o2.setHexValue(new byte[] { (byte)0xde, (byte)0xbb, (byte)0x1e });
            System.out.println(equals(o1,o2));
            */
            OpaqueData opaque = generateDUID_LLT();
            System.out.println(OpaqueDataUtil.toString(opaque));
        }
    }
}