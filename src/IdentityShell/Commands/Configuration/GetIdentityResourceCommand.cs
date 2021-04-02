using IdentityShell.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands.Configuration
{
    [Cmdlet(VerbsCommon.Get, "IdentityResource")]
    [OutputType(typeof(Duende.IdentityServer.Models.IdentityResource))]
    public class GetIdentityResourceCommand : IdentityCommandBase
    {
        protected override void ProcessRecord()
        {
            this.LocalServiceProvider
                .GetRequiredService<IIdentityResourceRepository>()
                .GetAll()
                .ToList()
                .ForEach(id => this.WriteObject(id));
        }
    }
};