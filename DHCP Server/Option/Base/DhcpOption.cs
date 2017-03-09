using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option.Base
{
    public interface DhcpOption //: Encodable, Decodable
    {

        int GetCode();
        string GetName();
        int GetLength();
        string ToString();
        bool IsV4();
        void Decode(ByteBuffer buf);
        ByteBuffer Encode();
    }
}
