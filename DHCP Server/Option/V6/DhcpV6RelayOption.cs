using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIXIS.DHCP.Message;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using System.Net;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6RelayOption : BaseDhcpOption
    {
        /** The NAME. */
        protected static string NAME = "Relay Message";

        // the DhcpRelayMessage which contains
        // this DhcpRelayOption
        /** The relay message. */
        protected DhcpV6RelayMessage relayMessage;

        // the DhcpMessage contained in this relay option, 
        // which may itself be a DhcpRelayMessage, 
        // thus containing yet another DhcpRelayOption
        /** The dhcp message being relayed. */
        protected DhcpV6Message dhcpMessage;

        /**
         * Instantiates a new dhcp relay option.
         */
        public DhcpV6RelayOption()
        {
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.DhcpOption#getLength()
         */
        public override int GetLength()
        {
            // the length of this option is the length of the contained message
            return dhcpMessage.GetLength();
        }


        /**
         * Gets the relay message.
         * 
         * @return the relay message
         */
        public DhcpV6RelayMessage GetRelayMessage()
        {
            return relayMessage;
        }

        /**
         * Sets the relay message.
         * 
         * @param relayMessage the new relay message
         */
        public void SetRelayMessage(DhcpV6RelayMessage relayMessage)
        {
            this.relayMessage = relayMessage;
        }

        /**
         * Gets the dhcp message.
         * 
         * @return the dhcp message
         */
        public DhcpV6Message GetDhcpMessage()
        {
            return dhcpMessage;
        }

        /**
         * Sets the dhcp message.
         * 
         * @param relayMessage the dhcp message
         */
        public void SetDhcpMessage(DhcpV6Message dhcpMessage)
        {
            this.dhcpMessage = dhcpMessage;
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.DhcpOption#getCode()
         */
        public override int GetCode()
        {
            return DhcpConstants.V6OPTION_RELAY_MSG;
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.option.BaseDhcpOption#getName()
         */
        public override string GetName()
        {
            return NAME;
        }

        /* (non-Javadoc)
         * @see java.lang.Object#toString()
         */
        public override string ToString()
        {
            if (dhcpMessage == null)
                return null;

            StringBuilder sb = new StringBuilder(NAME);
            sb.Append(": ");
            sb.Append(dhcpMessage.ToString());
            return sb.ToString();
        }

        public override void Decode(ByteBuffer buf)
        {
            int len = base.DecodeLength(buf);
            // since the linkAddr of the relay message is the interface
            // on which the relay itself received the message to be forwarded
            // we can assume that address is logically a server port
            IPEndPoint relayMsgLocalAddr =
                new IPEndPoint(relayMessage.GetLinkAddress(),
                        DhcpConstants.V6_SERVER_PORT);
            // the peerAddr of the relay message is the source address of
            // the message which it received and is forwarding, thus the
            // peerAddress is either another relay or the client itself,
            // so we don't really know what port to logically set for the
            // inner message's remote port
            IPEndPoint relayMsgRemoteAddr =
                new IPEndPoint(relayMessage.GetPeerAddress(), 0);

            // create a new buffer that will hold just this RelayOption
            byte[] b = buf.getBytes(len);

            ByteBuffer _buf = ByteBuffer.allocate(len);
            _buf.put(b);
            // use the wrapped buffer which represents the contents of the message
            // contained within this relay option, but not any more, i.e. not beyond
            // what _this_ relay option reports its length to be
            dhcpMessage = DhcpV6Message.Decode(_buf, relayMsgLocalAddr.Address, relayMsgRemoteAddr);
        }

        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            buf.put(dhcpMessage.Encode());
            return (ByteBuffer)buf.flip();
        }
    }
}
