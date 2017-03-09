using PIXIS.DHCP.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Request.Bind
{
    public interface BindingObject
    {
        void SetState(byte state);
        DhcpConfigObject GetConfigObj();
        void SetStartTime(DateTime startDate);
        void SetPreferredEndTime(DateTime preferredDate);
        void SetValidEndTime(DateTime validDate);
        IPAddress GetIpAddress();
    }
}
