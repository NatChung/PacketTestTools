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
    public interface V6PrefixBindingManager : StaticBindingManager
    {
        /// <summary>
        /// DHCP 派發IP前，確認IP是否已經使用
        /// </summary>
        /// <param name="ip">檢查IP</param>
        /// <param name="wait">等待時間(毫秒)</param>
        /// <returns></returns>
        Func<IPAddress, int, bool> CheckIPIsUsed { get; set; }

        /**
* Initialize the manager.
* 
* @throws DhcpServerConfigException
*/
        void Init();

        /**
         * Find current binding.
         * 
         * @param clientLink the client link
         * @param clientIdOption the client id option
         * @param iaPdOption the ia pd option
         * @param requestMsg the request msg
         * 
         * @return the binding
         */
        Binding FindCurrentBinding(DhcpLink clientLink, DhcpV6ClientIdOption clientIdOption,
               DhcpV6IaPdOption iaPdOption, DhcpMessage requestMsg);

        /**
         * Creates the solicit binding.
         * 
         * @param clientLink the client link
         * @param clientIdOption the client id option
         * @param iaPdOption the ia pd option
         * @param requestMsg the request msg
         * @param rapidCommit the rapid commit
         * 
         * @return the binding
         */
        Binding CreateSolicitBinding(DhcpLink clientLink, DhcpV6ClientIdOption clientIdOption,
               DhcpV6IaPdOption iaPdOption, DhcpMessage requestMsg, byte state, IPAddress clientV4IPAddress);

        /**
         * Update binding.
         * 
         * @param binding the binding
         * @param clientLink the client link
         * @param clientIdOption the client id option
         * @param iaPdOption the ia pd option
         * @param requestMsg the request msg
         * @param state the state
         * 
         * @return the binding
         */
        Binding UpdateBinding(Binding binding, DhcpLink clientLink,
                DhcpV6ClientIdOption clientIdOption, DhcpV6IaPdOption iaPdOption,
                DhcpMessage requestMsg, byte state, IPAddress clientV4IPAddress);

        /**
         * Release ia prefix.
         * 
         * @param iaPrefix the ia prefix
         */
        void ReleaseIaPrefix(IaPrefix iaPrefix);

        /**
         * Decline ia prefix.
         * 
         * @param iaPrefix the ia prefix
         */
        void DeclineIaPrefix(IaPrefix iaPrefix);
    }
}
