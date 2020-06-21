using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using System.Management.Automation;

namespace IdentityShell.Cmdlets.Operation
{
    [Cmdlet(VerbsCommon.Remove, "IdentityDeviceCode")]
    public class RemoveIdentityDeviceCodeCommand : IdentityOperationCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string DeviceCode { get; set; }

        protected override void ProcessRecord()
        {
            this
                .LocalServiceProvider
                .GetRequiredService<IDeviceFlowStore>()
                .RemoveByDeviceCodeAsync(this.DeviceCode)
                .Wait();
        }
    }
}