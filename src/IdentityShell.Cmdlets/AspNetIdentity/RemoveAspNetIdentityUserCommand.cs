using System.Management.Automation;

namespace IdentityShell.Cmdlets.AspNetIdentity
{
    [Cmdlet(VerbsCommon.Remove, "AspNetIdentityUser")]
    public sealed class RemoveAspNetIdentityUserCommand : AspNetIdentityUserCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string UserName { get; set; }

        protected override void ProcessRecord()
        {
            Await(this.UserManager.DeleteAsync(
                Await(this.UserManager.FindByNameAsync(this.UserName))));
        }
    }
}