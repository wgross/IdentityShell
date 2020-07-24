using IdentityServerAspNetIdentity.Models;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Cmdlets.AspNetIdentity
{
    [Cmdlet(VerbsCommon.Get, "AspNetIdentityUser")]
    [CmdletBinding(DefaultParameterSetName = "all")]
    [OutputType(typeof(ApplicationUser))]
    public class GetAspNetIdentityUserCommand : AspNetIdentityUserCommandBase
    {
        [Parameter(ParameterSetName = "byname", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public string UserName { get; set; }

        protected override void ProcessRecord()
        {
            switch (this.ParameterSetName)
            {
                case "byname":
                    this.WriteObject(Await(this.UserManager.FindByNameAsync(this.UserName)));
                    break;

                default:
                    this.Context.Users.ToList().ForEach(this.WriteObject);
                    break;
            }
        }
    }
}