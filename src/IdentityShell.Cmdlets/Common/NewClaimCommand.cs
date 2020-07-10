using IdentityModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Reflection;
using System.Security.Claims;

namespace IdentityShell.Cmdlets
{
    public class NewClaimCommandCompleter : IArgumentCompleter
    {
        private readonly string[] claimTypesNames = new[]
        {
            nameof(ClaimTypes.AuthenticationInstant),
            nameof(ClaimTypes.AuthenticationMethod),
            nameof(ClaimTypes.CookiePath),
            nameof(ClaimTypes.DenyOnlyPrimarySid),
            nameof(ClaimTypes.DenyOnlyPrimaryGroupSid),
            nameof(ClaimTypes.DenyOnlyWindowsDeviceGroup),
            nameof(ClaimTypes.Dsa),
            nameof(ClaimTypes.Expiration),
            nameof(ClaimTypes.Expired),
            nameof(ClaimTypes.GroupSid),
            nameof(ClaimTypes.IsPersistent),
            nameof(ClaimTypes.PrimaryGroupSid),
            nameof(ClaimTypes.PrimarySid),
            nameof(ClaimTypes.Role),
            nameof(ClaimTypes.SerialNumber),
            nameof(ClaimTypes.UserData),
            nameof(ClaimTypes.Version),
            nameof(ClaimTypes.WindowsAccountName),
            nameof(ClaimTypes.WindowsDeviceClaim),
            nameof(ClaimTypes.WindowsDeviceGroup),
            nameof(ClaimTypes.WindowsUserClaim),
            nameof(ClaimTypes.WindowsFqbnVersion),
            nameof(ClaimTypes.WindowsSubAuthority),
            nameof(ClaimTypes.Anonymous),
            nameof(ClaimTypes.Authentication),
            nameof(ClaimTypes.AuthorizationDecision),
            nameof(ClaimTypes.Country),
            nameof(ClaimTypes.DateOfBirth),
            nameof(ClaimTypes.Dns),
            nameof(ClaimTypes.DenyOnlySid),
            nameof(ClaimTypes.Email),
            nameof(ClaimTypes.Gender),
            nameof(ClaimTypes.GivenName),
            nameof(ClaimTypes.Hash),
            nameof(ClaimTypes.HomePhone),
            nameof(ClaimTypes.Locality),
            nameof(ClaimTypes.MobilePhone),
            nameof(ClaimTypes.Name),
            nameof(ClaimTypes.NameIdentifier),
            nameof(ClaimTypes.OtherPhone),
            nameof(ClaimTypes.PostalCode),
            nameof(ClaimTypes.Rsa),
            nameof(ClaimTypes.Sid),
            nameof(ClaimTypes.Spn),
            nameof(ClaimTypes.StateOrProvince),
            nameof(ClaimTypes.StreetAddress),
            nameof(ClaimTypes.Surname),
            nameof(ClaimTypes.System),
            nameof(ClaimTypes.Thumbprint),
            nameof(ClaimTypes.Upn),
            nameof(ClaimTypes.Uri),
            nameof(ClaimTypes.Webpage),
            nameof(ClaimTypes.X500DistinguishedName),
            nameof(ClaimTypes.Actor)
        };

