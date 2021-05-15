using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Security.Claims;

namespace IdentityShell.Commands.Common.ArgumentCompleters
{
    public sealed class ClaimValueTypeCompleter : IArgumentCompleter
    {
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
            return claimValueTypeNames
                .Where(n => n.Contains(wordToComplete ?? string.Empty, StringComparison.OrdinalIgnoreCase))
                .Select(n => new CompletionResult(n));
        }
    }
}