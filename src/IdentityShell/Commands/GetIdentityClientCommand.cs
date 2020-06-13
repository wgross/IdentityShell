using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "IdentityClient")]
    [OutputType(typeof(IdentityServer4.Models.Client))]
    public sealed class GetIdentityClientCommand : IdentityCommandBase
    {
        protected override void ProcessRecord()
        {
            using var context = ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            context.Clients.ToList().ForEach(client => this.WriteObject(client));
        }
    }
}