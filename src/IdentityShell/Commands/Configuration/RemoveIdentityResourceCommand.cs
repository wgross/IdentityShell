using IdentityShell.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands.Configuration
{
    [Cmdlet(VerbsCommon.Remove, "IdentityResource")]
    public class RemoveIdentityResourceCommand : IdentityCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            var identityEntity = this.LocalServiceProvider
                .GetRequiredService<IIdentityResourceRepository>()
                .Query(c => c.Name == this.Name)
                .FirstOrDefault();

            if (identityEntity is null)
            {
                this.WriteError(new ErrorRecord(
                    exception: new PSInvalidOperationException($"identity(name='{0}') doesn't exist"),
                    errorId: "identity.not_found",
                    errorCategory: ErrorCategory.ObjectNotFound,
                    targetObject: this.Name));
                return;
            }

            this.LocalServiceProvider
               .GetRequiredService<IIdentityResourceRepository>()
               .Remove(identityEntity);
        }
    }
}