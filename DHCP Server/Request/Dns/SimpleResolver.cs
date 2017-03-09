using System;

namespace PIXIS.DHCP.Request.Dns
{
    internal class SimpleResolver : Resolver
    {
        private string server;

        public SimpleResolver(string server)
        {
            this.server = server;
        }
    }
}