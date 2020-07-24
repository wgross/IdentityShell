using System.Linq;
using System.Management.Automation;
using System.Security.Claims;

namespace IdentityShell.Cmdlets.AspNetIdentity
{
    [Cmdlet(VerbsCommon.Get, "AspNetIdentityUserClaim")]
    [OutputType(typeof(Claim))]
    public class GetAspNetIdentityUserClaimCommand : AspNetIdentityUserCommandBase
    {
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string UserName { get; set; }

        protected override void ProcessRecord()
        {
            Await(
                this.UserManager.GetClaimsAsync(Await(
                    this.UserManager.FindByNameAsync(this.UserName))))
            .ToList()
            .ForEach(c => this.WriteObject(c));
        }
    }
}