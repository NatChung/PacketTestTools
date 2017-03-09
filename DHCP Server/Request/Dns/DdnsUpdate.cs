
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Request.Dns
{
    public abstract class DdnsUpdate
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /** The sha256 msg digest. */
        protected static MessageDigest sha256MsgDigest;

        /** The fqdn. */
        protected string fqdn;

        /** The inet addr. */
        protected IPAddress inetAddr;

        /** The data. */
        protected byte[] data;

        /** The server. */
        protected string server;

        /** The ttl. */
        protected long ttl;

        /** The zone. */
        protected string zone;

        /** The tsig key name. */
        protected string tsigKeyName;

        /** The tsig algorithm. */
        protected string tsigAlgorithm;

        /** The tsig key data. */
        protected string tsigKeyData;

        /**
         * Instantiates a new ddns update.
         * 
         * @param fqdn the fqdn
         * @param inetAddr the inet addr
         * @param duid the duid
         */
        public DdnsUpdate(string fqdn, IPAddress inetAddr, byte[] duid)
        {
            this.fqdn = fqdn;
            this.inetAddr = inetAddr;

            //byte[] buf = new byte[duid.Length + fqdn.getBytes().length];
            //System.arraycopy(duid, 0, buf, 0, duid.length);
            //System.arraycopy(fqdn.getBytes(), 0, buf, duid.length, fqdn.getBytes().length);

            //MessageDigest md = getSha256MsgDigest();
            //if (md != null)
            //{
            //    byte[] hash = md.digest(buf);

            //    this.data = new byte[3 + hash.length];
            //    data[0] = (byte)0x00;
            //    data[1] = (byte)0x02;
            //    data[2] = (byte)0x01;
            //    System.arraycopy(hash, 0, data, 3, hash.length);
            //}
        }

        /**
         * Creates the resolver.
         * 
         * @return the resolver
         * 
         * @throws UnknownHostException the unknown host exception
         * @throws TextParseException the text parse exception
         */
        protected Resolver CreateResolver()
        {

            Resolver res = new SimpleResolver(server);
            //if ((tsigKeyName != null) && (tsigKeyName.Length > 0))
            //{
            //    TSIG tsig = null;
            //    if (tsigAlgorithm != null)
            //    {
            //        tsig = new TSIG(new Name(tsigAlgorithm), tsigKeyName, tsigKeyData);
            //    }
            //    else
            //    {
            //        tsig = new TSIG(tsigKeyName, tsigKeyData);
            //    }
            //    res.setTSIGKey(tsig);
            //}
            return res;
        }

        /**
         * Send add.
         * 
         * @throws TextParseException the text parse exception
         * @throws IOException Signals that an I/O exception has occurred.
         */
        public abstract bool SendAdd();

        /**
         * Send delete.
         * 
         * @throws TextParseException the text parse exception
         * @throws IOException Signals that an I/O exception has occurred.
         */
        public abstract bool SendDelete();

        /**
         * Gets the server.
         * 
         * @return the server
         */
        public string GetServer()
        {
            return server;
        }

        /**
         * Sets the server.
         * 
         * @param server the new server
         */
        public void SetServer(string server)
        {
            this.server = server;
        }

        /**
         * Gets the ttl.
         * 
         * @return the ttl
         */
        public long GetTtl()
        {
            return ttl;
        }

        /**
         * Sets the ttl.
         * 
         * @param ttl the new ttl
         */
        public void SetTtl(long ttl)
        {
            this.ttl = ttl;
        }

        /**
         * Gets the zone.
         * 
         * @return the zone
         */
        public string GetZone()
        {
            return zone;
        }

        /**
         * Sets the zone.
         * 
         * @param zone the new zone
         */
        public void SetZone(string zone)
        {
            this.zone = zone;
        }

        /**
         * Gets the tsig key name.
         * 
         * @return the tsig key name
         */
        public string GetTsigKeyName()
        {
            return tsigKeyName;
        }

        /**
         * Sets the tsig key name.
         * 
         * @param tsigKeyName the new tsig key name
         */
        public void SetTsigKeyName(string tsigKeyName)
        {
            this.tsigKeyName = tsigKeyName;
        }

        /**
         * Gets the tsig algorithm.
         * 
         * @return the tsig algorithm
         */
        public string GetTsigAlgorithm()
        {
            return tsigAlgorithm;
        }

        /**
         * Sets the tsig algorithm.
         * 
         * @param tsigAlgorithm the new tsig algorithm
         */
        public void SetTsigAlgorithm(string tsigAlgorithm)
        {
            this.tsigAlgorithm = tsigAlgorithm;
        }

        /**
         * Gets the tsig key data.
         * 
         * @return the tsig key data
         */
        public string GetTsigKeyData()
        {
            return tsigKeyData;
        }

        /**
         * Sets the tsig key data.
         * 
         * @param tsigKeyData the new tsig key data
         */
        public void SetTsigKeyData(string tsigKeyData)
        {
            this.tsigKeyData = tsigKeyData;
        }
    }
}
