using PIXIS.DHCP.Option.V4;
using PIXIS.DHCP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option.Base
{
    public abstract class BaseDhcpOption : DhcpOption
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected string name;

        protected int code;

        protected bool v4;

        public abstract void Decode(ByteBuffer buf);
        public abstract ByteBuffer Encode();
        public abstract int GetLength();

        //  true only if DHCPv4 option
        protected ByteBuffer EncodeCodeAndLength()
        {
            ByteBuffer buf = null;
            if (!this.v4)
            {
                buf = ByteBuffer.allocate((2 + (2 + GetLength())));
                buf.putShort(((short)(this.GetCode())));
                buf.putShort(((short)(GetLength())));
            }
            else
            {
                buf = ByteBuffer.allocate((1 + (1 + GetLength())));
                buf.put(((byte)(this.GetCode())));
                buf.put(((byte)(GetLength())));
            }

            return buf;
        }

        protected int DecodeLength(ByteBuffer buf)
        {
            if (((buf != null)
                        && buf.hasRemaining()))
            {
                //  already have the code, so length is next
                int len = 0;
                if (!this.v4)
                {
                    len = Util.GetUnsignedShort(buf);
                }
                else
                {
                    len = Util.GetUnsignedByte(buf);
                }

                if (log.IsDebugEnabled)
                {
                    log.Debug((GetName() + (" reports length="
                                    + (len + (":  bytes remaining in buffer=" + buf.remaining())))));
                }

                return len;
            }

            return 0;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public virtual string GetName()
        {
            if ((this.name != null))
            {
                return this.name;
            }

            string className = this.GetType().Name;
            if (className.StartsWith("Dhcp"))
            {
                return className;
            }

            return ("Option-" + this.GetCode());
        }

        public void SetCode(int code)
        {
            this.code = code;
        }

        public virtual int GetCode()
        {
            return this.code;
        }

        public bool IsV4()
        {
            return this.v4;
        }

        public void SetV4(bool v4)
        {
            this.v4 = v4;
        }


    }
}
