using IdentityShell.Commands.Configuration.ArgumentCompleters;
using IdentityShell.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands.Configuration
{
    [Cmdlet(VerbsCommon.Remove, "IdentityApiScope")]
    public sealed class RemoveIdentityApiScopeCommand : IdentityCommandBase
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ArgumentCompleter(typeof(IdentityApiScopeNameCompleter))]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            var apiScope = this.LocalServiceProvider
                .GetRequiredService<IApiScopeRepository>()
                .Query(s => s.Name.Equals(this.Name))
                .FirstOrDefault();

            if (apiScope is null)
            {
                this.WriteError(new ErrorRecord(
                    exception: new PSInvalidOperationException($"apiScope(name='{0}') doesn't exist"),
                    errorId: "api_scope.not_found",
                    errorCategory: ErrorCategory.ObjectNotFound,
                    targetObject: this.Name));
                return;
            }

            this.LocalServiceProvider
                .GetRequiredService<IApiScopeRepository>()
                .Remove(apiScope);
        }
    }
}