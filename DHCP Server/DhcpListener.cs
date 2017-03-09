using PIXIS.DHCP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP
{
    public class DhcpListener
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IPEndPoint _localV4;
        private IPEndPoint _localV6;
        private Socket _localV4Socket;
        private Socket _localV6Socket;
       

        public DhcpListener()
        {
            _localV4 = new IPEndPoint(IPAddress.Any, DhcpConstants.V4_SERVER_PORT);
            _localV6 = new IPEndPoint(IPAddress.IPv6Any, DhcpConstants.V6_SERVER_PORT);
         
        }

        private Socket CreateSocket(IPEndPoint ep)
        {
            Socket socket = new Socket(ep.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            socket.EnableBroadcast = true;
            socket.SendBufferSize = 65536;
            socket.ReceiveBufferSize = 65536;
            socket.Ttl = 10;

            if (socket.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                IPv6MulticastOption multicastOption = new IPv6MulticastOption(DhcpConstants.ALL_DHCP_RELAY_AGENTS_AND_SERVERS);
                socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership, multicastOption);
            }
            if (!socket.IsBound) socket.Bind(ep);
            return socket;
        }
        public bool Open()
        {
            try
            {
                _localV4Socket = CreateSocket(_localV4);
                _localV6Socket = CreateSocket(_localV6);
            }
            catch (Exception ex)
            {

                //_log.ErrorFormat("DHCP DhcpListener can not open, V4:{0} , V6:{1}, message : {2} stack : {3}", _localV4.Address, _localV6.Address, ex.Message, ex.StackTrace);
                return false;
            }
            return true;
        }
        public void Close()
        {
            try
            {
                if (_localV4Socket != null) _localV4Socket.Close();
                if (_localV6Socket != null) _localV6Socket.Close();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("DhcpListener Close Exception. ErrorMsg: {0} , StackTrace: {1}", ex.Message, ex.StackTrace);
            }
            finally
            {
                _localV4Socket = null;
                _localV6Socket = null;
            }
        }
    }
}
