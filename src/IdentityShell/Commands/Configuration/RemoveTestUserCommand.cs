using Duende.IdentityServer.Test;
using IdentityShell.Commands.Configuration.ArgumentCompleters;
using IdentityShell.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands.Configuration
{
    [Cmdlet(VerbsCommon.Remove, "TestUser")]
    [CmdletBinding(DefaultParameterSetName = "all")]
    [OutputType(typeof(TestUser))]
    public class RemoveTestUserCommand : IdentityCommandBase
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ArgumentCompleter(typeof(TestUserNameCompleter))]
        public string Username { get; set; }

        protected override void ProcessRecord()
        {
            var user = this.LocalServiceProvider
               .GetRequiredService<ITestUserRepository>()
               .Query(c => c.Username == this.Username)
               .FirstOrDefault();

            if (user is null)
            {
                this.WriteError(new ErrorRecord(
                    exception: new PSInvalidOperationException($"client(clientId='{0}') doesn't exist"),
                    errorId: "client.not_found",
                    errorCategory: ErrorCategory.ObjectNotFound,
                    targetObject: this.Username));
                return;
            }

            this.LocalServiceProvider
                .GetRequiredService<ITestUserRepository>()
                .Remove(user);
        }
    }
}