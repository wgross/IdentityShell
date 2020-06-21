using System.Linq;
using System.Management.Automation;
using System.Security.Claims;

namespace IdentityShell.Cmdlets
{
    [Cmdlet(VerbsCommon.New, "IdentityClaim")]
    [OutputType(typeof(Claim))]
    public class NewIdentityClaimCommand : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Type { get; set; }

        [Parameter(Mandatory = true)]
        public string Value { get; set; }

        [Parameter()]
        [ValidateSet(
            nameof(ClaimValueTypes.Base64Binary),
            nameof(ClaimValueTypes.UpnName),
            nameof(ClaimValueTypes.UInteger64),
            nameof(ClaimValueTypes.UInteger32),
            nameof(ClaimValueTypes.Time),
            nameof(ClaimValueTypes.String),
            nameof(ClaimValueTypes.Sid),
            nameof(ClaimValueTypes.RsaKeyValue),
            nameof(ClaimValueTypes.Rsa),
            nameof(ClaimValueTypes.Rfc822Name),
            nameof(ClaimValueTypes.KeyInfo),
            nameof(ClaimValueTypes.Integer64),
            nameof(ClaimValueTypes.X500Name),
            nameof(ClaimValueTypes.Integer32),
            nameof(ClaimValueTypes.HexBinary),
            nameof(ClaimValueTypes.Fqbn),
            nameof(ClaimValueTypes.Email),
            nameof(ClaimValueTypes.DsaKeyValue),
            nameof(ClaimValueTypes.Double),
            nameof(ClaimValueTypes.DnsName),
            nameof(ClaimValueTypes.DaytimeDuration),
            nameof(ClaimValueTypes.DateTime),
            nameof(ClaimValueTypes.Date),
            nameof(ClaimValueTypes.Boolean),
            nameof(ClaimValueTypes.Base64Octet),
            nameof(ClaimValueTypes.Integer),
            nameof(ClaimValueTypes.YearMonthDuration)
        )]
        public string ValueType { get; set; }

        protected override void ProcessRecord()
        {
            this.WriteObject(new Claim(this.Type, this.Value, this.ClaimTypeValue()));
        }

        private string ClaimTypeValue() => (string)typeof(ClaimValueTypes)
            .GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
            .Single(m => m.Name == this.ValueType)
            .GetValue(null);
    }
}