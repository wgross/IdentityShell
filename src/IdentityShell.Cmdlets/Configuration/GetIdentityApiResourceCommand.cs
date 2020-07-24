using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityShell.Cmdlets.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "IdentityApiResource")]
    [CmdletBinding(DefaultParameterSetName = "all")]
    [OutputType(typeof(ApiResource))]
    public class GetIdentityApiResourceCommand : IdentityConfigurationCommandBase
    {
        [Parameter(ParameterSetName = "byname")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            if (this.ParameterSetName.Equals("byname"))
            {
                Await(
                    this.LocalServiceProvider
                        .GetRequiredService<IResourceStore>()
                        .FindApiResourcesByNameAsync(new[] { this.Name }))
                .ToList()
                .ForEach(api => this.WriteObject(api));
            }
            else
            {
                Await(
                    this.LocalServiceProvider
                        .GetRequiredService<IResourceStore>()
                        .GetAllResourcesAsync())
                .ApiResources
                .ToList()
                .ForEach(api => this.WriteObject(api));
            }
        }
    }
}