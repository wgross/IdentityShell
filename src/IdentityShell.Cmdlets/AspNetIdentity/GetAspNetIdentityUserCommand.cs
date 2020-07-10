using IdentityServerAspNetIdentity.Models;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Cmdlets.AspNetIdentity
{
    [Cmdlet(VerbsCommon.Get, "AspNetIdentityUser")]
    [OutputType(typeof(ApplicationUser))]
    public class GetAspNetIdentityUserCommand : AspNetIdentityUserCommandBase
    {
        protected override void ProcessRecord()
        {
            this.Context.Users.ToList().ForEach(this.WriteObject);
        }
    }
}