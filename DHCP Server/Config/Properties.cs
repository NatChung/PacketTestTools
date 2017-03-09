using System;
using System.Reflection;

namespace PIXIS.DHCP.Config
{
    public class Properties
    {
        private DhcpServerPolicies.Property[] dEFAULT_PROPERTIES;

        public Properties(DhcpServerPolicies.Property[] dEFAULT_PROPERTIES)
        {
            this.dEFAULT_PROPERTIES = dEFAULT_PROPERTIES;
        }

        public string GetProperty(string key)
        {
            DhcpServerPolicies polices = new DhcpServerPolicies();
            string value = "";
            PropertyInfo propertyInfo = polices.GetType().GetProperty(key.ToUpper());
            propertyInfo.SetValue(polices, Convert.ChangeType(value, propertyInfo.PropertyType), null);
            return value;
        }
    }
}