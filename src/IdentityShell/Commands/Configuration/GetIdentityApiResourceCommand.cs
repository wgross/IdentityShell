using Duende.IdentityServer.Models;
using IdentityShell.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "IdentityApiResource")]
    [CmdletBinding(DefaultParameterSetName = "all")]
    [OutputType(typeof(ApiResource))]
    public class GetIdentityApiResourceCommand : IdentityCommandBase
    {
        [Parameter(ParameterSetName = "byname")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            if (this.ParameterSetName.Equals("byname"))
            {
                this.LocalServiceProvider
                    .GetRequiredService<IApiResourceRepository>()
                    .FindApiResourcesByName(new[] { this.Name })
                    .ToList()
                .ForEach(api => this.WriteObject(api));
            }
            else
            {
                this.LocalServiceProvider
                    .GetRequiredService<IApiResourceRepository>()
                    .Query()
                    .ToList()
                    .ForEach(api => this.WriteObject(api));
            }
        }
    }
}