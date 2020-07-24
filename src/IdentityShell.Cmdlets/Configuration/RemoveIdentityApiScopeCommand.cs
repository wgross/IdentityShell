using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Cmdlets.Configuration
{
    [Cmdlet(VerbsCommon.Remove, "IdentityApiScope")]
    public sealed class RemoveIdentityApiScopeCommand : IdentityConfigurationCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            var apiScopeEntity = this.QueryApiScopes().FirstOrDefault(s => s.Name.Equals(this.Name));

            if (apiScopeEntity is null)
            {
                this.WriteError(new ErrorRecord(
                    exception: new PSInvalidOperationException($"apiScope(name='{0}') doesn't exist"),
                    errorId: "api_scope.not_found",
                    errorCategory: ErrorCategory.ObjectNotFound,
                    targetObject: this.Name));
                return;
            }

            this.Context.ApiScopes.Remove(apiScopeEntity);
            this.Context.SaveChanges();
        }
    }
}