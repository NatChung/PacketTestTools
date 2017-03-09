using PIXIS.DHCP.Option;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.DB
{
    public class IdentityAssoc
    {
        // iatypes
        public const byte V4_TYPE = 0;
        public const byte NA_TYPE = 1;
        public const byte TA_TYPE = 2;
        public const byte PD_TYPE = 3;

        // states
        public static readonly byte ADVERTISED = IaAddress.ADVERTISED;
        public static readonly byte COMMITTED = IaAddress.COMMITTED;
        public static readonly byte EXPIRED = IaAddress.EXPIRED;

        protected long id;  // the database-generated object ID
        protected byte[] duid;
        protected byte iatype;
        protected long iaid;        // need long to hold 32-bit unsigned integer
        protected byte state;
        protected List<IaAddress> iaAddresses;
        protected List<DhcpOption> dhcpOptions;

        /**
         * Gets the id.
         * 
         * @return the id
         */
        public long GetId()
        {
            return id;
        }

        /**
         * Sets the id.
         * 
         * @param id the new id
         */
        public void SetId(long id)
        {
            this.id = id;
        }

        /**
         * Gets the duid.
         * 
         * @return the duid
         */
        public byte[] GetDuid()
        {
            return duid;
        }

        /**
         * Sets the duid.
         * 
         * @param duid the new duid
         */
        public void SetDuid(byte[] duid)
        {
            this.duid = duid;
        }

        /**
         * Gets the iatype.
         * 
         * @return the iatype
         */
        public byte GetIatype()
        {
            return iatype;
        }

        /**
         * Sets the iatype.
         * 
         * @param iatype the new iatype
         */
        public void SetIatype(byte iatype)
        {
            this.iatype = iatype;
        }

        /**
         * Gets the iaid.
         * 
         * @return the iaid
         */
        public long GetIaid()
        {
            return iaid;
        }

        /**
         * Sets the iaid.
         * 
         * @param iaid the new iaid
         */
        public void SetIaid(long iaid)
        {
            this.iaid = iaid;
        }

        /**
         * Gets the state.
         * 
         * @return the state
         */
        public byte GetState()
        {
            return state;
        }

        /**
         * Sets the state.
         * 
         * @param state the new state
         */
        public void SetState(byte state)
        {
            this.state = state;
        }

        /**
         * Gets the ia addresses.
         * 
         * @return the ia addresses
         */
        public List<IaAddress> GetIaAddresses()
        {
            return iaAddresses;
        }

        /**
         * Sets the ia addresses.
         * 
         * @param iaAddresses the new ia addresses
         */
        public void SetIaAddresses(List<IaAddress> iaAddresses)
        {
            this.iaAddresses = iaAddresses;
        }

        /**
         * Get a specific DHCP option.
         * 
         * @param code
         * @return
         */
        public BaseIpAddressOption GetDhcpOption(int code)
        {
            if (dhcpOptions != null)
            {
                foreach (BaseIpAddressOption dhcpOption in dhcpOptions)
                {
                    if (dhcpOption.GetCode() == code)
                        return dhcpOption;
                }
            }
            return null;
        }

        /**
         * Gets the dhcp options.
         * 
         * @return the dhcp options
         */
        public List<DhcpOption> GetDhcpOptions()
        {
            return dhcpOptions;
        }

        /**
         * Sets the dhcp options.
         * 
         * @param dhcpOptions the new dhcp options
         */
        public void SetDhcpOptions(List<DhcpOption> dhcpOptions)
        {
            this.dhcpOptions = dhcpOptions;
        }

        /* (non-Javadoc)
         * @see java.lang.Object#hashCode()
         */
        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
            result = prime * result + duid.GetHashCode();
            result = prime * result + (int)(iaid ^ (Helper.MoveByte(iaid, 32)));
            result = prime * result + iatype;
            result = prime * result + ((id == 0) ? 0 : id.GetHashCode());
            result = prime * result + state;
            return result;
        }

        /* (non-Javadoc)
         * @see java.lang.Object#equals(java.lang.Object)
         */
        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if ((this.GetType() != obj.GetType()))
                return false;
            if (!(obj is IdentityAssoc))
                return false;

            IdentityAssoc other = (IdentityAssoc)obj;
            if (!Array.Equals(duid, other.duid))
                return false;
            if (iaid != other.iaid)
                return false;
            if (iatype != other.iatype)
                return false;
            if (id == 0)
            {
                if (other.id != 0)
                    return false;
            }
            else if (!id.Equals(other.id))
                return false;
            if (state != other.state)
                return false;
            return true;
        }


        /* (non-Javadoc)
         * @see java.lang.Object#toString()
         */
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Util.LINE_SEPARATOR);
            sb.Append(KeyToString(this.GetDuid(), this.GetIatype(), this.GetIaid()));
            sb.Append(" state=");
            sb.Append(this.GetState());
            sb.Append('(');
            sb.Append(IaAddress.StateToString(this.GetState()));
            sb.Append(')');
            List<IaAddress> iaAddrs = this.GetIaAddresses();
            if (iaAddrs != null)
            {
                foreach (IaAddress iaAddr in iaAddrs)
                {
                    sb.Append(Util.LINE_SEPARATOR);
                    sb.Append('\t');
                    sb.Append(iaAddr.ToString());
                }
            }
            List<DhcpOption> opts = this.GetDhcpOptions();
            if (opts != null)
            {
                foreach (BaseIpAddressOption dhcpOption in opts)
                {
                    sb.Append(Util.LINE_SEPARATOR);
                    sb.Append("\tIA Option: ");
                    sb.Append(dhcpOption.ToString());
                }
            }
            return sb.ToString();
        }

        public static String KeyToString(byte[] _duid, byte _iatype, long _iaid)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("IA: duid=");
            sb.Append(Util.ToHexString(_duid));
            sb.Append(" iatype=");
            sb.Append(_iatype);
            sb.Append('(');
            sb.Append(IaTypeToString(_iatype));
            sb.Append(')');
            sb.Append(" iaid=");
            sb.Append(_iaid);
            return sb.ToString();
        }

        public static string IaTypeToString(byte _iatype)
        {
            string s = null;
            switch (_iatype)
            {
                case V4_TYPE:
                    s = "V4";
                    break;
                case NA_TYPE:
                    s = "NA";
                    break;
                case TA_TYPE:
                    s = "TA";
                    break;
                case PD_TYPE:
                    s = "PD";
                    break;
                default:
                    s = "??";
                    break;
            }
            return s;
        }
    }
}
