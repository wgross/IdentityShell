using Duende.IdentityServer.Models;
using IdentityShell.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Management.Automation;

namespace IdentityShell.Commands.Operation
{
    [Cmdlet(VerbsCommon.Get, "IdentityDeviceCode")]
    [CmdletBinding(DefaultParameterSetName = "byDeviceCode")]
    [OutputType(typeof(DeviceCode))]
    public sealed class GetIdentityDeviceCodeCommand : IdentityCommandBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "byDeviceCode")]
        public string DeviceCode { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "byUserCode")]
        public string UserCode { get; set; }

        protected override void ProcessRecord()
        {
            if (this.ParameterSetName.Equals("byUserCode"))
            {
                var result = this.LocalServiceProvider
                    .GetRequiredService<IDeviceCodeRepository>()
                    .FindByUserCodeAsync(this.UserCode);

                if (result is not null)
                {
                    var pso = PSObject.AsPSObject(result);
                    pso.Properties.Add(new PSNoteProperty(nameof(UserCode), this.UserCode));
                    this.WriteObject(pso);
                }
            }
            else
            {
                var result = this.LocalServiceProvider
                    .GetRequiredService<IDeviceCodeRepository>()
                    .FindByDeviceCodeAsync(this.DeviceCode);

                if (result is { })
                {
                    var pso = PSObject.AsPSObject(result);
                    pso.Properties.Add(new PSNoteProperty(nameof(DeviceCode), this.DeviceCode));
                    this.WriteObject(pso);
                }
            }
        }
    }
}