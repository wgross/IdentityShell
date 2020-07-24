using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Cmdlets.Configuration
{
    [Cmdlet(VerbsCommon.Get, "IdentityApiScope")]
    [CmdletBinding(DefaultParameterSetName = "all")]
    public sealed class GetIdentityApiScopeCommand : IdentityConfigurationCommandBase
    {
        [Parameter(ValueFromPipelineByPropertyName = true, ParameterSetName = "byname")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            if (this.ParameterSetName.Equals("byname"))
            {
                Await(
                   this.LocalServiceProvider
                       .GetRequiredService<IResourceStore>()
                       .FindApiScopesByNameAsync(new[] { this.Name }))
                    .ToList()
                    .ForEach(api => this.WriteObject(api));
            }
            else
            {
                Await(
                   this.LocalServiceProvider
                       .GetRequiredService<IResourceStore>()
                       .GetAllResourcesAsync())
                   .ApiScopes
                   .ToList()
                   .ForEach(api => this.WriteObject(api));
            }
        }
    }
}