using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.V4Process
{
    public class DhcpV4InformProcessor : BaseDhcpV4Processor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Construct an DhcpV4InformRequest processor.

        //@param requestMsg the Inform message
        //@param clientLinkAddress the client link address
        public DhcpV4InformProcessor(DhcpV4Message requestMsg, IPAddress clientLinkAddress) : base(requestMsg, clientLinkAddress)
        {
        }

        //@see com.jagornet.dhcpv6.server.request.BaseDhcpProcessor#preProcess() 
        public override bool PreProcess()
        {
            if (!base.PreProcess())
            {
                return false;
            }
            if (_requestMsg.GetCiAddr() == null)
            {
                return false;
            }
            if (_requestMsg.GetCiAddr().Equals(DhcpConstants.ZEROADDR_V4))
            {
                log.Warn("Ignoring Inform message: " +
                        "ciAddr is zero");
            }
            return true;
        }
        //Process the client request.  Find appropriate configuration based on any 
        //criteria in the request message that can be matched against the server's 
        //configuration, then formulate a response message containing the options 
        //to be sent to the client. 

        //@return true if a reply should be sent, false otherwise 
        public override bool Process()
        {
            //        When the server receives an Information-request message, the client 
            //        is requesting configuration information that does not include the 
            //        assignment of any addresses.  The server determines all configuration 
            //        parameters appropriate to the client, based on the server 
            //        configuration policies known to the server. 


            _replyMsg.SetCiAddr(_requestMsg.GetCiAddr());  // copy the ciAddr from client 
            _replyMsg.SetMessageType((short)DhcpConstants.V4MESSAGE_TYPE_ACK);
            PopulateV4Reply(_clientLink, null);

            return true;
        }
    }
}
