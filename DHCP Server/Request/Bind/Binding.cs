using PIXIS.DHCP.Config;
using PIXIS.DHCP.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Request.Bind
{
    public class Binding : IdentityAssoc
    {

        private IdentityAssoc origIa;

        private DhcpLink dhcpLink;

        public Binding(IdentityAssoc ia, DhcpLink dhcpLink)
        {
            this.origIa = ia;
            //  save a reference to the original IA
            this.SetDhcpOptions(ia.GetDhcpOptions());
            this.SetDuid(ia.GetDuid());
            this.SetIaAddresses(ia.GetIaAddresses());
            this.SetIaid(ia.GetIaid());
            this.SetIatype(ia.GetIatype());
            this.SetId(ia.GetId());
            this.SetState(ia.GetState());
            
            this.dhcpLink = dhcpLink;
        }

        public DhcpLink GetDhcpLink()
        {
            return this.dhcpLink;
        }

        public void SetDhcpLink(DhcpLink dhcpLink)
        {
            this.dhcpLink = dhcpLink;
        }

        //     @SuppressWarnings("unchecked")
        public HashSet<BindingObject> GetBindingObjects()
        {
            //         return (Collection<BindingObject>)iaAddresses;
            //  Manually convert from IaAddresses to BindingObjects
            //  which is safe because *this* Binding holds either
            //  BindingAddresses or BindingPrefixes, both of which
            //  extend from IaAddress and implement BindingObject
            if ((iaAddresses != null))
            {
                HashSet<BindingObject> bindingObjs = new HashSet<BindingObject>();
                foreach (IaAddress iaAddr in iaAddresses)
                {
                    if ((iaAddr is BindingObject))
                    {
                        bindingObjs.Add(((BindingObject)(iaAddr)));
                    }

                }

                return bindingObjs;
            }

            return null;
        }

        //     @SuppressWarnings("unchecked")
        public void SetBindingObjects(HashSet<BindingObject> bindingObjs)
        {
            //         this.setIaAddresses((Collection<? extends IaAddress>) bindingObjs);
            //  Manually convert from BindingObjects to IaAddresses
            //  which is safe because *this* Binding holds either
            //  BindingAddresses or BindingPrefixes, both of which
            //  extend from IaAddress and implement BindingObject
            if ((bindingObjs != null))
            {
                List<IaAddress> iaAddrs = new List<IaAddress>();
                foreach (BindingObject bindingObj in bindingObjs)
                {
                    if ((bindingObj is IaAddress))
                    {
                        iaAddrs.Add(((IaAddress)(bindingObj)));
                    }

                }

                this.SetIaAddresses(iaAddrs);
            }

        }

        public bool HasChanged()
        {
            return !this.Equals(this.origIa);
        }
    }
}
