
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Request.Dns
{
    public class ForwardDdnsUpdate : DdnsUpdate
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ForwardDdnsUpdate(string fqdn, IPAddress inetAddr, byte[] duid) : base(fqdn, inetAddr, duid)
        {
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.server.request.ddns.DdnsUpdate#sendAdd()
         */
        public override bool SendAdd()
        {

            bool rc = false;

            //          Resolver res = createResolver();

            //          Name owner = new Name(fqdn);
            //          Record a_aaaa = null;
            //          if (inetAddr instanceof Inet6Address) {
            //              a_aaaa = new AAAARecord(owner, DClass.IN, ttl, inetAddr);
            //          }
            //else {
            //              a_aaaa = new ARecord(owner, DClass.IN, ttl, inetAddr);
            //          }
            //          DHCIDRecord dhcid = new DHCIDRecord(owner, DClass.IN, ttl, data);

            //          Name _zone = buildZoneName(fqdn);

            //          Update update = new Update(_zone);
            //          update.absent(owner);
            //          update.add(a_aaaa);
            //          update.add(dhcid);

            //          if (log.isDebugEnabled())
            //          {
            //              log.Debug("Sending forward DDNS update (not-exist/add) to server=" + server + ":\n" +
            //                      update.toString());
            //          }
            //          else if (log.isInfoEnabled())
            //          {
            //              log.info("Sending forward DDNS update (not-exist/add): " + a_aaaa.toString());
            //          }
            //          Message response = res.send(update);

            //          if (response.getRcode() == Rcode.NOERROR)
            //          {
            //              log.info("Forward DDNS update (not-exist/add) succeeded: " + a_aaaa.toString());
            //              rc = true;
            //          }
            //          else
            //          {
            //              if (response.getRcode() == Rcode.YXDOMAIN)
            //              {
            //                  update = new Update(_zone);
            //                  update.present(owner);
            //                  dhcid = new DHCIDRecord(owner, DClass.IN, 0, data);
            //                  update.present(dhcid);
            //                  update.add(a_aaaa);
            //                  if (log.isDebugEnabled())
            //                  {
            //                      log.Debug("Sending forward DDNS update (exist/update) to server=" + server + ":\n" +
            //                              update.toString());
            //                  }
            //                  else if (log.isInfoEnabled())
            //                  {
            //                      log.info("Sending forward DDNS update (exist/update): " + a_aaaa.toString());
            //                  }
            //                  response = res.send(update);
            //                  if (response.getRcode() == Rcode.NOERROR)
            //                  {
            //                      log.info("Forward DDNS update (exist/update) succeeded: " + a_aaaa.toString());
            //                      rc = true;
            //                  }
            //                  else
            //                  {
            //                      log.error("Forward DDNS update (exist/update) failed (rcode=" +
            //                              Rcode.string(response.getRcode()) + "): " + a_aaaa.toString());
            //                  }
            //              }
            //              else
            //              {
            //                  log.error("Forward DDNS update (not-exist/add) failed (rcode=" +
            //                          Rcode.string(response.getRcode()) + "): " + a_aaaa.toString());
            //              }
            //          }
            return rc;
        }

        /* (non-Javadoc)
         * @see com.jagornet.dhcpv6.server.request.ddns.DdnsUpdate#sendDelete()
         */
        public override bool SendDelete()
        {

            bool rc = false;

            //Resolver res = createResolver();

            //          Name owner = new Name(fqdn);
            //          Record a_aaaa = null;
            //          if (inetAddr instanceof Inet6Address) {
            //              a_aaaa = new AAAARecord(owner, DClass.IN, 0, inetAddr);
            //          }
            //else {
            //              a_aaaa = new ARecord(owner, DClass.IN, 0, inetAddr);
            //          }
            //          DHCIDRecord dhcid = new DHCIDRecord(owner, DClass.IN, 0, data);

            //          Name _zone = buildZoneName(fqdn);

            //          Update update = new Update(_zone);
            //          update.present(dhcid);
            //          update.delete(a_aaaa);

            //          if (log.isDebugEnabled())
            //          {
            //              log.Debug("Sending forward DDNS update (exist/delete) to server=" + server + ":\n" +
            //                      update.toString());
            //          }
            //          else if (log.isInfoEnabled())
            //          {
            //              log.info("Sending forward DDNS update (exist/delete): " + a_aaaa.toString());
            //          }
            //          Message response = res.send(update);

            //          if (response.getRcode() == Rcode.NOERROR)
            //          {
            //              update = new Update(_zone);
            //              update.present(dhcid);
            //              update.absent(owner, Type.A);
            //              update.absent(owner, Type.AAAA);
            //              update.delete(owner);
            //              if (log.isDebugEnabled())
            //              {
            //                  log.Debug("Sending forward DDNS update (not-exist/delete) to server=" + server + ":\n" +
            //                          update.toString());
            //              }
            //              else if (log.isInfoEnabled())
            //              {
            //                  log.info("Sending forward DDNS update (not-exist/delete): " + owner.toString());
            //              }
            //              response = res.send(update);
            //              if (response.getRcode() == Rcode.NOERROR)
            //              {
            //                  log.info("Forward DDNS update (not-exist/delete) succeeded: " + owner.toString());
            //                  rc = true;
            //              }
            //              else
            //              {
            //                  log.error("Forward DDNS update (not-exist/delete) failed (rcode=" +
            //                          Rcode.string(response.getRcode()) + "): " + owner.toString());
            //              }
            //          }
            //          else
            //          {
            //              log.error("Forward DDNS update (exist/delete) failed (rcode=" +
            //                      Rcode.string(response.getRcode()) + "): " + a_aaaa.toString());
            //          }
            return rc;
        }

        /**
         * Builds the zone name.
         * 
         * @param fqdn the fqdn
         * 
         * @return the name
         * 
         * @throws TextParseException the text parse exception
         */
        //private Name buildZoneName(String fqdn)
        //{
        //    Name _zone = null;
        //    if (zone != null)
        //    {
        //        _zone = new Name(zone);
        //    }
        //    else
        //    {
        //        int p = fqdn.indexOf('.');
        //        _zone = new Name(fqdn.substring(p + 1));
        //    }
        //    return _zone;
        //}
    }
}
