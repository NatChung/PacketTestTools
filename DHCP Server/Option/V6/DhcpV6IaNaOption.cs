using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6IaNaOption : BaseDhcpOption
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected long iaId;

        protected long t1;

        protected long t2;

        protected Dictionary<int, DhcpOption> dhcpOptions = new Dictionary<int, DhcpOption>();

        private List<DhcpV6IaAddrOption> iaAddrOptions = new List<DhcpV6IaAddrOption>();

        public DhcpV6IaNaOption() : this(null)
        {
        }

        public DhcpV6IaNaOption(v6IaNaOption iaNaOption) : base()
        {
            if ((iaNaOption != null))
            {
                this.iaId = iaNaOption.iaId;
                this.t1 = iaNaOption.t1;
                this.t2 = iaNaOption.t2;
            }

            SetCode(DhcpConstants.V6OPTION_IA_NA);
        }

        public long GetIaId()
        {
            return this.iaId;
        }

        public void SetIaId(long iaId)
        {
            this.iaId = iaId;
        }

        public long GetT1()
        {
            return this.t1;
        }

        public void SetT1(long t1)
        {
            this.t1 = t1;
        }

        public long GetT2()
        {
            return this.t2;
        }

        public void SetT2(long t2)
        {
            this.t2 = t2;
        }

        public Dictionary<int, DhcpOption> GetDhcpOptionMap()
        {
            return this.dhcpOptions;
        }

        public void SetDhcpOptionMap(Dictionary<int, DhcpOption> dhcpOptions)
        {
            this.dhcpOptions = dhcpOptions;
        }

        public void PutAllDhcpOptions(Dictionary<int, DhcpOption> dhcpOptions)
        {
            this.dhcpOptions.PutAll(dhcpOptions);
        }

        public void PutDhcpOption(DhcpOption dhcpOption)
        {
            this.dhcpOptions[dhcpOption.GetCode()] = dhcpOption;
        }

        public List<DhcpV6IaAddrOption> GetIaAddrOptions()
        {
            return this.iaAddrOptions;
        }

        public void SetIaAddrOptions(List<DhcpV6IaAddrOption> iaAddrOptions)
        {
            this.iaAddrOptions = iaAddrOptions;
        }

        public override int GetLength()
        {
            return this.GetDecodedLength();
        }

        public int GetDecodedLength()
        {
            int len = (4 + (4 + 4));
            //  iaId + t1 + t2
            if ((this.iaAddrOptions != null))
            {
                foreach (DhcpV6IaAddrOption iaAddrOption in this.iaAddrOptions)
                {
                    //  code(short) + len(short) + data_len
                    len = (len + (4 + iaAddrOption.GetDecodedLength()));
                }

            }

            if ((this.dhcpOptions != null))
            {
                foreach (DhcpOption dhcpOption in this.dhcpOptions.Values)
                {
                    //  code(short) + len(short) + data_len
                    len = (len + (4 + dhcpOption.GetLength()));
                }

            }

            return len;
        }

        protected void DecodeOptions(ByteBuffer buf, long eof)
        {
            while ((buf.position() < eof))
            {
                int code = Util.GetUnsignedShort(buf);
                log.Debug(("Option code=" + code));
                DhcpOption option = DhcpV6OptionFactory.GetDhcpOption(code);
                if ((option != null))
                {
                    option.Decode(buf);
                    if ((option is DhcpV6IaAddrOption))
                    {
                        this.iaAddrOptions.Add(((DhcpV6IaAddrOption)(option)));
                    }
                    else
                    {
                        this.PutDhcpOption(option);
                    }

                }
                else
                {
                    break;
                    //  no more options, or one is malformed, so we're done
                }

            }

        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Util.LINE_SEPARATOR);
            sb.Append(base.GetName());
            sb.Append(": iaId=");
            sb.Append(this.iaId);
            sb.Append(" t1=");
            sb.Append(this.t1);
            sb.Append(" t2=");
            sb.Append(this.t2);
            if (((this.dhcpOptions != null)
                        && this.dhcpOptions.Count > 0))
            {
                sb.Append(Util.LINE_SEPARATOR);
                sb.Append("IA_DHCPOPTIONS");
                foreach (DhcpOption dhcpOption in this.dhcpOptions.Values)
                {
                    sb.Append(dhcpOption.ToString());
                }

            }

            if (((this.iaAddrOptions != null)
                        && this.iaAddrOptions.Count > 0))
            {
                sb.Append(Util.LINE_SEPARATOR);
                sb.Append("IA_ADDRS");
                sb.Append(Util.LINE_SEPARATOR);
                foreach (DhcpV6IaAddrOption iaAddrOption in this.iaAddrOptions)
                {
                    sb.Append(iaAddrOption.ToString());
                }

            }

            return sb.ToString();
        }

        public override ByteBuffer Encode()
        {
            ByteBuffer buf = base.EncodeCodeAndLength();
            buf.putInt((int)iaId);
            buf.putInt((int)t1);
            buf.putInt((int)t2);

            if (iaAddrOptions != null)
            {
                foreach (DhcpV6IaAddrOption iaAddrOption in iaAddrOptions)
                {
                    ByteBuffer _buf = iaAddrOption.Encode();
                    if (_buf != null)
                    {
                        buf.put(_buf);
                    }
                }
            }
            if (dhcpOptions != null)
            {
                foreach (DhcpOption dhcpOption in dhcpOptions.Values)
                {
                    ByteBuffer _buf = dhcpOption.Encode();
                    if (_buf != null)
                    {
                        buf.put(_buf);
                    }
                }
            }
            return (ByteBuffer)buf.flip();
        }

        public override void Decode(ByteBuffer buf)
        {
            if ((buf != null) && buf.hasRemaining())
            {
                // already have the code, so length is next
                long len = Util.GetUnsignedShort(buf);
                if (log.IsDebugEnabled)
                    log.Debug("IA_NA option reports length=" + len +
                              ":  bytes remaining in buffer=" + buf.remaining());
                long eof = buf.position() + len;
                if (buf.position() < eof)
                {
                    iaId = Util.GetUnsignedInt(buf);
                    if (buf.position() < eof)
                    {
                        t1 = Util.GetUnsignedInt(buf);
                        if (buf.position() < eof)
                        {
                            t1 = Util.GetUnsignedInt(buf);
                            if (buf.position() < eof)
                            {
                                DecodeOptions(buf, eof);
                            }
                        }
                    }
                }
            }
        }


    }
}
