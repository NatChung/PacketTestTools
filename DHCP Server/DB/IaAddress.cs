using PIXIS.DHCP.Option;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Option.V4;
using PIXIS.DHCP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.DB
{
    public class IaAddress
    {

        //  states
        public const byte ADVERTISED = 1;

        public const byte COMMITTED = 2;

        public const byte EXPIRED = 3;

        public const byte RELEASED = 4;

        public const byte DECLINED = 5;

        public const byte STATIC = 6;

        protected long id;

        //  the database-generated object ID
        protected IPAddress ipAddress;

        protected DateTime startTime;

        protected DateTime preferredEndTime;

        protected DateTime validEndTime;

        protected byte state;

        protected long identityAssocId;

        protected List<DhcpOption> dhcpOptions;

        public long GetId()
        {
            return this.id;
        }

        public void SetId(long id)
        {
            this.id = id;
        }

        public IPAddress GetIpAddress()
        {
            return this.ipAddress;
        }

        public void SetIpAddress(IPAddress ipAddress)
        {
            this.ipAddress = ipAddress;
        }

        public DateTime GetStartTime()
        {
            return this.startTime;
        }

        public void SetStartTime(DateTime startTime)
        {
            this.startTime = startTime;
        }

        public DateTime GetPreferredEndTime()
        {
            return this.preferredEndTime;
        }

        public void SetPreferredEndTime(DateTime preferredEndTime)
        {
            this.preferredEndTime = preferredEndTime;
        }

        public DateTime GetValidEndTime()
        {
            return this.validEndTime;
        }

        public void SetValidEndTime(DateTime validEndTime)
        {
            this.validEndTime = validEndTime;
        }

        public byte GetState()
        {
            return this.state;
        }

        public void SetState(byte state)
        {
            this.state = state;
        }

        public long GetIdentityAssocId()
        {
            return this.identityAssocId;
        }

        public void SetIdentityAssocId(long identityAssocId)
        {
            this.identityAssocId = identityAssocId;
        }

        public DhcpOption GetDhcpOption(int code)
        {
            if ((this.dhcpOptions != null))
            {
                foreach (DhcpOption dhcpOption in this.dhcpOptions)
                {
                    if ((dhcpOption.GetCode() == code))
                    {
                        return dhcpOption;
                    }

                }

            }

            return null;
        }

        public void SetDhcpOption(DhcpOption newOption)
        {
            if ((this.dhcpOptions == null))
            {
                this.dhcpOptions = new List<DhcpOption>();
            }

            //  first remove the option, if it exists
            foreach (DhcpOption dhcpOption in this.dhcpOptions)
            {
                if ((dhcpOption.GetCode() == newOption.GetCode()))
                {
                    this.dhcpOptions.Remove(dhcpOption);
                    break;
                }

            }

            this.dhcpOptions.Add(newOption);
        }

        public List<DhcpOption> GetDhcpOptions()
        {
            return this.dhcpOptions;
        }

        public void SetDhcpOptions(List<DhcpOption> dhcpOptions)
        {
            this.dhcpOptions = dhcpOptions;
        }

        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
            result = prime * result + id.GetHashCode();
            result = prime * result
                    + identityAssocId.GetHashCode();
            result = prime * result
                    + ((ipAddress == null) ? 0 : ipAddress.GetHashCode());
            result = prime
                    * result
                    + ((preferredEndTime == null) ? 0 : preferredEndTime.GetHashCode());
            result = prime * result
                    + ((startTime == null) ? 0 : startTime.GetHashCode());
            result = prime * result + state;
            result = prime * result
                    + ((validEndTime == null) ? 0 : validEndTime.GetHashCode());
            return result;
        }

        public override bool Equals(Object obj)
        {
            if ((this == obj))
            {
                return true;
            }

            if ((obj == null))
            {
                return false;
            }

            if ((this.GetType() != obj.GetType()))
            {
                return false;
            }

            if (!(obj is IaAddress))
            {
                return false;
            }

            IaAddress other = ((IaAddress)(obj));
            if ((this.id == 0))
            {
                if ((other.id != 0))
                {
                    return false;
                }

            }
            else if (!this.id.Equals(other.id))
            {
                return false;
            }

            if ((this.identityAssocId == 0))
            {
                if ((other.identityAssocId != 0))
                {
                    return false;
                }

            }
            else if (!this.identityAssocId.Equals(other.identityAssocId))
            {
                return false;
            }

            if ((this.ipAddress == null))
            {
                if ((other.ipAddress != null))
                {
                    return false;
                }

            }
            else if (!this.ipAddress.Equals(other.ipAddress))
            {
                return false;
            }

            if ((this.preferredEndTime == null))
            {
                if ((other.preferredEndTime != null))
                {
                    return false;
                }

            }
            else if (!this.preferredEndTime.Equals(other.preferredEndTime))
            {
                return false;
            }

            if ((this.startTime == null))
            {
                if ((other.startTime != null))
                {
                    return false;
                }

            }
            else if (!this.startTime.Equals(other.startTime))
            {
                return false;
            }

            if ((this.state != other.state))
            {
                return false;
            }

            if ((this.validEndTime == null))
            {
                if ((other.validEndTime != null))
                {
                    return false;
                }

            }
            else if (!this.validEndTime.Equals(other.validEndTime))
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("IA_ADDR: ");
            sb.Append(" ip=");
            sb.Append(this.GetIpAddress().ToString());
            sb.Append(" state=");
            sb.Append((this.GetState() + ("("
                            + (IaAddress.StateToString(this.GetState()) + ")"))));
            sb.Append(" startTime=");
            if ((this.GetStartTime() != null))
            {
                sb.Append(this.GetStartTime().ToString(Util.GMT_DATEFORMAT));
            }

            sb.Append(" preferredEndTime=");
            if ((this.GetPreferredEndTime() != null))
            {
                if ((this.GetPreferredEndTime().Millisecond < 0))
                {
                    sb.Append("infinite");
                }
                else
                {
                    sb.Append(this.GetPreferredEndTime().ToString(Util.GMT_DATEFORMAT));
                }

            }

            sb.Append(" validEndTime=");
            if ((this.GetValidEndTime() != null))
            {
                if ((this.GetValidEndTime().Millisecond < 0))
                {
                    sb.Append("infinite");
                }
                else
                {
                    sb.Append(this.GetValidEndTime().ToString(Util.GMT_DATEFORMAT));
                }

            }

            List<DhcpOption> opts = this.GetDhcpOptions();
            if ((opts != null))
            {
                foreach (BaseIpAddressOption dhcpOption in opts)
                {
                    sb.Append(Util.LINE_SEPARATOR);
                    sb.Append("\t\tIA_ADDR Option: ");
                    sb.Append(dhcpOption.ToString());
                }

            }

            return sb.ToString();
        }

        public static string StateToString(byte state)
        {
            string s = null;
            switch (state)
            {
                case ADVERTISED:
                    s = "Advertised";
                    break;
                case COMMITTED:
                    s = "Committed";
                    break;
                case EXPIRED:
                    s = "Expired";
                    break;
                case RELEASED:
                    s = "Released";
                    break;
                case DECLINED:
                    s = "Declined";
                    break;
                case STATIC:
                    s = "Static";
                    break;
                default:
                    s = "Unknown";
                    break;
            }
            return s;
        }
    }
}
