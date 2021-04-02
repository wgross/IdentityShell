using Duende.IdentityServer.Models;
using IdentityShell.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands.Configuration
{
    [Cmdlet(VerbsCommon.Get, "IdentityClient")]
    [CmdletBinding(DefaultParameterSetName = "all")]
    [OutputType(typeof(Client))]
    public sealed class GetIdentityClientCommand : IdentityCommandBase
    {
        [Parameter(ParameterSetName = "byname")]
        public string ClientId { get; set; }

        protected override void ProcessRecord()
        {
            if (this.ParameterSetName.Equals("byname"))
            {
                this.WriteObject(this.LocalServiceProvider
                    .GetRequiredService<IClientRepository>()
                    .FindClientById(this.ClientId));
            }
            else
            {
                this.LocalServiceProvider
                    .GetRequiredService<IClientRepository>()
                    .Query()
                    .ToList()
                    .ForEach(c => this.WriteObject(c));
            }
        }
    }
}