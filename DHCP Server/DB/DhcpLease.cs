using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using System;
using System.Collections.Generic;
using System.Net;

namespace PIXIS.DHCP.DB
{
    public class DhcpLease
    {

        protected IPAddress ipAddress;
        protected byte[] duid;
        protected byte iatype;
        protected long iaid;        // need long to hold 32-bit unsigned integer
        protected short prefixLength;
        protected byte state;
        protected DateTime startTime;
        protected DateTime preferredEndTime;
        protected DateTime validEndTime;
        protected List<DhcpOption> iaDhcpOptions;
        protected List<DhcpOption> iaAddrDhcpOptions;

        /**
         * Gets the ip address.
         *
         * @return the ip address
         */
        public IPAddress GetIpAddress()
        {
            return ipAddress;
        }


        /**
         * Sets the ip address.
         *
         * @param ipAddress the new ip address
         */
        public void SetIpAddress(IPAddress ipAddress)
        {
            this.ipAddress = ipAddress;
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
         * Gets the prefix length.
         *
         * @return the prefix length
         */
        public short GetPrefixLength()
        {
            return prefixLength;
        }

        /**
         * Sets the prefix length.
         *
         * @param prefixLength the new prefix length
         */
        public void SetPrefixLength(short prefixLength)
        {
            this.prefixLength = prefixLength;
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
         * Gets the start time.
         *
         * @return the start time
         */
        public DateTime GetStartTime()
        {
            return startTime;
        }


        /**
         * Sets the start time.
         *
         * @param startTime the new start time
         */
        public void SetStartTime(DateTime startTime)
        {
            this.startTime = startTime;
        }


        /**
         * Gets the preferred end time.
         *
         * @return the preferred end time
         */
        public DateTime GetPreferredEndTime()
        {
            return preferredEndTime;
        }


        /**
         * Sets the preferred end time.
         *
         * @param preferredEndTime the new preferred end time
         */
        public void SetPreferredEndTime(DateTime preferredEndTime)
        {
            this.preferredEndTime = preferredEndTime;
        }


        /**
         * Gets the valid end time.
         *
         * @return the valid end time
         */
        public DateTime GetValidEndTime()
        {
            return validEndTime;
        }


        /**
         * Sets the valid end time.
         *
         * @param validEndTime the new valid end time
         */
        public void SetValidEndTime(DateTime validEndTime)
        {
            this.validEndTime = validEndTime;
        }


        /**
         * Gets the ia dhcp options.
         *
         * @return the ia dhcp options
         */
        public List<DhcpOption> GetIaDhcpOptions()
        {
            return iaDhcpOptions;
        }


        /**
         * Sets the ia dhcp options.
         *
         * @param iaDhcpOptions the new ia dhcp options
         */
        public void SetIaDhcpOptions(List<DhcpOption> iaDhcpOptions)
        {
            this.iaDhcpOptions = iaDhcpOptions;
        }

        /**
         * Adds the ia dhcp option.
         *
         * @param iaDhcpOption the ia dhcp option
         */
        public void AddIaDhcpOption(DhcpOption iaDhcpOption)
        {
            if (iaDhcpOptions == null)
            {
                //TODO: consider a Set?
                iaDhcpOptions = new List<DhcpOption>();
            }
            iaDhcpOptions.Add(iaDhcpOption);
        }


        /**
         * Gets the ia addr dhcp options.
         *
         * @return the ia addr dhcp options
         */
        public List<DhcpOption> GetIaAddrDhcpOptions()
        {
            return iaAddrDhcpOptions;
        }


        /**
         * Sets the ia addr dhcp options.
         *
         * @param iaAddrDhcpOptions the new ia addr dhcp options
         */
        public void SetIaAddrDhcpOptions(List<DhcpOption> iaAddrDhcpOptions)
        {
            this.iaAddrDhcpOptions = iaAddrDhcpOptions;
        }

        /**
         * Adds the ia addr dhcp option.
         *
         * @param iaDhcpOption the ia dhcp option
         */
        public void AddIaAddrDhcpOption(DhcpOption iaDhcpOption)
        {
            if (iaAddrDhcpOptions == null)
            {
                //TODO: consider a Set?
                iaAddrDhcpOptions = new List<DhcpOption>();
            }
            iaAddrDhcpOptions.Add(iaDhcpOption);
        }

        /* (non-Javadoc)
         * @see java.lang.Object#equals(java.lang.Object)
         */
        //public override bool Equals(Object obj)
        //{
        //    if (this == obj)
        //        return true;
        //    if (obj == null)
        //        return false;
        //    if (this.GetType() != obj.GetType())
        //        return false;
        //    DhcpLease other = (DhcpLease)obj;
        //    if (!Array.Equals(duid, other.duid))
        //        return false;
        //    if (iaAddrDhcpOptions == null)
        //    {
        //        if (other.iaAddrDhcpOptions != null)
        //            return false;
        //    }
        //    else if (!iaAddrDhcpOptions.Equals(other.iaAddrDhcpOptions))
        //        return false;
        //    if (iaDhcpOptions == null)
        //    {
        //        if (other.iaDhcpOptions != null)
        //            return false;
        //    }
        //    else if (!iaDhcpOptions.Equals(other.iaDhcpOptions))
        //        return false;
        //    if (iaid != other.iaid)
        //        return false;
        //    if (iatype != other.iatype)
        //        return false;
        //    if (ipAddress == null)
        //    {
        //        if (other.ipAddress != null)
        //            return false;
        //    }
        //    else if (!ipAddress.Equals(other.ipAddress))
        //        return false;
        //    if (preferredEndTime == null)
        //    {
        //        if (other.preferredEndTime != null)
        //            return false;
        //    }
        //    else if (!preferredEndTime.Equals(other.preferredEndTime))
        //        return false;
        //    if (startTime == null)
        //    {
        //        if (other.startTime != null)
        //            return false;
        //    }
        //    else if (!startTime.Equals(other.startTime))
        //        return false;
        //    if (state != other.state)
        //        return false;
        //    if (validEndTime == null)
        //    {
        //        if (other.validEndTime != null)
        //            return false;
        //    }
        //    else if (!validEndTime.Equals(other.validEndTime))
        //        return false;
        //    return true;
        //}

        /* (non-Javadoc)
         * @see java.lang.Object#toString()
         */
        public override string ToString()
        {
            return "DhcpLease [ipAddress=" + ipAddress +
                    ", duid=" + duid +
                    ", iatype=" + iatype + ", iaid=" + iaid + ", state=" + state +
                    ", startTime=" + startTime.ToString(Util.GMT_DATEFORMAT) +
                    ", preferredEndTime=" + preferredEndTime.ToString(Util.GMT_DATEFORMAT) +
                    ", validEndTime=" + validEndTime.ToString(Util.GMT_DATEFORMAT) +
                    ", iaDhcpOptions=" + iaDhcpOptions +
                    ", iaAddrDhcpOptions=" + iaAddrDhcpOptions + "]";
        }
    }
}