using Duende.IdentityServer;
using IdentityModel;
using IdentityShell.Commands.Common.ArgumentCompleters;
using System;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Security.Claims;

namespace IdentityShell.Commands.Common
{
    [Cmdlet(VerbsCommon.New, "Claim")]
    [OutputType(typeof(Claim))]
    public class NewClaimCommand : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Identifier of the claim")]
        [ArgumentCompleter(typeof(ClaimTypeCompleter))]
        public string Type { get; set; }

        [Parameter(Mandatory = true)]
        public string Value { get; set; }

        [Parameter()]
        [ArgumentCompleter(typeof(ClaimValueTypeCompleter))]
        public string ValueType { get; set; }

        [Parameter()]
        public string Issuer { get; set; }

        [Parameter()]
        public string OriginalIssuer { get; set; }

        protected override void ProcessRecord() => this.WriteObject(new Claim(this.ClaimType(), this.Value, this.ClaimTypeValue(), this.Issuer, this.OriginalIssuer));

        private string ClaimTypeValue() => this.MapToFieldName(this.ValueType, typeof(ClaimValueTypes));

        private string ClaimType() => this.MapToFieldName(this.Type, typeof(JwtClaimTypes), typeof(IdentityServerConstants.ClaimValueTypes));

        private string MapToFieldName(string value, params Type[] types)
        {
            FieldInfo getFirstField()
            {
                foreach (var type in types)
                {
                    var symbolicField = type
                        .GetFields(BindingFlags.Static | BindingFlags.Public)
                        .FirstOrDefault(m => m.Name == value);

                    if (symbolicField is { })
                        return symbolicField;
                }
                return null;
            }

            var symbolicField = getFirstField();
            if (symbolicField is null)
                return value;
            else
                return (string)symbolicField.GetValue(null);
        }
    }
}