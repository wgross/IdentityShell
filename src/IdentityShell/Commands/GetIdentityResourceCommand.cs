using IdentityServer4.EntityFramework.Mappers;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "IdentityResource")]
    [OutputType(typeof(IdentityServer4.Models.IdentityResource))]
    public class GetIdentityResourceCommand : IdentityCommandBase
    {
        protected override void ProcessRecord()
        {        
            this.QueryIdentityResource().ToList().ForEach(ir => this.WriteObject(ir.ToModel()));
        }
    }
}