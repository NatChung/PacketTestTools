using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Request.Dns
{
    public interface DdnsCallback
    {
        void FwdAddComplete(bool success);
        void FwdDeleteComplete(bool success);
        void FevAddComplete(bool success);
        void FevDeleteComplete(bool success);
        void RevAddComplete(object p);
        void RevDeleteComplete(object p);
    }
}
