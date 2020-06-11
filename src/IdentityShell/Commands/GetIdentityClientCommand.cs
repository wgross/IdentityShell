using System.Management.Automation;

namespace IdentityShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "IdentityClient")]
    [OutputType(typeof(IdentityServer4.Models.Client))]
    public class GetIdentityClientCommand : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            IdentityShell.Config.Clients.ForEach(client => this.WriteObject(client));
        }
    }
}