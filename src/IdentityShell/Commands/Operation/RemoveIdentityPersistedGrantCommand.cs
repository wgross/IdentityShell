using IdentityShell.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Management.Automation;

namespace IdentityShell.Commands.Operation
{
    [Cmdlet(VerbsCommon.Remove, "IdentityPersitentGrant")]
    public class RemoveIdentityPersistedGrantCommand : IdentityCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Key { get; set; }

        protected override void ProcessRecord()
        {
            this.LocalServiceProvider
                .GetRequiredService<IPersistedGrantRepository>()
                .Remove(this.Key);
        }
    }
}