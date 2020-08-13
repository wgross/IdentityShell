using System.Management.Automation;

namespace IdentityShell.Cmdlets.WebHost
{
    [Cmdlet(VerbsLifecycle.Restart, "IdentityServer")]
    public class RestartIdentityServerCommand : IdentityCommandBase
    {
        public static IWebHostControl WebHostControl { private get; set; }

        [Parameter]
        public string ConfigurationStore { get; set; }

        [Parameter]
        public string OperationalStore { get; set; }

        [Parameter]
        public string UserStore { get; set; }

        protected override void ProcessRecord()
        {
            if (string.IsNullOrEmpty(this.ConfigurationStore))
            {
                IdentityCommandConfgurationOverride.Default.Remove("ConnectionStrings:ConfigurationStore");
            }
            else
            {
                IdentityCommandConfgurationOverride.Default["ConnectionStrings:ConfigurationStore"] = $"Data Source={this.ConfigurationStore}";
            }

            if (string.IsNullOrEmpty(this.OperationalStore))
            {
                IdentityCommandConfgurationOverride.Default.Remove("ConnectionStrings:OperationalStore");
            }
            else
            {
                IdentityCommandConfgurationOverride.Default["ConnectionStrings:OperationalStore"] = $"Data Source={this.OperationalStore}";
            }

            if (string.IsNullOrEmpty(this.UserStore))
            {
                IdentityCommandConfgurationOverride.Default.Remove("ConnectionStrings:UserStore");
            }
            else
            {
                IdentityCommandConfgurationOverride.Default["ConnectionStrings:UserStore"] = $"Data Source={this.UserStore}";
            }

            WebHostControl.Stop();
            WebHostControl.Start(new string[0]);
        }
    }
}