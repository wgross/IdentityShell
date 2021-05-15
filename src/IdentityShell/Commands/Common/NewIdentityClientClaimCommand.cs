using Duende.IdentityServer.Models;
using IdentityShell.Commands.Common.ArgumentCompleters;
using System.Management.Automation;

namespace IdentityShell.Commands.Common
{
    [Cmdlet(VerbsCommon.New, "IdentityClientClaim")]
    [OutputType(typeof(ClientClaim))]
    public class NewIdentityClientClaimCommand : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        [ArgumentCompleter(typeof(ClaimTypeCompleter))]
        public string Type { get; set; }

        [Parameter(Mandatory = true)]
        public string Value { get; set; }

        [Parameter()]
        [ArgumentCompleter(typeof(ClaimTypeCompleter))]
        public string ValueType { get; set; }

        protected override void ProcessRecord() => this.WriteObject(new ClientClaim(this.Type, this.Value, this.ValueType));
    }
}