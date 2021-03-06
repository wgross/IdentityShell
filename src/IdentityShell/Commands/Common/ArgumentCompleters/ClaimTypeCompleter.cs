﻿using IdentityModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Security.Claims;

namespace IdentityShell.Commands.Common.ArgumentCompleters
{
    public sealed class ClaimTypeCompleter : IArgumentCompleter
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
            ClaimTypes.Actor,

            // JWT cliam types
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

        public IEnumerable<CompletionResult> CompleteArgument(string commandName, string parameterName, string wordToComplete, CommandAst commandAst, IDictionary fakeBoundParameters)
        {
            return this.claimTypesNames
                .Where(n => n.Contains(wordToComplete ?? string.Empty, StringComparison.OrdinalIgnoreCase))
                .Select(n => new CompletionResult(n));
        }
    }
}