/*
 * Copyright 2009-2014 Jagornet Technologies, LLC.  All Rights Reserved.
 *
 * This software is the proprietary information of Jagornet Technologies, LLC. 
 * Use is subject to license terms.
 *
 */

/*
 *   This file OpaqueDataUtil.java is part of Jagornet DHCP.
 *
 *   Jagornet DHCP is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   Jagornet DHCP is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with Jagornet DHCP.  If not, see <http://www.gnu.org/licenses/>.
 *
 */
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option
{
    public class OpaqueDataUtil
    {
        /** The log. */
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

            opaqueDataOptionType opaqueOption = (opaqueDataOptionType)expression.Item;
            if (opaqueOption == null)
                return false;

            @operator op = expression.@operator;
            return Matches(myOpaque, opaqueOption.opaqueData, op);
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
                        return myAscii.ToUpper() == expAscii.ToUpper();
                    }
                    else if (op.Equals(@operator.startsWith))
                    {
                        return myAscii.StartsWith(expAscii);
                    }
                    else if (op.Equals(@operator.contains))
                    {
                        return myAscii.Contains(expAscii);
                    }
                    else if (op.Equals(@operator.endsWith))
                    {
                        return myAscii.EndsWith(expAscii);
                    }
                    else if (op.Equals(@operator.regExp))
                    {
                        Match m = Regex.Match(myAscii, expAscii);
                        return m.Success;
                    }
                    else
                    {
                        log.Error("Unsupported expression operator: " + op);
                        return false;
                    }
                }
                else if ((expAscii == null) && (myAscii == null))
                {
                    byte[] expHex = that.hexValue;
                    byte[] myHex = myOpaque.GetHex();
                    if ((expHex != null) && (myHex != null))
                    {
                        if (op.Equals(@operator.equals))
                        {
                            return Array.Equals(myHex, expHex);
                        }
                        else if (op.Equals(@operator.startsWith))
                        {
                            if (myHex.Length >= expHex.Length)
                            {
                                for (int i = 0; i < expHex.Length; i++)
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
                        else if (op.Equals(@operator.contains))
                        {
                            if (myHex.Length >= expHex.Length)
                            {
                                int j = 0;
                                for (int i = 0; i < myHex.Length; i++)
                                {
                                    if (myHex[i] == expHex[j])
                                    {
                                        // found a potential match
                                        j++;
                                        bool matches = true;
                                        for (int ii = i + 1; ii < myHex.Length; ii++)
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
                        else if (op.Equals(@operator.endsWith))
                        {
                            if (myHex.Length >= expHex.Length)
                            {
                                for (int i = myHex.Length - 1;
                                     i >= myHex.Length - expHex.Length;
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
                        else if (op.Equals(@operator.regExp))
                        {
                            log.Error("Regular expression operator not valid for hex opaque opaqueData");
                            return false;
                        }
                        else
                        {
                            log.Error("Unsupported expression operator: " + op);
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
        public static string ToString(opaqueData opaque)
        {
            if (opaque == null)
                return null;

            string ascii = opaque.asciiValue;
            if (ascii != null)
                return ascii;
            else
                return Util.ToHexString(opaque.hexValue);
        }

        /**
         * Equals.
         * 
         * @param opaque1 the opaque1
         * @param opaque2 the opaque2
         * 
         * @return true, if successful
         */
        public static bool Equals(BaseOpaqueData opaque1, BaseOpaqueData opaque2)
        {
            if ((opaque1 == null) || (opaque2 == null))
                return false;

            String ascii1 = opaque1.GetAscii();
            if (ascii1 != null)
            {
                String ascii2 = opaque2.GetAscii();
                if (ascii1.ToUpper() == ascii2.ToUpper())
                {
                    return true;
                }
            }
            else
            {
                return Array.Equals(opaque1.GetHex(), opaque2.GetHex());
            }
            return false;
        }

        ///**
        // * Generate the DHCPv6 Server's DUID-LLT.  See sections 9 and 22.3 of RFC 3315.
        // * 
        // * @return the opaque opaqueData
        // */
        public static opaqueData GenerateDUID_LLT()
        {
            opaqueData opaque = null;
            try
            {
                var intfs = NetworkInterface.GetAllNetworkInterfaces().GetEnumerator();
                if (intfs != null)
                {
                    while (intfs.MoveNext())
                    {
                        NetworkInterface intf = intfs.Current as NetworkInterface;
                        if (intf.OperationalStatus == OperationalStatus.Up && !(intf.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                            && !(intf.NetworkInterfaceType == NetworkInterfaceType.Ppp)// && !intf.isVirtual()
                            )
                        {
                            opaque = new opaqueData();
                            ByteBuffer bb = ByteBuffer.allocate(intf.GetPhysicalAddress().GetAddressBytes().Length + 8);
                            bb.putShort((short)1);  // DUID based on LLT
                            bb.putShort((short)1);  // assume ethernet
                            bb.putInt((int)(GetCurrentMilli() / 1000));    // seconds since the Epoch
                            bb.put(intf.GetPhysicalAddress().GetAddressBytes());
                            opaque.hexValue = bb.getAllBytes();
                            break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                log.Error("Failed to generate DUID-LLT: " + ex.Message);
            }
            return opaque;
        }
        public static long GetCurrentMilli()
        {
            DateTime Jan1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan javaSpan = DateTime.UtcNow - Jan1970;
            return Convert.ToInt64(javaSpan.TotalMilliseconds);
        }
    }

}
