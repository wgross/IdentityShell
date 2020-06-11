using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "IdentityClient")]
    [OutputType(typeof(IdentityServer4.Models.Client))]
    public class GetIdentityClientCommand : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            using var serviceScope = Startup.AppServices.GetService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            context.Clients.ToList().ForEach(client => this.WriteObject(client));
        }
    }
}