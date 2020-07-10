using IdentityServer4.Stores;
using IdentityShell.Cmdlets.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "IdentityApiResource")]
    [OutputType(typeof(IdentityServer4.Models.ApiResource))]
    public class GetIdentityApiResourceCommand : IdentityConfigurationCommandBase
    {
        protected override void ProcessRecord()
        {
            this.AwaitResult(
                this.LocalServiceProvider
                    .GetRequiredService<IResourceStore>()
                    .GetAllResourcesAsync())
                .ApiResources
                .ToList()
                .ForEach(api => this.WriteObject(api));

            //this.QueryApiResource().ToList().ForEach(c => this.WriteObject(c.ToModel()));
        }
    }
}