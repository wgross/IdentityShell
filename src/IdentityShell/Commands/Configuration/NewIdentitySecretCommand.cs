using Duende.IdentityServer.Models;
using System;
using System.Management.Automation;

namespace IdentityShell.Commands
{
    [Cmdlet(VerbsCommon.New, "IdentitySecret")]
    [CmdletBinding(DefaultParameterSetName = nameof(PlainText))]
    [OutputType(typeof(Secret))]
    public class NewIdentitySecretCommand : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ParameterSetName = nameof(Value),
            HelpMessage = "Literal value of the 'Value' property of the created secret. A hashed sha256 hashed value is usually expected here.")]
        public string Value { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 0,
            ParameterSetName = nameof(PlainText),
            HelpMessage = "The given plan text is hashed sih SHA256 and then base64 encoded.")]
        public string PlainText { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Optional descriptive text of the secret")]
        [ValidateNotNullOrEmpty()]
        public string Description { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Date and time where the secret is no longer valid.")]
        public DateTime? Expiration { get; set; }

        protected override void ProcessRecord()
        {
            switch (this.ParameterSetName)
            {
                case nameof(Value):

                    this.WriteObject(new Secret(this.Value, this.Description, this.Expiration));
                    break;

                default:

                    this.WriteObject(new Secret(this.PlainText.Sha256(), this.Description, this.Expiration));
                    break;
            }
        }
    }
}