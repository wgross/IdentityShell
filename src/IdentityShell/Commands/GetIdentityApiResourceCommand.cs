using IdentityServer4.EntityFramework.Mappers;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "IdentityApiResource")]
    [OutputType(typeof(IdentityServer4.Models.ApiResource))]
    public class GetIdentityApiResourceCommand : IdentityCommandBase
    {
        protected override void ProcessRecord()
        {            
            this.QueryApiResource().ToList().ForEach(c => this.WriteObject(c.ToModel()));
        }
    }
}