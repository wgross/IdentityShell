using IdentityShell.Commands.Configuration.ArgumentCompleters;
using IdentityShell.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands.Configuration
{
    [Cmdlet(VerbsCommon.Remove, "IdentityClient")]
    public class RemoveIdentityClientCommand : IdentityCommandBase
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ArgumentCompleter(typeof(IdentityClientIdCompleter))]
        public string ClientId { get; set; }

        protected override void ProcessRecord()
        {
            var client = this.LocalServiceProvider
                .GetRequiredService<IClientRepository>()
                .Query(c => c.ClientId == this.ClientId)
                .FirstOrDefault();

            if (client is null)
            {
                this.WriteError(new ErrorRecord(
                    exception: new PSInvalidOperationException($"client(clientId='{0}') doesn't exist"),
                    errorId: "client.not_found",
                    errorCategory: ErrorCategory.ObjectNotFound,
                    targetObject: this.ClientId));
                return;
            }

            this.LocalServiceProvider
                .GetRequiredService<IClientRepository>()
                .Remove(client);
        }
    }
}