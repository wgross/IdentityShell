using IdentityShell.Commands.Configuration.ArgumentCompleters;
using IdentityShell.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands.Configuration
{
    [Cmdlet(VerbsCommon.Remove, "IdentityApiResource")]
    public class RemoveIdentityApiResourceCommand : IdentityCommandBase
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ArgumentCompleter(typeof(IdentityApiResourceNameCompleter))]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            var apiEntity = this.LocalServiceProvider
                .GetRequiredService<IApiResourceRepository>()
                .Query(c => c.Name == this.Name)
                .FirstOrDefault();

            if (apiEntity is null)
            {
                this.WriteError(new ErrorRecord(
                    exception: new PSInvalidOperationException($"api(name='{0}') doesn't exist"),
                    errorId: "api.not_found",
                    errorCategory: ErrorCategory.ObjectNotFound,
                    targetObject: this.Name));
                return;
            }

            this.LocalServiceProvider.GetRequiredService<IApiResourceRepository>().Remove(apiEntity);
        }
    }
}