using PIXIS.DHCP.Message;
using PIXIS.DHCP.Option.V6;
using PIXIS.DHCP.Utility;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Request
{
    public class DhcpV6MessageHandler
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // NOTE: this is the magic method where the nio and net implementations come together
        public static DhcpV6Message HandleMessage(IPAddress localAddress, DhcpV6Message dhcpMessage, IPAddress v4IPAddress = null)
        {
            DhcpV6Message replyMessage = null;
            if (dhcpMessage is DhcpV6RelayMessage)
            {
                //if (dhcpMessage.GetMessageType() == DhcpConstants.V6MESSAGE_TYPE_RELAY_FORW)
                //{
                //    DhcpV6RelayMessage relayMessage = (DhcpV6RelayMessage)(dhcpMessage);
                //    replyMessage = DhcpV6MessageHandler.HandleRelayForward(relayMessage);
                //}
                //else
                //{
                //    _log.ErrorFormat("Unsupported message type: {0}", dhcpMessage.GetMessageType());
                //}
            }
            else
            {
                _log.InfoFormat("Handling client request on local client link address: {0}", localAddress.ToString());
                replyMessage = DhcpV6MessageHandler.HandleClientRequest(localAddress, dhcpMessage, v4IPAddress);
            }
            return replyMessage;
        }

        private static DhcpV6RelayMessage HandleRelayForward(DhcpV6RelayMessage relayMessage)
        {
            IPAddress linkAddr = relayMessage.GetLinkAddress();
            _log.Info(("Handling relay forward on link address: " + linkAddr.ToString()));
            DhcpV6RelayOption relayOption = relayMessage.GetRelayOption();
            if ((relayOption != null))
            {
                DhcpV6Message relayOptionMessage = relayOption.GetDhcpMessage();
                while ((relayOptionMessage != null))
                {
                    //  check what kind of message is in the option
                    if ((relayOptionMessage is DhcpV6RelayMessage))
                    {
                        //  encapsulated message is another relay message
                        DhcpV6RelayMessage anotherRelayMessage = ((DhcpV6RelayMessage)(relayOptionMessage));
                        //  flip this inner relay_forward into a relay_reply,
                        //  because we reuse the relay message "stack" for the reply
                        anotherRelayMessage.SetMessageType(DhcpConstants.V6MESSAGE_TYPE_RELAY_REPL);
                        //  reset the client link reference
                        linkAddr = anotherRelayMessage.GetLinkAddress();
                        //  reset the current relay option reference to the
                        //  encapsulated relay message's relay option
                        relayOption = anotherRelayMessage.GetRelayOption();
                        //  reset the relayOptionMessage reference to recurse
                        relayOptionMessage = relayOption.GetDhcpMessage();
                    }
                    else
                    {
                        //  we've peeled off all the layers of the relay message(s),
                        //  so now go handle the client request
                        _log.Info(("Handling client request on remote client link address: " + linkAddr.ToString()));
                        DhcpV6Message replyMessage = DhcpV6MessageHandler.HandleClientRequest(linkAddr, relayOptionMessage, null);
                        if ((replyMessage != null))
                        {
                            //  replace the original client request message inside
                            //  the relayed message with the generated Reply message
                            relayOption.SetDhcpMessage(replyMessage);
                            //  flip the outer-most relay_foward into a relay_reply
                            relayMessage.SetMessageType(DhcpConstants.V6MESSAGE_TYPE_RELAY_REPL);
                            //  return the relay message we started with, 
                            //  with each relay "layer" flipped from a relay_forward
                            //  to a relay_reply, and the lowest level relayOption
                            //  will contain our Reply for the client request
                            return relayMessage;
                        }

                        relayOptionMessage = null;
                        //  done with relayed messages
                    }

                }

            }
            else
            {
                _log.Error("Relay message does not contain a relay option");
            }

            //  if we get here, no reply was generated
            return null;
        }

        public static DhcpV6Message HandleClientRequest(IPAddress linkAddress, DhcpV6Message dhcpMessage, IPAddress clientV4IP)
        {
            BaseDhcpV6Processor processor = null;
            switch (dhcpMessage.GetMessageType())
            {
                case DhcpConstants.V6MESSAGE_TYPE_SOLICIT:
                    processor = new DhcpV6SolicitProcessor(dhcpMessage, linkAddress, clientV4IP);
                    break;
                case DhcpConstants.V6MESSAGE_TYPE_REQUEST:
                    processor = new DhcpV6RequestProcessor(dhcpMessage, linkAddress, clientV4IP);
                    break;
                case DhcpConstants.V6MESSAGE_TYPE_RENEW:
                    processor = new DhcpV6RenewProcessor(dhcpMessage, linkAddress, clientV4IP);
                    break;
                case DhcpConstants.V6MESSAGE_TYPE_RELEASE:
                    processor = new DhcpV6ReleaseProcessor(dhcpMessage, linkAddress);
                    break;
                case DhcpConstants.V6MESSAGE_TYPE_INFO_REQUEST:
                    processor = new DhcpV6InfoRequestProcessor(dhcpMessage, linkAddress);
                    break;
                case DhcpConstants.V6MESSAGE_TYPE_REBIND:
                    processor = new DhcpV6RebindProcessor(dhcpMessage, linkAddress, clientV4IP);
                    break;
                case DhcpConstants.V6MESSAGE_TYPE_DECLINE:
                    processor = new DhcpV6DeclineProcessor(dhcpMessage, linkAddress);
                    break;
                case DhcpConstants.V6MESSAGE_TYPE_CONFIRM:
                    processor = new DhcpV6ConfirmProcessor(dhcpMessage, linkAddress);
                    break;
                default:
                    _log.Error("Unknown message type.");
                    break;
            }
            if (processor != null)
            {
                return processor.ProcessMessage();
            }
            else
            {
                _log.Error(("No processor found for message type: " + dhcpMessage.GetMessageType()));
            }

            return null;
        }
    }
}
