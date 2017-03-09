using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Xml;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option.Generic
{
    public class GenericOptionFactory
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static DhcpOption GetDhcpOption(optionDefType optionDef)
        {
            int code = optionDef.code;
            string name = optionDef.name;
            //if (optionDef.ItemElementName == ItemChoiceType.domainNameListOption)
            //{
            //    return new GenericDomainNameListOption(code, name, optionDef.getDomainNameListOption());
            //}
            //else if (optionDef.isSetDomainNameOption())
            //{
            //    return new GenericDomainNameOption(code, name, optionDef.getDomainNameOption());
            //}
            //else if (optionDef.isSetIpAddressListOption())
            //{
            //    return new GenericIpAddressListOption(code, name, optionDef.getIpAddressListOption());
            //}
            //else if (optionDef.isSetIpAddressOption())
            //{
            //    return new GenericIpAddressOption(code, name, optionDef.getIpAddressOption());
            //}
            //else if (optionDef.isSetOpaqueDataListOption())
            //{
            //    return new GenericOpaqueDataListOption(code, name, optionDef.getOpaqueDataListOption());
            //}
            //else if (optionDef.isSetOpaqueDataOption())
            //{
            //    return new GenericOpaqueDataOption(code, name, optionDef.getOpaqueDataOption());
            //}
            //else if (optionDef.isSetStringOption())
            //{
            //    return new GenericStringOption(code, name, optionDef.getStringOption());
            //}
            //else if (optionDef.isSetUByteOption())
            //{
            //    return new GenericUnsignedByteOption(code, name, optionDef.getUByteOption());
            //}
            //else if (optionDef.isSetUIntOption())
            //{
            //    return new GenericUnsignedIntOption(code, name, optionDef.getUIntOption());
            //}
            //else if (optionDef.isSetUShortListOption())
            //{
            //    return new GenericUnsignedShortListOption(code, name, optionDef.getUShortListOption());
            //}
            //else if (optionDef.isSetUShortOption())
            //{
            //    return new GenericUnsignedShortOption(code, name, optionDef.getUShortOption());
            //}
            //else
            //{
            //    log.error("Unknown generic option type");
            //}
            return null;
        }

        /**
         * Convert a list of XML Generic options to a map of DhcpOptions
         * 
         * @param genericOptions
         * @return a map of generic options
         */
        public static Dictionary<int, DhcpOption> GenericOptions(List<optionDefType> genericOptions)
        {
            Dictionary<int, DhcpOption> optMap = new Dictionary<int, DhcpOption>();
            //    if (genericOptions != null)
            //    {
            //        List<optionDefType> optionDefs = genericOptions.getOptionDefList();
            //        if ((optionDefs != null) && !optionDefs.isEmpty())
            //        {
            //            for (OptionDefType optionDefType : optionDefs)
            //            {

            //                int code = optionDefType.getCode();
            //                String name = optionDefType.getName();

            //                // the XML schema defines the optionDefType as a choice,
            //                // so we must determine which generic option is set

            //                if (optionDefType.isSetDomainNameListOption())
            //                {
            //                    DomainNameListOptionType domainNameListOption =
            //                        optionDefType.getDomainNameListOption();
            //                    if (domainNameListOption != null)
            //                    {
            //                        GenericDomainNameListOption dhcpOption =
            //                            new GenericDomainNameListOption(code, name, domainNameListOption);
            //                        optMap.put(code, dhcpOption);
            //                        continue;
            //                    }
            //                }

            //                if (optionDefType.isSetDomainNameOption())
            //                {
            //                    DomainNameOptionType domainNameOption =
            //                        optionDefType.getDomainNameOption();
            //                    if (domainNameOption != null)
            //                    {
            //                        GenericDomainNameOption dhcpOption =
            //                            new GenericDomainNameOption(code, name, domainNameOption);
            //                        optMap.put(code, dhcpOption);
            //                        continue;
            //                    }
            //                }

            //                if (optionDefType.isSetIpAddressListOption())
            //                {
            //                    IpAddressListOptionType ipAddressListOption =
            //                        optionDefType.getIpAddressListOption();
            //                    if (ipAddressListOption != null)
            //                    {
            //                        GenericIpAddressListOption dhcpOption =
            //                            new GenericIpAddressListOption(code, name, ipAddressListOption);
            //                        optMap.put(code, dhcpOption);
            //                        continue;
            //                    }
            //                }

            //                if (optionDefType.isSetIpAddressOption())
            //                {
            //                    IpAddressOptionType ipAddressOption =
            //                        optionDefType.getIpAddressOption();
            //                    if (ipAddressOption != null)
            //                    {
            //                        GenericIpAddressOption dhcpOption =
            //                            new GenericIpAddressOption(code, name, ipAddressOption);
            //                        optMap.put(code, dhcpOption);
            //                        continue;
            //                    }
            //                }

            //                if (optionDefType.isSetOpaqueDataListOption())
            //                {
            //                    OpaqueDataListOptionType opaqueDataListOption =
            //                        optionDefType.getOpaqueDataListOption();
            //                    if (opaqueDataListOption != null)
            //                    {
            //                        GenericOpaqueDataListOption dhcpOption =
            //                            new GenericOpaqueDataListOption(code, name, opaqueDataListOption);
            //                        optMap.put(code, dhcpOption);
            //                        continue;
            //                    }
            //                }

            //                if (optionDefType.isSetOpaqueDataOption())
            //                {
            //                    OpaqueDataOptionType opaqueDataOption =
            //                        optionDefType.getOpaqueDataOption();
            //                    if (opaqueDataOption != null)
            //                    {
            //                        GenericOpaqueDataOption dhcpOption =
            //                            new GenericOpaqueDataOption(code, name, opaqueDataOption);
            //                        optMap.put(code, dhcpOption);
            //                        continue;
            //                    }
            //                }

            //                if (optionDefType.isSetStringOption())
            //                {
            //                    StringOptionType stringOption =
            //                        optionDefType.getStringOption();
            //                    if (stringOption != null)
            //                    {
            //                        GenericStringOption dhcpOption =
            //                            new GenericStringOption(code, name, stringOption);
            //                        optMap.put(code, dhcpOption);
            //                        continue;
            //                    }
            //                }

            //                if (optionDefType.isSetUByteListOption())
            //                {
            //                    UnsignedByteListOptionType uByteListOption =
            //                        optionDefType.getUByteListOption();
            //                    if (uByteListOption != null)
            //                    {
            //                        GenericUnsignedByteListOption dhcpOption =
            //                            new GenericUnsignedByteListOption(code, name, uByteListOption);
            //                        optMap.put(code, dhcpOption);
            //                        continue;
            //                    }
            //                }

            //                if (optionDefType.isSetUByteOption())
            //                {
            //                    UnsignedByteOptionType uByteOption =
            //                        optionDefType.getUByteOption();
            //                    if (uByteOption != null)
            //                    {
            //                        GenericUnsignedByteOption dhcpOption =
            //                            new GenericUnsignedByteOption(code, name, uByteOption);
            //                        optMap.put(code, dhcpOption);
            //                        continue;
            //                    }
            //                }

            //                if (optionDefType.isSetUIntOption())
            //                {
            //                    UnsignedIntOptionType uIntOption =
            //                        optionDefType.getUIntOption();
            //                    if (uIntOption != null)
            //                    {
            //                        GenericUnsignedIntOption dhcpOption =
            //                            new GenericUnsignedIntOption(code, name, uIntOption);
            //                        optMap.put(code, dhcpOption);
            //                        continue;
            //                    }
            //                }

            //                if (optionDefType.isSetUShortListOption())
            //                {
            //                    UnsignedShortListOptionType uShortListOption =
            //                        optionDefType.getUShortListOption();
            //                    if (uShortListOption != null)
            //                    {
            //                        GenericUnsignedShortListOption dhcpOption =
            //                            new GenericUnsignedShortListOption(code, name, uShortListOption);
            //                        optMap.put(code, dhcpOption);
            //                        continue;
            //                    }
            //                }

            //                if (optionDefType.isSetUShortOption())
            //                {
            //                    UnsignedShortOptionType uShortOption =
            //                        optionDefType.getUShortOption();
            //                    if (uShortOption != null)
            //                    {
            //                        GenericUnsignedShortOption dhcpOption =
            //                            new GenericUnsignedShortOption(code, name, uShortOption);
            //                        optMap.put(code, dhcpOption);
            //                        continue;
            //                    }
            //                }

            //                if (optMap.get(code) instanceof BaseDhcpOption) {
            //                if (optionDefType.isSetV4())
            //                {
            //                    ((BaseDhcpOption)optMap.get(code)).setV4(true);
            //                }
            //            }
            //        }
            //    }
            //}
            return optMap;
        }

    }
}
