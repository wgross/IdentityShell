using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using System.Management.Automation;

namespace IdentityShell.Cmdlets.Operation
{
    public class RemoveIdentityPersistedGrantCommand : IdentityOperationCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Key { get; set; }

        protected override void ProcessRecord()
        {
            this
                .LocalServiceProvider
                .GetRequiredService<IPersistedGrantStore>()
                .RemoveAsync(this.Key);
        }
    }
}