        private readonly string[] jwtClaimTypeNames = new[]
        {
            nameof(JwtClaimTypes.Subject),
            nameof(JwtClaimTypes.Name),
            nameof(JwtClaimTypes.GivenName),
            nameof(JwtClaimTypes.FamilyName),
            nameof(JwtClaimTypes.MiddleName),
            nameof(JwtClaimTypes.NickName),
            nameof(JwtClaimTypes.PreferredUserName),
            nameof(JwtClaimTypes.Profile),
            nameof(JwtClaimTypes.Picture),
            nameof(JwtClaimTypes.WebSite),
            nameof(JwtClaimTypes.Email),
            nameof(JwtClaimTypes.EmailVerified),
            nameof(JwtClaimTypes.Gender),
            nameof(JwtClaimTypes.BirthDate),
            nameof(JwtClaimTypes.ZoneInfo),
            nameof(JwtClaimTypes.Locale),
            nameof(JwtClaimTypes.PhoneNumber),
            nameof(JwtClaimTypes.PhoneNumberVerified),
            nameof(JwtClaimTypes.Address),
            nameof(JwtClaimTypes.Audience),
            nameof(JwtClaimTypes.Issuer),
            nameof(JwtClaimTypes.NotBefore),
            nameof(JwtClaimTypes.Expiration),
            nameof(JwtClaimTypes.UpdatedAt),
            nameof(JwtClaimTypes.IssuedAt),
            nameof(JwtClaimTypes.AuthenticationMethod),
            nameof(JwtClaimTypes.SessionId),
            nameof(JwtClaimTypes.AuthenticationContextClassReference),
            nameof(JwtClaimTypes.AuthenticationTime),
            nameof(JwtClaimTypes.AuthorizedParty),
            nameof(JwtClaimTypes.AccessTokenHash),
            nameof(JwtClaimTypes.AuthorizationCodeHash),
            nameof(JwtClaimTypes.StateHash),
            nameof(JwtClaimTypes.Nonce),
            nameof(JwtClaimTypes.JwtId),
            nameof(JwtClaimTypes.Events),
            nameof(JwtClaimTypes.ClientId),
            nameof(JwtClaimTypes.Scope),
            nameof(JwtClaimTypes.Actor),
            nameof(JwtClaimTypes.MayAct),
            nameof(JwtClaimTypes.Id),
            nameof(JwtClaimTypes.IdentityProvider),
            nameof(JwtClaimTypes.Role),
            nameof(JwtClaimTypes.ReferenceTokenId),
            nameof(JwtClaimTypes.Confirmation),
        };

        private readonly string[] claimValueTypeNames = new[]
        {
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
            nameof(ClaimValueTypes.YearMonthDuration),
            nameof(IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
        };

        public IEnumerable<CompletionResult> CompleteArgument(string commandName, string parameterName, string wordToComplete, CommandAst commandAst, IDictionary fakeBoundParameters)
        {
            return parameterName switch
            {
                nameof(NewClaimCommand.Type) => this.Complete(wordToComplete, jwtClaimTypeNames),
                nameof(NewClaimCommand.ValueType) => this.Complete(wordToComplete, claimValueTypeNames),
                _ => Enumerable.Empty<CompletionResult>()
            };
        }

        private IEnumerable<CompletionResult> Complete(string wordToComplete, string[] claimTypesNames) => claimTypesNames
            .Where(n => n.StartsWith(wordToComplete ?? string.Empty, ignoreCase: true, culture: null))
            .Select(n => new CompletionResult(n));
    }

    [Cmdlet(VerbsCommon.New, "Claim")]
    [OutputType(typeof(Claim))]
    public class NewClaimCommand : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        [ArgumentCompleter(typeof(NewClaimCommandCompleter))]
        public string Type { get; set; }

        [Parameter(Mandatory = true)]
        public string Value { get; set; }

        [Parameter()]
        [ArgumentCompleter(typeof(NewClaimCommandCompleter))]
        public string ValueType { get; set; }

        protected override void ProcessRecord() => this.WriteObject(new Claim(this.ClaimType(), this.Value, this.ClaimTypeValue()));

        private string ClaimTypeValue() => this.MapToFieldName(this.ValueType, typeof(ClaimValueTypes));

        private string ClaimType() => this.MapToFieldName(this.Type, typeof(JwtClaimTypes), typeof(IdentityServer4.IdentityServerConstants.ClaimValueTypes));

        private string MapToFieldName(string value, params Type[] types)
        {
            FieldInfo getFirstField()
            {
                foreach (var type in types)
                {
                    var symbolicField = type
                        .GetFields(BindingFlags.Static | BindingFlags.Public)
                        .SingleOrDefault(m => m.Name == value);

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