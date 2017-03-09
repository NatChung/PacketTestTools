using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Request.Dns
{
    public class ReverseDdnsUpdate : DdnsUpdate
    {
        /** The rev zone bit length. */
        protected int revZoneBitLength = 64;    // default

        protected int v4RevZoneBitLength = 24;

        /**
         * Instantiates a new reverse ddns update.
         * 
         * @param fqdn the fqdn
         * @param inetAddr the inet addr
         * @param duid the duid
         */
        public ReverseDdnsUpdate(String fqdn, IPAddress inetAddr, byte[] duid) : base(fqdn, inetAddr, duid)
        {
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.server.request.ddns.DdnsUpdate#sendAdd()
         */
        public override bool SendAdd()
        {

            bool rc = false;

            Resolver res = CreateResolver();

            String revIp = BuildReverseIpString();

            //Name owner = new Name(revIp.toString());
            //PTRRecord ptr = new PTRRecord(owner, DClass.IN, ttl, new Name(fqdn));

            //Name _zone = buildZoneName(revIp);

            //Update update = new Update(_zone);
            //update.delete(owner);
            //update.add(ptr);

            //if (log.isDebugEnabled())
            //{
            //    log.Debug("Sending reverse DDNS update (replace) to server=" + server + ":\n" +
            //            update.toString());
            //}
            //else if (log.isInfoEnabled())
            //{
            //    log.info("Sending reverse DDNS update (replace): " + ptr.toString());
            //}
            //Message response = res.send(update);

            //if (response.getRcode() == Rcode.NOERROR)
            //{
            //    log.info("Reverse DDNS update (replace) succeeded: " + ptr.toString());
            //    rc = true;
            //}
            //else
            //{
            //    log.error("Reverse DDNS update (replace) failed (rcode=" +
            //            Rcode.string(response.getRcode()) + "): " + ptr.toString());
            //}
            return rc;
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.server.request.ddns.DdnsUpdate#sendDelete()
         */
        public override bool SendDelete()
        {

            bool rc = false;

            //        Resolver res = createResolver();

            //        String revIp = buildReverseIpString();

            //        Name owner = new Name(revIp);
            //        PTRRecord ptr = new PTRRecord(owner, DClass.IN, 0, new Name(fqdn));

            //        Name _zone = buildZoneName(revIp);

            //        Update update = new Update(_zone);
            //        update.delete(ptr);

            //		if (log.isDebugEnabled()) {
            //			log.Debug("Sending reverse DDNS update (delete) to server=" + server + ":\n" + 
            //					update.toString());
            //		}
            //		else if (log.isInfoEnabled()) {
            //			log.info("Sending reverse DDNS update (delete): " + ptr.toString());
            //		}
            //Message response = res.send(update);

            //		if (response.getRcode() == Rcode.NOERROR) {
            //			log.info("Reverse DDNS update (delete) succeeded: " + ptr.toString());
            //			rc = true;
            //		}
            //		else {
            //			log.error("Reverse DDNS update (delete) failed (rcode=" +
            //					Rcode.string(response.getRcode()) + "): " + ptr.toString());			
            //		}
            return rc;
        }

        /**
         * Builds the reverse ip string.
         * 
         * @return the string
         */
        protected string BuildReverseIpString()
        {
            //          if (inetAddr instanceof Inet6Address) {
            //              String[] flds = inetAddr.getHostAddress().split(":");
            //              if (flds.length != 8)
            //              {
            //                  throw new IllegalStateException("Invalid IPv6 Address: " +
            //                          inetAddr.getHostAddress());
            //              }
            //              StringBuilder expanded = new StringBuilder();
            //              for (int i = 0; i < 8; i++)
            //              {
            //                  StringBuilder sb = new StringBuilder();
            //                  if (flds[i].length() < 4)
            //                  {
            //                      for (int j = 0; j < 4 - flds[i].length(); j++)
            //                      {
            //                          sb.append('0');
            //                      }
            //                  }
            //                  sb.append(flds[i]);
            //                  expanded.append(sb);
            //              }
            //              StringBuilder reversed = expanded.reverse();
            //              StringBuilder revIp = new StringBuilder();
            //              for (int i = 0; i < reversed.length(); i++)
            //              {
            //                  revIp.append(reversed.substring(i, i + 1));
            //                  revIp.append('.');
            //              }
            //              revIp.append("ip6.arpa.");
            //              return revIp.toString();
            //          }
            //else {
            //              String[] flds = inetAddr.getHostAddress().split("\\.");
            //              if (flds.length != 4)
            //              {
            //                  throw new IllegalStateException("Invalid IPv4 Address: " +
            //                          inetAddr.getHostAddress());
            //              }
            //              StringBuilder revIp = new StringBuilder();
            //              for (int i = 3; i >= 0; i--)
            //              {
            //                  revIp.append(flds[i]);
            //                  revIp.append('.');
            //              }
            //              revIp.append("in-addr.arpa.");
            //              return revIp.ToString();
            //          }
            return "";
        }

        /**
         * Builds the zone name.
         * 
         * @param revIp the rev ip
         * 
         * @return the name
         * 
         * @throws TextParseException the text parse exception
         */
        private Name BuildZoneName(string revIp)
        {
            Name _zone = null;
            //if (zone != null) {
            //              _zone = new Name(zone);
            //          }
            //else {
            //              if (inetAddr instanceof Inet6Address) {
            //                  int p = 64 - (revZoneBitLength / 2);
            //                  if (p < 0)
            //                      p = 64;
            //                  _zone = new Name(revIp.substring(p));
            //              }
            //	else {
            //                  _zone = new Name(revIp);
            //                  int c = 0;
            //                  if (v4RevZoneBitLength <= 8)
            //                      c = 3;
            //                  else if (v4RevZoneBitLength <= 16)
            //                      c = 2;
            //                  else
            //                      c = 1;
            //                  for (int i = 0; i < c; i++)
            //                  {
            //                      int p = _zone.toString().indexOf('.');
            //                      _zone = new Name(_zone.toString().substring(p + 1));
            //                  }
            //              }
            //          }
            return _zone;
        }

        /**
         * Gets the rev zone bit length.
         * 
         * @return the rev zone bit length
         */
        public int GetRevZoneBitLength()
        {
            return revZoneBitLength;
        }

        /**
         * Sets the rev zone bit length.
         * 
         * @param revZoneBitLength the new rev zone bit length
         */
        public void SetRevZoneBitLength(int revZoneBitLength)
        {
            this.revZoneBitLength = revZoneBitLength;
        }

        public int GetV4RevZoneBitLength()
        {
            return v4RevZoneBitLength;
        }

        public void SetV4RevZoneBitLength(int v4RevZoneBitLength)
        {
            this.v4RevZoneBitLength = v4RevZoneBitLength;
        }
    }
}
