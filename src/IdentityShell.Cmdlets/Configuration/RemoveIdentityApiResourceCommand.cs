using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Cmdlets.Configuration
{
    [Cmdlet(VerbsCommon.Remove, "IdentityApiResource")]
    public class RemoveIdentityApiResourceCommand : IdentityConfigurationCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            var apiEntity = this.QueryApiResource().FirstOrDefault(c => c.Name == this.Name);

            if (apiEntity is null)
            {
                this.WriteError(new ErrorRecord(
                    exception: new PSInvalidOperationException($"api(name='{0}') doesn't exist"),
                    errorId: "api.not_found",
                    errorCategory: ErrorCategory.ObjectNotFound,
                    targetObject: this.Name));
                return;
            }

            this.Context.ApiResources.Remove(apiEntity);
            this.Context.SaveChanges();
        }
    }
}