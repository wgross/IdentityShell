using Duende.IdentityServer.Test;
using IdentityShell.Commands.Configuration.ArgumentCompleters;
using IdentityShell.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands.Configuration
{
    [Cmdlet(VerbsCommon.Get, "TestUser")]
    [CmdletBinding(DefaultParameterSetName = "all")]
    [OutputType(typeof(TestUser))]
    public class GetTestUserCommand : IdentityCommandBase
    {
        [Parameter(ParameterSetName = "byname", Position = 0, ValueFromPipelineByPropertyName = true)]
        [ArgumentCompleter(typeof(TestUserNameCompleter))]
        [ValidateNotNullOrEmpty()]
        public string Username { get; set; }

        protected override void ProcessRecord()
        {
            if (this.ParameterSetName.Equals("byname"))
            {
                this.WriteObject(this.LocalServiceProvider
                    .GetRequiredService<ITestUserRepository>()
                    .FindUsername(this.Username));
            }
            else
            {
                this.LocalServiceProvider
                    .GetRequiredService<ITestUserRepository>()
                    .Query()
                    .ToList()
                    .ForEach(u => this.WriteObject(u));
            }
        }
    }
}