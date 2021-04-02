using Duende.IdentityServer;
using IdentityModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Reflection;
using System.Security.Claims;

namespace IdentityShell.Commands.Common
{
    public class NewClaimCommandCompleter : IArgumentCompleter
    {
        private readonly string[] claimTypesNames = new[]
        {
            ClaimTypes.AuthenticationInstant,
            ClaimTypes.AuthenticationMethod,
            ClaimTypes.CookiePath,
            ClaimTypes.DenyOnlyPrimarySid,
            ClaimTypes.DenyOnlyPrimaryGroupSid,
            ClaimTypes.DenyOnlyWindowsDeviceGroup,
            ClaimTypes.Dsa,
            ClaimTypes.Expiration,
            ClaimTypes.Expired,
            ClaimTypes.GroupSid,
            ClaimTypes.IsPersistent,
            ClaimTypes.PrimaryGroupSid,
            ClaimTypes.PrimarySid,
            ClaimTypes.Role,
            ClaimTypes.SerialNumber,
            ClaimTypes.UserData,
            ClaimTypes.Version,
            ClaimTypes.WindowsAccountName,
            ClaimTypes.WindowsDeviceClaim,
            ClaimTypes.WindowsDeviceGroup,
            ClaimTypes.WindowsUserClaim,
            ClaimTypes.WindowsFqbnVersion,
            ClaimTypes.WindowsSubAuthority,
            ClaimTypes.Anonymous,
            ClaimTypes.Authentication,
            ClaimTypes.AuthorizationDecision,
            ClaimTypes.Country,
            ClaimTypes.DateOfBirth,
            ClaimTypes.Dns,
            ClaimTypes.DenyOnlySid,
            ClaimTypes.Email,
            ClaimTypes.Gender,
            ClaimTypes.GivenName,
            ClaimTypes.Hash,
            ClaimTypes.HomePhone,
            ClaimTypes.Locality,
            ClaimTypes.MobilePhone,
            ClaimTypes.Name,
            ClaimTypes.NameIdentifier,
            ClaimTypes.OtherPhone,
            ClaimTypes.PostalCode,
            ClaimTypes.Rsa,
            ClaimTypes.Sid,
            ClaimTypes.Spn,
            ClaimTypes.StateOrProvince,
            ClaimTypes.StreetAddress,
            ClaimTypes.Surname,
            ClaimTypes.System,
            ClaimTypes.Thumbprint,
            ClaimTypes.Upn,
            ClaimTypes.Uri,
            ClaimTypes.Webpage,
            ClaimTypes.X500DistinguishedName,
            ClaimTypes.Actor
        };

        private readonly string[] jwtClaimTypeNames = new[]
        {
            JwtClaimTypes.Subject,
            JwtClaimTypes.Name,
            JwtClaimTypes.GivenName,
            JwtClaimTypes.FamilyName,
            JwtClaimTypes.MiddleName,
            JwtClaimTypes.NickName,
            JwtClaimTypes.PreferredUserName,
            JwtClaimTypes.Profile,
            JwtClaimTypes.Picture,
            JwtClaimTypes.WebSite,
            JwtClaimTypes.Email,
            JwtClaimTypes.EmailVerified,
            JwtClaimTypes.Gender,
            JwtClaimTypes.BirthDate,
            JwtClaimTypes.ZoneInfo,
            JwtClaimTypes.Locale,
            JwtClaimTypes.PhoneNumber,
            JwtClaimTypes.PhoneNumberVerified,
            JwtClaimTypes.Address,
            JwtClaimTypes.Audience,
            JwtClaimTypes.Issuer,
            JwtClaimTypes.NotBefore,
            JwtClaimTypes.Expiration,
            JwtClaimTypes.UpdatedAt,
            JwtClaimTypes.IssuedAt,
            JwtClaimTypes.AuthenticationMethod,
            JwtClaimTypes.SessionId,
            JwtClaimTypes.AuthenticationContextClassReference,
            JwtClaimTypes.AuthenticationTime,
            JwtClaimTypes.AuthorizedParty,
            JwtClaimTypes.AccessTokenHash,
            JwtClaimTypes.AuthorizationCodeHash,
            JwtClaimTypes.StateHash,
            JwtClaimTypes.Nonce,
            JwtClaimTypes.JwtId,
            JwtClaimTypes.Events,
            JwtClaimTypes.ClientId,
            JwtClaimTypes.Scope,
            JwtClaimTypes.Actor,
            JwtClaimTypes.MayAct,
            JwtClaimTypes.Id,
            JwtClaimTypes.IdentityProvider,
            JwtClaimTypes.Role,
            JwtClaimTypes.ReferenceTokenId,
            JwtClaimTypes.Confirmation,
        };

        private readonly string[] claimValueTypeNames = new[]
        {
            ClaimValueTypes.Base64Binary,
            ClaimValueTypes.UpnName,
            ClaimValueTypes.UInteger64,
            ClaimValueTypes.UInteger32,
            ClaimValueTypes.Time,
            ClaimValueTypes.String,
            ClaimValueTypes.Sid,
            ClaimValueTypes.RsaKeyValue,
            ClaimValueTypes.Rsa,
            ClaimValueTypes.Rfc822Name,
            ClaimValueTypes.KeyInfo,
            ClaimValueTypes.Integer64,
            ClaimValueTypes.X500Name,
            ClaimValueTypes.Integer32,
            ClaimValueTypes.HexBinary,
            ClaimValueTypes.Fqbn,
            ClaimValueTypes.Email,
            ClaimValueTypes.DsaKeyValue,
            ClaimValueTypes.Double,
            ClaimValueTypes.DnsName,
            ClaimValueTypes.DaytimeDuration,
            ClaimValueTypes.DateTime,
            ClaimValueTypes.Date,
            ClaimValueTypes.Boolean,
            ClaimValueTypes.Base64Octet,
            ClaimValueTypes.Integer,
            ClaimValueTypes.YearMonthDuration,
            Duende.IdentityServer.IdentityServerConstants.ClaimValueTypes.Json
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