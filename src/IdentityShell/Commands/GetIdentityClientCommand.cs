using IdentityServer4.EntityFramework.Mappers;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "IdentityClient")]
    [OutputType(typeof(IdentityServer4.Models.Client))]
    public sealed class GetIdentityClientCommand : IdentityCommandBase
    {
        protected override void ProcessRecord()
        {
            using (this.ServiceProviderScope)
            using (this.Context)
            {
                this.Query().ToList().ForEach(c => this.WriteObject(c.ToModel()));
            }
        }
    }
}