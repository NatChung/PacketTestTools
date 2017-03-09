using SharpPcap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace MaxP.Arpro.Probe
{
    class NetRecver
    {
        private ICaptureDevice _ncard;
        private PhysicalAddress _probeMAC;

        public NetRecver(ICaptureDevice device, PhysicalAddress probeMAC)
        {

        }
    }
}
