using Microsoft.PowerShell;
using System.Management.Automation.Runspaces;

namespace IdentityShell.Cmdlets.Common
{
    public static class InitialSessionStateExtensions
    {
        public static InitialSessionState AddCommonCommands(this InitialSessionState sessionState)
        {
            sessionState.Commands.Add(new SessionStateCmdletEntry("New-Claim", typeof(NewClaimCommand), string.Empty));
            sessionState.Commands.Add(new SessionStateCmdletEntry("New-IdentityClientClaim", typeof(NewIdentityClientClaimCommand), string.Empty));

            return sessionState;
        }
    }
}