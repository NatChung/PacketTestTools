using PIXIS.DHCP.Config;
using PIXIS.DHCP.DB;
using PIXIS.DHCP.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace PIXIS.DHCP.Request.Bind
{
    public interface V4AddrBindingManager : StaticBindingManager
    {
        Func<IPAddress, int, bool> CheckIPIsUsed { get; set; }

        //Initialize the manager.
        //@throws DhcpServerConfigException
        void Init();
        void UpdatePool();
        //Find current binding. 
        //@param clientLink the client link 
        //@param clientIdOption the client id option 
        //@param requestMsg the request msg 
        //@return the binding 
        Binding FindCurrentBinding(DhcpLink clientLink, byte[] macAddr, DhcpMessage requestMsg);

        //Creates the solicit binding. 
        //@param clientLink the client link 
        //@param clientIdOption the client id option 
        //@param requestMsg the request msg 
        //@param rapidCommit the rapid commit 
        //@return the binding 
        Binding CreateDiscoverBinding(DhcpLink clientLink, byte[] macAddr, DhcpMessage requestMsg, byte state);

        //Update binding. 
        //@param binding the binding 
        //@param clientLink the client link 
        //@param clientIdOption the client id option 
        //@param requestMsg the request msg 
        //@param state the state 
        //@return the binding 
        Binding UpdateBinding(Binding binding, DhcpLink clientLink, byte[] macAddr, DhcpMessage requestMsg, byte state);

        //Release ia address. 
        //@param iaAddr the ia addr 
        void ReleaseIaAddress(IdentityAssoc ia, IaAddress iaAddr);

        //Decline ia address. 
        //@param iaAddr the ia addr 
        void DeclineIaAddress(IdentityAssoc ia, IaAddress iaAddr);
    }
}
