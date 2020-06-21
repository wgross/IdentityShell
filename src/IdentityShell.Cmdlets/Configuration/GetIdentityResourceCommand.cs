using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Cmdlets.Configuration
{
    [Cmdlet(VerbsCommon.Get, "IdentityResource")]
    [OutputType(typeof(IdentityServer4.Models.IdentityResource))]
    public class GetIdentityResourceCommand : IdentityConfigurationCommandBase
    {
        protected override void ProcessRecord()
        {
            this.LocalServiceProvider
                .GetRequiredService<IResourceStore>()
                .GetAllResourcesAsync()
                .Result
                .IdentityResources
                .ToList()
                .ForEach(id => this.WriteObject(id));
            //this.QueryIdentityResource().ToList().ForEach(ir => this.WriteObject(ir.ToModel()));
        }
    }
}