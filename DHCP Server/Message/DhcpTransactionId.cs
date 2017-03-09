using PIXIS.DHCP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Message
{
    public class DhcpTransactionId
    {

        public static ByteBuffer Encode(int xId)
        {
            ByteBuffer buf = ByteBuffer.allocate(3);
            Util.PutMediumInt(buf, xId);
            return ((ByteBuffer)(buf.flip()));
        }

        public static int Decode(ByteBuffer buf)
        {
            return Util.GetUnsignedMediumInt(buf);
        }
    }
}
