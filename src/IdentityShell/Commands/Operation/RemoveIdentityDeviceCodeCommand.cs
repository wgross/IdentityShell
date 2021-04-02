using IdentityShell.Commands;
using IdentityShell.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Management.Automation;

namespace IdentityShell.Cmdlets.Operation
{
    [Cmdlet(VerbsCommon.Remove, "IdentityDeviceCode")]
    public class RemoveIdentityDeviceCodeCommand : IdentityCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string DeviceCode { get; set; }

        protected override void ProcessRecord()
        {
            this.LocalServiceProvider
                .GetRequiredService<IDeviceCodeRepository>()
                .RemoveByDeviceCodeAsync(this.DeviceCode);
        }
    }
}