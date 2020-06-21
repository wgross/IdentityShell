using IdentityServer4.EntityFramework.Mappers;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Cmdlets.Configuration
{
    [Cmdlet(VerbsCommon.Get, "IdentityClient")]
    [OutputType(typeof(IdentityServer4.Models.Client))]
    public sealed class GetIdentityClientCommand : IdentityConfigurationCommandBase
    {
        protected override void ProcessRecord()
        {            
            this.QueryClients().ToList().ForEach(c => this.WriteObject(c.ToModel()));
        }
    }
}