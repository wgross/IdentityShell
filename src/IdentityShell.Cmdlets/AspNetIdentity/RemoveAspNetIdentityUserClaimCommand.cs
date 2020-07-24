using System.Management.Automation;
using System.Security.Claims;

namespace IdentityShell.Cmdlets.AspNetIdentity
{
    [Cmdlet(VerbsCommon.Remove, "AspNetIdentityUserClaim")]
    public sealed class RemoveAspNetIdentityUserClaimCommand : AspNetIdentityUserCommandBase
    {
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string UserName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public Claim InputObject { get; set; }

        protected override void ProcessRecord()
        {
            var user = Await(this.UserManager.FindByNameAsync(this.UserName));

            Await(this.UserManager.RemoveClaimAsync(user, this.InputObject));
        }
    }
}