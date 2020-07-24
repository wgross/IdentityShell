using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Cmdlets.Configuration
{
    [Cmdlet(VerbsCommon.Get, "IdentityClient")]
    [CmdletBinding(DefaultParameterSetName = "all")]
    [OutputType(typeof(IdentityServer4.Models.Client))]
    public sealed class GetIdentityClientCommand : IdentityConfigurationCommandBase
    {
        [Parameter(ParameterSetName = "byname")]
        public string ClientId { get; set; }

        protected override void ProcessRecord()
        {
            if (this.ParameterSetName.Equals("byname"))
            {
                this.WriteObject(
                    Await(
                        this.LocalServiceProvider
                            .GetRequiredService<IClientStore>()
                            .FindClientByIdAsync(this.ClientId)));
            }
            else
            {
                this.QueryClients().ToList().ForEach(c => this.WriteObject(c.ToModel()));
            }
        }
    }
}