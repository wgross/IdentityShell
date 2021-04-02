using Duende.IdentityServer.Models;
using IdentityShell.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Management.Automation;

namespace IdentityShell.Commands.Operation
{
    [Cmdlet(VerbsCommon.Get, "IdentityPersistedGrant")]
    [OutputType(typeof(PersistedGrant))]
    public sealed class GetIdentityPersistedGrantCommand : IdentityCommandBase
    {
        protected override void ProcessRecord()
        {
            this.LocalServiceProvider
                .GetRequiredService<IPersistedGrantRepository>()
                .Query()
                .ToList()
                .ForEach(m => this.WriteObject(m));
        }
    }
}