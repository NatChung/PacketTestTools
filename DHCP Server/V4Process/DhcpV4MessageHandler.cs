using PIXIS.DHCP.Option.V4;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PIXIS.DHCP.V4Process
{
    public class DhcpV4MessageHandler
    {
        /** The log. */
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static DhcpV4Message HandleMessage(IPAddress localAddress, DhcpV4Message dhcpMessage)
        {
            DhcpV4Message replyMessage = null;
            if (dhcpMessage.GetOp() == DhcpConstants.V4_OP_REQUEST)
            {
                IPAddress linkAddress = null;
                if (dhcpMessage.GetGiAddr().Equals(DhcpConstants.ZEROADDR_V4))
                {
                    linkAddress = localAddress;
                    //log.Info("Handling client request on local client link address: " +
                    //        linkAddress.ToString());
                }
                else
                {
                    linkAddress = dhcpMessage.GetGiAddr();
                    log.Info("Handling client request on remote client link address: " +
                            linkAddress.ToString());
                }
                DhcpV4MsgTypeOption msgTypeOption = (DhcpV4MsgTypeOption)
                        dhcpMessage.GetDhcpOption(DhcpConstants.V4OPTION_MESSAGE_TYPE);
                if (msgTypeOption != null)
                {
                    short msgType = msgTypeOption.GetUnsignedByte();
                    DhcpV4MessageProcessor processor = null;
                    switch (msgType)
                    {
                        case DhcpConstants.V4MESSAGE_TYPE_DISCOVER:
                            processor = new DhcpV4DiscoverProcessor(dhcpMessage, linkAddress);
                            break;
                        case DhcpConstants.V4MESSAGE_TYPE_REQUEST:
                            processor = new DhcpV4RequestProcessor(dhcpMessage, linkAddress);
                            break;
                        case DhcpConstants.V4MESSAGE_TYPE_DECLINE:
                            processor = new DhcpV4DeclineProcessor(dhcpMessage, linkAddress);
                            break;
                        case DhcpConstants.V4MESSAGE_TYPE_RELEASE:
                            processor = new DhcpV4ReleaseProcessor(dhcpMessage, linkAddress);
                            break;
                        case DhcpConstants.V4MESSAGE_TYPE_INFORM:
                            processor = new DhcpV4InformProcessor(dhcpMessage, linkAddress);
                            break;
                        default:
                            log.Error("Unknown message type.");
                            break;
                    }
                    if (processor != null)
                    {
                        return processor.ProcessMessage(localAddress);
                    }
                    else
                    {
                        log.Error("No processor found for message type: " + msgType);
                    }
                }
                else
                {
                    log.Error("No message type option found in request.");
                }
                return null;
            }
            else
            {
                log.Error("Unsupported op code: " + dhcpMessage.GetOp());
            }
            return replyMessage;
        }
    }

}
