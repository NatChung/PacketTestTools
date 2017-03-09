using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIXIS.DHCP.Config;
using PIXIS.DHCP.Message;
using PIXIS.DHCP.Option.V6;
using PIXIS.DHCP.DB;
using System.Net;

namespace PIXIS.DHCP.Request.Bind
{
    public interface V6TaAddrBindingManager : StaticBindingManager
    {  
        /// <summary>
       /// DHCP 派發IP前，確認IP是否已經使用
       /// </summary>
       /// <param name="ip">檢查IP</param>
       /// <param name="wait">等待時間(毫秒)</param>
       /// <returns></returns>
        Func<IPAddress, int, bool> CheckIPIsUsed { get; set; }

        void Init();

        Binding FindCurrentBinding(DhcpLink clientLink, DhcpV6ClientIdOption clientIdOption, DhcpV6IaTaOption iaTaOption, DhcpMessage requestMsg);

        Binding CreateSolicitBinding(DhcpLink clientLink, DhcpV6ClientIdOption clientIdOption, DhcpV6IaTaOption iaTaOption, DhcpMessage requestMsg, byte state, IPAddress clientV4IPAddress);

        Binding UpdateBinding(Binding binding, DhcpLink clientLink, DhcpV6ClientIdOption clientIdOption, DhcpV6IaTaOption iaTaOption, DhcpMessage requestMsg, byte state, IPAddress clientV4IPAddress);

        void ReleaseIaAddress(IdentityAssoc ia, IaAddress iaAddr);


        void DeclineIaAddress(IdentityAssoc ia, IaAddress iaAddr);

    }
}
