using System.Collections.Generic;

namespace IdentityShell.Cmdlets
{
    public class IdentityCommandConfgurationOverride
    {
        /*
           "ConnectionStrings": {
           "OperationalStore": "Data Source=IdentityShell.OperationalStore.db",
           "ConfigurationStore": "Data Source=IdentityShell.ConfigurationStore.db",
            "UserStore": "Data Source=IdentityShell.UserStore.db"
        */

        public static Dictionary<string, string> Default { get; private set; } = new Dictionary<string, string>();
        //{
        //    { "ConnectionStrings:ConfigurationStore","Data Source=IdentityShell.ConfigurationStore-2.db"}
        //};
    }
}