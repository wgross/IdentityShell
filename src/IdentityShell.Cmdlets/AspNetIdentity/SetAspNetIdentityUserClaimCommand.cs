using System.Linq;
using System.Management.Automation;
using System.Security.Claims;

namespace IdentityShell.Cmdlets.AspNetIdentity
{
    [Cmdlet(VerbsCommon.Set, "AspNetIdentityUserClaim")]
    public class SetAspNetIdentityUserClaimCommand : AspNetIdentityUserCommandBase
    {
        [Parameter(Mandatory = true, Position = 0)]
        [ValidateNotNullOrEmpty]
        public string UserName { get; set; }

        [Parameter(ValueFromPipeline = true)]
        public Claim InputObject { get; set; }

        protected override void ProcessRecord()
        {
            var user = Await(this.UserManager.FindByNameAsync(this.UserName));
            var claims = Await(this.UserManager.GetClaimsAsync(user));
            var claimToModify = claims.SingleOrDefault(c => c.Type.Equals(this.InputObject.Type));
            if (claimToModify is null)
            {
                Await(this.UserManager.AddClaimAsync(user, this.InputObject));
            }
            else
            {
                Await(this.UserManager.ReplaceClaimAsync(user, claimToModify, this.InputObject));
            }
        }
    }
}