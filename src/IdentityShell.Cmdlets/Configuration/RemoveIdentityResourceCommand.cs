using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Cmdlets.Configuration
{
    [Cmdlet(VerbsCommon.Remove, "IdentityResource")]
    public class RemoveIdentityResourceCommand : IdentityConfigurationCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Name { get; set; }

        protected override void ProcessRecord()
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