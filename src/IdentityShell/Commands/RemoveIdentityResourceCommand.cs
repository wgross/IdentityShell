using IdentityServer4.Models;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "IdentityResource")]
    public class RemoveIdentityResourceCommand : IdentityCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Name { get; set; }

        [Parameter(ValueFromPipeline = true)]
        public IdentityResource InputObject { get; set; }

        protected override void ProcessRecord()
        {
            using (this.ServiceProviderScope)
            using (this.Context)
            {
                var identityEntity = this.QueryIdentityResource().SingleOrDefault(c => c.Name == this.Name);

                if (identityEntity is null)
                {
                    this.WriteError(new ErrorRecord(
                        exception: new PSInvalidOperationException($"identity(name='{0}') doesn't exist"),
                        errorId: "identity.not_found",
                        errorCategory: ErrorCategory.ObjectNotFound,
                        targetObject: this.Name));
                    return;
                }

                this.Context.IdentityResources.Remove(identityEntity);
                this.Context.SaveChanges();
            }
        }
    }
}