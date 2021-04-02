using IdentityShell.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands.Configuration
{
    [Cmdlet(VerbsCommon.Get, "IdentityApiScope")]
    [CmdletBinding(DefaultParameterSetName = "all")]
    public sealed class GetIdentityApiScopeCommand : IdentityCommandBase
    {
        [Parameter(ValueFromPipelineByPropertyName = true, ParameterSetName = "byname")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            if (this.ParameterSetName.Equals("byname"))
            {
                this.LocalServiceProvider
                    .GetRequiredService<IApiScopeRepository>()
                    .FindApiScopesByName(new[] { this.Name })
                    .ToList()
                    .ForEach(api => this.WriteObject(api));
            }
            else
            {
                this.LocalServiceProvider
                    .GetRequiredService<IApiScopeRepository>()
                    .Query()
                    .ToList()
                    .ForEach(api => this.WriteObject(api));
            }
        }
    }
}