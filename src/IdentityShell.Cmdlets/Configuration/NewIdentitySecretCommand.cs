using IdentityServer4.Models;
using System;
using System.Management.Automation;

namespace IdentityShell.Cmdlets
{
    [Cmdlet(VerbsCommon.New, "IdentitySecret")]
    [OutputType(typeof(Secret))]
    public class NewIdentitySecretCommand : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Value { get; set; }

        [Parameter()]
        [ValidateNotNullOrEmpty()]
        public string Description { get; set; }

        [Parameter()]
        public DateTime? Expiration { get; set; }

        protected override void ProcessRecord()
        {
            this.WriteObject(new Secret(this.Value, this.Description, this.Expiration));
        }
    }
}